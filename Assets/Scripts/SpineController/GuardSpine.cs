using Spine;
using Spine.Unity;
using System.Collections;
using UnityEngine;

public class GuardSpine : HeroSpine
{
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

    private void Start()
    {
        AttackPos.gameObject.SetActive(false);
    }

    public override void GetHit()
    {
        ChangeAnim("attack", "Gethit1", false);
    }

    public override void Attack()
    {
        Attack(0);
    }

    /// <summary>
    /// 根据不同的攻击类型播放动画
    /// </summary>
    /// <param name="attackType">攻击序号</param>
    private void Attack(int attackType = 0)
    {
        string AnimationName = "attack";
        switch(attackType)
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
            case 4:
                AnimationName = "attack";
                break;
        }
        ChangeAnim("attack", AnimationName, false);
    }

    public override void Forward()
    {
        enemyDead = false;
        ChangeAnim("normal", "Forward", true);
    }

    public override void StandBy()
    {
        enemyDead = true;
        StandBy(1);
    }

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

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        base.StartHandleEvent(trackEntry);
        CreateArrow(trackEntry.ToString());
    }

    /// <summary>
    /// 产生箭矢
    /// </summary>
    /// <param name="animName">动画名称</param>
    private void CreateArrow(string animName)
    {
        if (debug)
            Debug.Log("产生箭矢" + animName);
        switch (animName)
        {
            case "attack2":
                AttackPos.SetBone("z_g");
                AttackPos.Initialize();
                CreateArrow(0, TargetType.Enemy);
                break;
            case "attack":
                AttackPos.SetBone("s_g");
                AttackPos.Initialize();
                CreateArrow(1, TargetType.Enemy);
                break;
            case "Skill1":
                AttackPos.SetBone("d_g");
                AttackPos.Initialize();
                createArrowCoroutine = StartCoroutine(DelayCreate(0.7f, 2));
                break;
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

    public override void CompleteHandleEvent(TrackEntry trackEntry)
    {
        if (debug)
            Debug.Log("英雄播放完成");
        switch (trackEntry.ToString())
        {
            case "attack2":
                if (AttackType == 3)
                {
                    AttackType = 0;
                    Attack(AttackType);
                }
                else
                {
                    RandomAttack();
                }
                break;
            case "attack":
                RandomAttack();
                break;
            case "Skill1":
                RandomAttack();
                break;
            case "Gethit1":
                if (createArrowCoroutine != null) StopCoroutine(createArrowCoroutine);
                AttackType = Random.Range(0, 4);
                Attack(AttackType);
                break;
            case "Standby":
                if (enemyDead) return;
                Attack(AttackType);
                break;
            case "Standby02":
                Attack(AttackType);
                break;
        }
    }

    /// <summary>
    /// 等待播放战斗动画
    /// </summary>
    private void RandomAttack()
    {
        AttackType = Random.Range(0, 4);
        switch(AttackType)
        {
            case 2:
                StandBy(0);
                break;
            default:
                StandBy(1);
                break;
        }
        
    }

}
