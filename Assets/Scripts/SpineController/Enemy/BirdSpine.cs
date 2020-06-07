using Spine;
using System.Collections;
using UnityEngine;

public enum BirdLevel
{
    Normal,
    Elite,
    Boss
}

public enum BirdAttackMode
{
    Normal,
    Special
}

public enum SpecialAttackMode
{
    Combo
}

public class BirdSpine : EnemySpine
{
    [Space]
    public BirdLevel birdLevel; 
    /// <summary>
    /// 是否已经死亡
    /// </summary>
    private bool IsDead = false;
    /// <summary>
    /// 最大血量
    /// </summary>
    private float maxBlood;
    /// <summary>
    /// 普通攻击次数
    /// </summary>
    private int normalAttackNumber = 0;
    /// <summary>
    /// 当前鸟类攻击模式
    /// </summary>
    private BirdAttackMode birdAttackMode = BirdAttackMode.Normal;
    /// <summary>
    /// 特殊攻击携程
    /// </summary>
    private Coroutine specialAttackCorotine;
    /// <summary>
    /// 特殊攻击监听
    /// </summary>
    private Custom specialAttackCustom;
    /// <summary>
    /// 特殊攻击次数
    /// </summary>
    private int specialAttackTimes;

    private SpecialAttackMode specialAttackMode;

    public override void Create()
    {
        StandBy();
        currentBloodLine.IniteBlood(CurrentBlood);
        maxBlood = CurrentBlood;
    }

    public override void Dead()
    {
        base.Dead();
        PlayAnim("Gethit1");
        IsDead = true;
        CloseBox();
        StopSpecialAttack();
        StartCoroutine(DeadForWait());
    }

    public override void GetHit()
    {
        if (specialAttackCorotine != null)
            return;
        PlayAnim("Gethit1");
    }

    public override void StandBy()
    {
        StandBy(false);
    }

    private void StandBy(bool loop)
    {
        PlayAnim("Standby", loop, 0);
    }

    public override void Attack()
    {
        DelayAttack(0.5f);
    }

    /// <summary>
    /// 带延时的攻击
    /// </summary>
    /// <param name="delay">延时时间</param>
    private void DelayAttack(float delay, int arrowType = 0)
    {
        PlayAnim("attack");
        DelayCreate(delay, arrowType);
    }

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        base.StartHandleEvent(trackEntry);
        switch (trackEntry.ToString())
        {
            case "Gethit1":
                StopCreate();
                break;
        }
    }

    public override void CompleteHandleEvent(TrackEntry trackEntry)
    {
        switch (trackEntry.ToString())
        {
            case "attack":
                switch(birdLevel)
                {
                    case BirdLevel.Normal:
                        StandBy(false);
                        break;
                    case BirdLevel.Elite:
                        switch(birdAttackMode)
                        {
                            case BirdAttackMode.Normal:
                                StandBy(false);
                                break;
                            case BirdAttackMode.Special:
                                specialAttackTimes++;
                                Debug.Log($"特殊攻击数量：{specialAttackTimes}");
                                if (specialAttackTimes == 2)
                                {
                                    specialAttackCustom.complete = true;
                                }
                                else
                                {
                                    DelayAttack(0.25f);
                                }
                                break;
                        }
                        break;
                    case BirdLevel.Boss:
                        switch (birdAttackMode)
                        {
                            case BirdAttackMode.Normal:
                                StandBy(false);
                                break;
                            case BirdAttackMode.Special:
                                specialAttackTimes++;
                                Debug.Log($"特殊攻击数量：{specialAttackTimes}");
                                if (specialAttackTimes == 2)
                                {
                                    specialAttackCustom.complete = true;
                                }
                                else
                                {
                                    DelayAttack(0.25f);
                                }
                                break;
                        }
                        break;
                }
                break;
            case "Gethit1":
                WaitAttack();
                break;
            case "Standby":
                WaitAttack();
                break;
        }
    }

    private void WaitAttack()
    {
        switch(birdLevel)
        {
            case BirdLevel.Normal:
                Attack();
                break;
            case BirdLevel.Elite:
                EliteAttack();
                break;
            case BirdLevel.Boss:
                EliteAttack();
                break;
        }
    }

    /// <summary>
    /// 停止特殊攻击
    /// </summary>
    private void StopSpecialAttack()
    {
        if (specialAttackCorotine == null)
            return;
        StopCoroutine(specialAttackCorotine);
        switch(specialAttackMode)
        {
            case SpecialAttackMode.Combo:
                StopCreate();
                break;
        }
    }

    /// <summary>
    /// 精英攻击
    /// </summary>
    private void EliteAttack()
    {
        normalAttackNumber++;
        if (normalAttackNumber > 1)
        {
            normalAttackNumber = 0;
            birdAttackMode = BirdAttackMode.Special;
            DoubleCombo();
        }
        else
        {
            Attack();
        }
    }

    private void DoubleCombo()
    {
        Debug.Log("开启连击");
        specialAttackMode = SpecialAttackMode.Combo;
        specialAttackCorotine = StartCoroutine(DoubleComboTask());
    }

    IEnumerator DoubleComboTask()
    {
        //播放攻击动作
        CurrentSkeleton.AnimationState.TimeScale = 2;
        DelayAttack(0.25f);

        //记录动画
        specialAttackCustom = new Custom();
        yield return specialAttackCustom;

        //回归正常
        birdAttackMode = BirdAttackMode.Normal;
        CurrentSkeleton.AnimationState.TimeScale = 1;
        specialAttackTimes = 0;
        StandBy(false);
        specialAttackCorotine = null;
    }

    IEnumerator DeadForWait()
    {
        yield return new WaitForSeconds(0.5f);
        CurrentSkeleton.AnimationState.TimeScale = 0;
        
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }
}
