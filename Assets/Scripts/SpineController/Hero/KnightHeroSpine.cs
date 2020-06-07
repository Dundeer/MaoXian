using QFramework;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightHeroSpine : HeroSpine
{
    private Coroutine delayCreateCorotine;

    public override void Create()
    {
        
    }

    public override void GetHit()
    {
        PlayAnim("Gethit1");
    }

    public override void GetHit(float hitnumber)
    {
        PlayAnim("Gethit1");
        CurrentBlood -= hitnumber;
        currentBloodLine.CutBlood(hitnumber);
        CreateHitNumberObjcet(GetHitType.Normal, hitnumber);
    }

    public override void Attack()
    {
        PlayAnim("attack");
        StartDelayCreateArrow(0.5f, 0);
    }

    public override void Forward()
    {
        base.Forward();
        PlayAnim("Forward", true);
    }

    public override void StandBy()
    {
        StopDelayCreateArrow();
        PlayAnim("Standby", true);
        base.StandBy();
    }

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        base.StartHandleEvent(trackEntry);
        switch(trackEntry.ToString())
        {
            case "attack":
                //CreateArrow(0, TargetType.Enemy);
                break;
        }
    }

    public override void CompleteHandleEvent(TrackEntry trackEntry)
    {
        if (debug)
            Debug.Log("英雄播放完成");
        switch (trackEntry.ToString())
        {
            case "attack":
                switch(heroAttackMode)
                {
                    case HeroAttackMode.Normal:
                        PlayAnim("Standby");
                        break;
                    case HeroAttackMode.Special:
                        
                        break;
                }
                break;
            case "Standby":
                if (enemyDead) return;
                RandomAttack();
                break;
            case "Skill2":
                CurrentSkeleton.AnimationState.TimeScale = 2;
                DoubleComboAttack();
                break;
            case "Skill1":
                SpecialAttackTimes++;
                switch (SpecialAttackTimes)
                {
                    case 3:
                        ResetAttackMode();
                        PlayAnim("Standby");
                        break;
                    default:
                        DoubleComboAttack();
                        break;
                }
                break;
        }
    }

    private void StartDelayCreateArrow(float waitTime, int arrowType)
    {
        delayCreateCorotine = StartCoroutine(DelayCreate(waitTime, arrowType));
    }

    private void StopDelayCreateArrow()
    {
        if (delayCreateCorotine != null)
            StopCoroutine(delayCreateCorotine);
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
        AttackPos.SetBone("z_weapon");
        AttackPos.Initialize();
        CreateArrow(arrowType, TargetType.Enemy);
    }

    public override void DissatisfyNormalAttackTimes()
    {
        NormalAttackTimes++;
        Attack();
    }

    public override void StartSpecial()
    {
        base.StartSpecial();
        StartDoubleCombo();
    }

    private void StartDoubleCombo()
    {
        PlayAnim("Skill2");
    }

    private void DoubleComboAttack()
    {
        PlayAnim("Skill1");
        StartDelayCreateArrow(0.25f, 1);
    }
}
