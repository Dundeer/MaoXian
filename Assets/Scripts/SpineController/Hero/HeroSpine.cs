using Spine;
using System;
using System.Collections;
using UnityEngine;
using QFramework;

public class HeroSpine : SpineController
{
    /// <summary>
    /// 敌人是否死亡
    /// </summary>
    [NonSerialized]
    public bool enemyDead = false;
    /// <summary>
    /// 增加血量提示
    /// </summary>
    public HitTextController AddBloodObject;
    /// <summary>
    /// 特殊攻击套路1
    /// </summary>
    public int[] specialAttackModeGroup1;
    /// <summary>
    /// 特殊攻击模式携程
    /// </summary>
    [NonSerialized]
    public Coroutine specialAttackCoroutine;
    /// <summary>
    /// 特殊攻击特效携程
    /// </summary>
    [NonSerialized]
    public Coroutine specialEffectCoroutine;
    /// <summary>
    /// 特殊攻击模式记录
    /// </summary>
    [NonSerialized]
    public Custom specialAttackCustom;
    /// <summary>
    /// 普通攻击次数
    /// </summary>
    [NonSerialized]
    public int NormalAttackTimes;
    /// <summary>
    /// 普通攻击目标次数
    /// </summary>
    [NonSerialized]
    public int NormalAttackTargetTimes;
    /// <summary>
    /// 特殊攻击次数
    /// </summary>
    [NonSerialized]
    public int SpecialAttackTimes = 0;
    /// <summary>
    /// 英雄攻击模式
    /// </summary>
    [NonSerialized]
    public HeroAttackMode heroAttackMode;
    /// <summary>
    /// 特殊进攻模式进程
    /// </summary>
    [NonSerialized]
    public int specialAttackMode = 0;
    /// <summary>
    /// 当前特殊攻击套路
    /// </summary>
    [NonSerialized]
    public int[] specialAttackModeGroup;

    /// <summary>
    /// 前进
    /// </summary>
    public virtual void Forward()
    {
        enemyDead = false;
    }

    public override void StandBy()
    {
        StopSpecial();
        enemyDead = true;
        float KnightHeroHeight = ((RectTransform)transform).rect.height;
        EventManager.Send<Vector3>("HeroAddBloodPlay", new Vector3(0, -KnightHeroHeight / 2, 0));
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
        StartAddBlood(addTimes, addSpeed);
    }

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        if (debug)
            Debug.Log("英雄开始动画" + trackEntry);
    }

    /// <summary>
    /// 生成添加血量提示物体
    /// </summary>
    private void CreateAddBlood()
    {
        HitTextController gameObject = GameObject.Instantiate(AddBloodObject);
        gameObject.transform.SetParent(transform.parent);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
        gameObject.SetText(100);
    }

    /// <summary>
    /// 回复血量添加动画 
    /// </summary>
    /// <param name="addTimes">添加次数</param>
    /// <param name="addSpeed">生成速度</param>
    /// <returns></returns>
    IEnumerator AddBlood(int addTimes, float addSpeed)
    {
        while (addTimes > 0)
        {
            yield return new WaitForSeconds(addSpeed);
            addTimes--;
            CreateAddBlood();
            currentBloodLine.AddBlood(100);
        }
    }

    public void StartAddBlood(int addTimes, float addSpeed)
    {
        StartCoroutine(AddBlood(addTimes, addSpeed));
    }

    /// <summary>
    /// 开启特殊攻击
    /// </summary>
    public virtual void StartSpecial()
    {
        if (specialAttackModeGroup == null || specialAttackMode == specialAttackModeGroup.Length)
        {
            specialAttackMode = 0;
            specialAttackModeGroup = specialAttackModeGroup1;
        }
        if (debug)
            Debug.Log($"当前特殊攻击类型{specialAttackMode}");
    }

    /// <summary>
    /// 停止特殊攻击
    /// </summary>
    public virtual void StopSpecial()
    {
        if (specialAttackCoroutine != null) StopCoroutine(specialAttackCoroutine);
        if (specialEffectCoroutine != null) StopCoroutine(specialEffectCoroutine);
        ResetAttackMode();
        specialAttackMode = 0;
        EventManager.Send("EffectsAllStop");
    }

    /// <summary>
    /// 重置攻击模式
    /// </summary>
    public void ResetAttackMode()
    {
        specialAttackCustom = null;
        CurrentSkeleton.AnimationState.TimeScale = 1;
        heroAttackMode = HeroAttackMode.Normal;
        SpecialAttackTimes = 0;
    }

    /// <summary>
    /// 初始化普通攻击次数
    /// </summary>
    public void SetNormalAttackTimes()
    {
        NormalAttackTargetTimes = UnityEngine.Random.Range(1, 3);
        NormalAttackTimes = 0;
        heroAttackMode = HeroAttackMode.Normal;
    }

    /// <summary>
    /// 等待播放战斗动画
    /// </summary>
    public void RandomAttack()
    {
        if (NormalAttackTimes >= NormalAttackTargetTimes)
        {
            SetNormalAttackTimes();
            heroAttackMode = HeroAttackMode.Special;
            StartSpecial();
        }
        else
        {
            DissatisfyNormalAttackTimes();
        }
    }

    /// <summary>
    /// 满足普通攻击次数
    /// </summary>
    public virtual void DissatisfyNormalAttackTimes()
    {
        
    }
}
