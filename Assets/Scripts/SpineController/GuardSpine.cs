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
    /// 人物骨骼坐标
    /// </summary>
    public Transform SpineTransform;
    /// <summary>
    /// 增加血量提示
    /// </summary>
    public HitTextBase AddBloodObject;
    /// <summary>
    /// 当前攻击类型
    /// </summary>
    private int AttackType;
    /// <summary>
    /// 当前创建箭矢的携程
    /// </summary>
    private Coroutine createArrowCoroutine;
    /// <summary>
    /// 敌人是否死亡
    /// </summary>
    private bool enemyDead = false;
    /// <summary>
    /// 特殊攻击模式携程
    /// </summary>
    private Coroutine specialAttackCoroutine;
    /// <summary>
    /// 特殊攻击模式记录
    /// </summary>
    private Custom specialAttackCustom;
    /// <summary>
    /// 普通攻击次数
    /// </summary>
    private int NormalAttackTimes;
    /// <summary>
    /// 普通攻击目标次数
    /// </summary>
    private int NormalAttackTargetTimes;
    /// <summary>
    /// 特殊攻击次数
    /// </summary>
    private int SpecialAttackTimes = 0;
    /// <summary>
    /// 英雄攻击模式
    /// </summary>
    private HeroAttackMode heroAttackMode;
    #endregion

    #region Unity
    private void Start()
    {
        AttackPos.gameObject.SetActive(false);
        currentBloodLine.IniteBlood(CurrentBlood);
    }
    #endregion

    #region 父类重写
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
        enemyDead = false;
        SetNormalAttackTimes();
        ChangeAnim("normal", "Forward", true);
    }

    public override void StandBy()
    {
        StopSpecial();
        enemyDead = true;
        ChangeAnim("attack", "vertigo", true);
        EventManager.Send("HeroAddBlood");
        float lossBlood = MaxBlood - CurrentBlood;
        int addTimes = (int)Math.Floor(lossBlood / 100.0f);
        float Remainder = lossBlood % 100.0f;
        if (Remainder > 0)
        {
            addTimes++;
        }
        float addSpeed;
        if (addTimes < 3)
        {
            addSpeed = 1.0f;
        }
        else
        {
            addSpeed = 3.0f / addTimes;
        }
        CurrentBlood = MaxBlood;
        Debug.Log($"增加次数{addTimes}, 增加速度{addSpeed}, 减少血量{lossBlood}");
        StartCoroutine(AddBlood(addTimes, addSpeed));
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
                RandomAttack(false);
                break;
            case "Standby":
                if (enemyDead) return;
                Attack(AttackType);
                break;
            case "Standby02":
                Attack(AttackType);
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
    /// 回复血量添加动画 
    /// </summary>
    /// <param name="addTimes">添加次数</param>
    /// <param name="addSpeed">生成速度</param>
    /// <returns></returns>
    IEnumerator AddBlood(int addTimes, float addSpeed)
    {
        while(addTimes > 0)
        {
            yield return new WaitForSeconds(addSpeed);
            addTimes--;
            CreateAddBlood();
            currentBloodLine.AddBlood(100);
        }
    }

    /// <summary>
    /// 延时创建
    /// </summary>
    /// <param name="waitTime">等待时间</param>
    /// <param name="arrowType">箭矢类型</param>
    /// <returns></returns>
    IEnumerator DelayCreate(float waitTime, int arrowType)
    {
        yield return new WaitForSeconds(waitTime);
        CreateArrow(arrowType, TargetType.Enemy);
    }

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

    IEnumerator WaitPlayMagicEffect()
    {
        yield return new WaitForSeconds(0.2f);
        AttackPos.SetBone("d_g");
        AttackPos.Initialize();
        EventManager.Send<Vector3>("HeroMagicPlay", AttackPos.transform.localPosition + AttackPos.transform.parent.localPosition);
    }
    #endregion

    #region 脚本独有
    /// <summary>
    /// 生成添加血量提示物体
    /// </summary>
    private void CreateAddBlood()
    {
        HitTextBase gameObject = GameObject.Instantiate(AddBloodObject);
        gameObject.transform.SetParent(transform.parent);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        gameObject.SetText(100);
    }

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
                AttackPos.SetBone("s_g");
                AttackPos.Initialize();
                createArrowCoroutine = StartCoroutine(DelayCreate(0.7f, 2));
                break;
        }
    }

    /// <summary>
    /// 等待播放战斗动画
    /// </summary>
    private void RandomAttack(bool isStand = true)
    {
        if(NormalAttackTimes >= NormalAttackTargetTimes)
        {
            SetNormalAttackTimes();
            heroAttackMode = HeroAttackMode.Special;
            StartSpecial();
        }
        else
        {
            NormalAttackTimes++;
            AttackType = UnityEngine.Random.Range(0, 2);
            switch(isStand)
            {
                case true:
                    StandBy(1);
                    break;
                case false:
                    Attack(AttackType);
                    break;
            }
            //switch (AttackType)
            //{
            //    case 2:
            //        StandBy(0);
            //        break;
            //    default:
            //        break;
            //}
        }
    }

    private void StartSpecial()
    {
        specialAttackCoroutine = StartCoroutine(EnchantmentCombo());
    }

    private void StopSpecial()
    {
        if (specialAttackCoroutine != null) StopCoroutine(specialAttackCoroutine);
        AttackPos.SetBone("s_g");
        AttackPos.Initialize();
        ResetAttackMode();
    }

    private void ResetAttackMode()
    {
        specialAttackCustom = null;
        CurrentSkeleton.AnimationState.TimeScale = 1;
        heroAttackMode = HeroAttackMode.Normal;
    }

    /// <summary>
    /// 初始化普通攻击次数
    /// </summary>
    private void SetNormalAttackTimes()
    {
        NormalAttackTargetTimes = UnityEngine.Random.Range(3, 5);
        NormalAttackTimes = 0;
        heroAttackMode = HeroAttackMode.Normal;
    }

    /// <summary>
    /// 播放技能3东湖
    /// </summary>
    /// <returns></returns>
    private Custom PlaySkill3Anim()
    {
        specialAttackCustom = new Custom();
        PlayAnim("Skill3");
        StartCoroutine(WaitPlayMagicEffect());
        return specialAttackCustom;
    }

    private Custom PlaySkill3Attack()
    {
        specialAttackCustom = new Custom();
        CurrentSkeleton.AnimationState.TimeScale = 2;
        PlayAnim("attack2");
        StartCoroutine(WaitPlayMagicEffect());
        return specialAttackCustom;
    }
    #endregion

}