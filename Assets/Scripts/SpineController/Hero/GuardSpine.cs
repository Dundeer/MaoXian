using QFramework;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

public enum HeroAttackMode
{
    Normal,
    Special
}

public class GuardSpine : HeroSpine
{
    #region Member
    /// <summary>
    /// 普通状态资源
    /// </summary>
    public SkeletonDataAsset NormalData;
    /// <summary>
    /// 攻击状态资源
    /// </summary>
    public SkeletonDataAsset AttackData;

    /// <summary>
    /// 当前攻击类型
    /// </summary>
    private int AttackType;
    /// <summary>
    /// 当前创建箭矢的携程
    /// </summary>
    private Coroutine createArrowCoroutine;
    /// <summary>
    /// 剑雨数量
    /// </summary>
    private int rainArrowNumber = 0;

    private bool isStandy = true;
    #endregion

    #region Unity
    private void Start()
    {
        AttackPos.gameObject.SetActive(false);
        currentBloodLine.IniteBlood(CurrentBlood);
    }
    #endregion

    #region 父类重写
    public override void Create()
    {
        EventManager.Register("ArrowRainRecord", ArrowRainRecord);
    }

    public override void GetHit()
    {
        if (CurrentTrackEntry.ToString() == "Skill1") return;
        ChangeAnim("attack", "Gethit1", false);
    }

    public override void GetHit(float hitnumber)
    {
        CurrentBlood -= hitnumber;
        currentBloodLine.CutBlood(hitnumber);
        CreateHitNumberObjcet(GetHitType.Normal, hitnumber);
        if (CurrentTrackEntry.ToString() == "Skill1" || specialAttackCoroutine != null) return;
        ChangeAnim("attack", "Gethit1", false);
    }

    public override void Attack()
    {
        Attack(0);
    }

    public override void Forward()
    {
        base.Forward();
        SetNormalAttackTimes();
        ChangeAnim("normal", "Forward", true);
    }

    public override void StandBy()
    {
        ChangeAnim("attack", "vertigo", true);
        base.StandBy();
    }

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        base.StartHandleEvent(trackEntry);
        switch (trackEntry.ToString())
        {
            case "Gethit1":
                if (createArrowCoroutine != null) StopCoroutine(createArrowCoroutine);
                break;
            default:
                CreateArrow(trackEntry.ToString());
                break;
        }
    }

    public override void CompleteHandleEvent(TrackEntry trackEntry)
    {
        if (debug)
            Debug.Log("英雄播放完成");
        switch (trackEntry.ToString())
        {
            case "attack2":
                switch(heroAttackMode)
                {
                    case HeroAttackMode.Special:
                        if (SpecialAttackTimes >= 5)
                        {
                            SpecialAttackTimes = 0;
                            specialAttackCustom.complete = true;
                        }
                        else
                        {
                            SpecialAttackTimes++;
                            PlayAnim("attack2");
                        }
                        break;
                    case HeroAttackMode.Normal:
                        RandomAttack();
                        break;
                }
                break;
            case "attack":
                RandomAttack();
                break;
            case "Skill1":
                RandomAttack();
                break;
            case "Gethit1":
                isStandy = false;
                RandomAttack();
                break;
            case "Standby":
                if (enemyDead) return;
                Attack(AttackType);
                break;
            case "Standby02":
                switch(heroAttackMode)
                {
                    case HeroAttackMode.Normal:
                        Attack(AttackType);
                        break;
                }
                break;
            case "Skill3":
                if (specialAttackCustom != null)
                {
                    EventManager.Send("HeroMagicStop");
                    specialAttackCustom.complete = true;
                }
                break;
        }
    }
    #endregion

    #region 携程
    /// <summary>
    /// 附魔连击
    /// </summary>
    /// <returns></returns>
    IEnumerator EnchantmentCombo()
    {
        //播放技能动画
        yield return PlaySkill3Anim();

        //播放攻击动画，播放十次后结束
        yield return PlaySkill3Attack();

        //回归普通攻击
        ResetAttackMode();
        RandomAttack();
    }

    /// <summary>
    /// 等待播放附魔动画
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitPlayMagicEffect()
    {
        //等待到达位置
        yield return new WaitForSeconds(0.4f);

        //展示附魔动画
        CurrentSkeleton.AnimationState.TimeScale = 0;
        AttackPos.SetBone("d_g");
        AttackPos.Initialize();
        EventManager.Send<float>("HeroMagicTime", 2.5f);
        EventManager.Send<bool>("HeroMagicSet", true);
        EventManager.Send<int>("HeroMagicMode", 0);
        EventManager.Send<Vector3>("HeroMagicPlay", AttackPos.transform.localPosition + AttackPos.transform.parent.localPosition);
        yield return new WaitForSeconds(2.0f);

        //恢复动作
        CurrentSkeleton.AnimationState.TimeScale = 1;
    }

    IEnumerator ArrowRain()
    {
        //播放前摇动作
        specialEffectCoroutine = StartCoroutine(ArrowRainStand());
        yield return specialEffectCoroutine;

        //等待
        StandBy(2);

        //剑雨生成
        rainArrowNumber = 0;
        if (arrowGroup[3] == null)
        {
            Debug.LogError("剑雨物体不存在");
            specialAttackCoroutine = null;
            StopSpecial();
            yield break;
        }
        float createX = DataBase.SCREEN_WIDTH * 0.25f;
        float createY = DataBase.SCREEN_HEIGHT / 2;
        for (int c = 0; c < 10; c++)
        {
            var rainArrow = GameObject.Instantiate(arrowGroup[3], transform.parent, false);
            rainArrow.transform.eulerAngles = new Vector3(0, 0, -90);
            rainArrow.transform.localPosition = new Vector3(createX + UnityEngine.Random.Range(-200, 200), createY, 0);
            rainArrow.SetTarget(TargetType.Enemy, false);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.5f));
        }

        //回归普通攻击
        while (true)
        {
            yield return null;
            if (rainArrowNumber == 10)
            {
                ResetAttackMode();
                RandomAttack();
                yield break;
            }
        }

    }

    /// <summary>
    /// 剑雨前摇
    /// </summary>
    /// <returns></returns>
    IEnumerator ArrowRainStand()
    {
        //进入待机
        StandBy(0);
        yield return new WaitForSeconds(0.3f);

        //播放魔法动画
        AttackPos.SetBone("z_g");
        AttackPos.Initialize();
        EventManager.Send<bool>("HeroMagicSet", true);
        EventManager.Send<float>("HeroMagicTime", 10.0f);
        EventManager.Send<int>("HeroMagicMode", 1);
        EventManager.Send<Vector3>("HeroMagicPlay", AttackPos.transform.localPosition + AttackPos.transform.parent.localPosition);
        yield return new WaitForSeconds(1.0f);

        //播放攻击动画
        PlayAnim("Skill4");

        //特效跟随
        float followTime = 0;
        AttackPos.SetBone("s_g");
        while (followTime < 0.15f)
        {
            yield return null;
            followTime += Time.deltaTime;
            AttackPos.Initialize();
            EventManager.Send<Vector3>("HeroMagicPos", AttackPos.transform.localPosition + AttackPos.transform.parent.localPosition);
        }
    }

    IEnumerator CareAttack()
    {
        //播放前摇动作
        PlayAnim("Skill1");

        //等待升空
        yield return new WaitForSeconds(0.6f);
        CurrentSkeleton.AnimationState.TimeScale = 0;

        //播放特效
        AttackPos.SetBone("s_g");
        AttackPos.Initialize();
        EventManager.Send<Vector3>("HeroCarePlay", AttackPos.transform.localPosition + AttackPos.transform.parent.localPosition);
        EventManager.Send<Vector3>("HeroAimPlay", Vector3.zero);
        yield return new WaitForSeconds(1.0f);

        //产生箭矢
        AttackPos.SetBone("s_g");
        AttackPos.Initialize();
        CurrentSkeleton.AnimationState.TimeScale = 1;
        CreateArrow(2, TargetType.Enemy);

        //回归普通攻击
        yield return new WaitForSeconds(0.5f);
        ResetAttackMode();
        RandomAttack();
    }
    #endregion

    #region 脚本独有
    /// <summary>
    /// 待机动画
    /// </summary>
    /// <param name="index">待机序号</param>
    private void StandBy(int index)
    {
        switch (index)
        {
            case 0:
                ChangeAnim("attack", "Standby02", false);
                break;
            case 1:
                ChangeAnim("attack", "Standby", false);
                break;
            case 2:
                ChangeAnim("attack", "Standby_up", true);
                break;
        }
    }

    /// <summary>
    /// 根据不同的攻击类型播放动画
    /// </summary>
    /// <param name="attackType">攻击序号</param>
    private void Attack(int attackType = 0)
    {
        string AnimationName = "attack";
        switch (attackType)
        {
            case 0:
                AnimationName = "attack";
                break;
            case 1:
                AnimationName = "attack2";
                break;
            case 2:
                AnimationName = "Skill1";
                break;
        }
        ChangeAnim("attack", AnimationName, false);
    }

    /// <summary>
    /// 改变动画
    /// </summary>
    /// <param name="dataType">动画数据类型</param>
    /// <param name="animName">动画名称</param>
    /// <param name="loop">是否循环</param>
    /// <param name="tarck">管道</param>
    private void ChangeAnim(string dataType, string animName, bool loop = false, int tarck = 0)
    {
        SkeletonDataAsset selectData = null;
        switch (dataType)
        {
            case "normal":
                selectData = NormalData;
                break;
            case "attack":
                selectData = AttackData;
                break;
        }
        if (selectData != null)
        {
            if (CurrentSkeleton.skeletonDataAsset != selectData)
            {
                if (debug)
                    Debug.Log($"{name} CurrentSkeleton.skeletonDataAsset != selectData");
                CurrentSkeleton.skeletonDataAsset = selectData;
                CurrentSkeleton.startingAnimation = animName;
                CurrentSkeleton.Initialize(true);
                CurrentSkeleton.Skeleton.SetToSetupPose();
            }
            else
            {
                CurrentSkeleton.Initialize(true);
            }
            switch (dataType)
            {
                case "normal":
                    AttackPos.gameObject.SetActive(false);
                    break;
                case "attack":
                    AttackPos.gameObject.SetActive(true);
                    break;
            }
            isAdd = false;
        }
        PlayAnim(animName, loop, tarck);
    }

    /// <summary>
    /// 产生箭矢
    /// </summary>
    /// <param name="animName">动画名称</param>
    private void CreateArrow(string animName)
    {
        if (debug)
            Debug.Log("产生箭矢" + animName);
        //AttackPos.SetBone("s_g");
        //AttackPos.Initialize();
        switch (animName)
        {
            case "attack2":
                AttackPos.SetBone("s_g");
                AttackPos.Initialize();
                switch (heroAttackMode)
                {
                    case HeroAttackMode.Special:
                        EventManager.Send<Vector3>("HeroWindMagic", AttackPos.transform.localPosition + AttackPos.transform.parent.localPosition);
                        CreateArrow(1, TargetType.Enemy);
                        break;
                    case HeroAttackMode.Normal:
                        CreateArrow(0, TargetType.Enemy);
                        break;
                }
                break;
            case "attack":
                AttackPos.SetBone("s_g");
                AttackPos.Initialize();
                CreateArrow(1, TargetType.Enemy);
                break;
            case "Skill1":
                //AttackPos.SetBone("s_g");
                //AttackPos.Initialize();
                //createArrowCoroutine = StartCoroutine(DelayCreate(0.7f, 2));
                break;
        }
    }

    public override void DissatisfyNormalAttackTimes()
    {
        NormalAttackTimes++;
        AttackType = UnityEngine.Random.Range(0, 2);
        switch (isStandy)
        {
            case true:
                StandBy(1);
                break;
            case false:
                Attack(AttackType);
                break;
        }
        isStandy = true;
    }

    public override void StartSpecial()
    {
        base.StartSpecial();
        switch (specialAttackModeGroup[specialAttackMode])
        {
            case 0:
                specialAttackCoroutine = StartCoroutine(EnchantmentCombo());
                break;
            case 1:
                specialAttackCoroutine = StartCoroutine(ArrowRain());
                break;
            case 2:
                specialAttackCoroutine = StartCoroutine(CareAttack());
                break;
        }
        specialAttackMode++;
    }

    public override void StopSpecial()
    {
        base.StopSpecial();
        AttackPos.SetBone("s_g");
        AttackPos.Initialize();
    }

    /// <summary>
    /// 播放技能3动画
    /// </summary>
    /// <returns></returns>
    private Custom PlaySkill3Anim()
    {
        specialAttackCustom = new Custom();
        SpecialAttackTimes = 0;
        PlayAnim("Skill3");
        specialEffectCoroutine = StartCoroutine(WaitPlayMagicEffect());
        return specialAttackCustom;
    }

    /// <summary>
    /// 播放技能3的攻击
    /// </summary>
    /// <returns></returns>
    private Custom PlaySkill3Attack()
    {
        specialAttackCustom = new Custom();
        SpecialAttackTimes = 0;
        CurrentSkeleton.AnimationState.TimeScale = 2;
        PlayAnim("attack2");
        return specialAttackCustom;
    }

    /// <summary>
    /// 记录剑雨销毁的数量
    /// </summary>
    private void ArrowRainRecord()
    {
        rainArrowNumber++;
        Debug.Log($"增加了剑雨记录次数:{rainArrowNumber}");
    }
    #endregion
}