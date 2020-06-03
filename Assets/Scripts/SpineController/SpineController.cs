using Spine;
using Spine.Unity;
using System;
using UnityEngine;

public class SpineController : MonoBehaviour
{
    public bool debug = false;

    /// <summary>
    /// 当前挂在的骨骼动画控件
    /// </summary>
    [NonSerialized]
    public SkeletonGraphic CurrentSkeleton = null;
    /// <summary>
    /// 当前播放动画的记录
    /// </summary>
    [NonSerialized]
    public TrackEntry CurrentTrackEntry = null;
    /// <summary>
    /// 敌人血量
    /// </summary>
    public float CurrentBlood = 100;
    /// <summary>
    /// 攻击位置
    /// </summary>
    public BoneFollowerGraphic AttackPos;
    /// <summary>
    /// 可以使用的箭
    /// </summary>
    public ArrowController[] arrowGroup;
    /// <summary>
    /// 当前血条
    /// </summary>
    public BloodLine currentBloodLine;
    /// <summary>
    /// 普通受击文字
    /// </summary>
    public HitTextBase noramlHitObject;
    /// <summary>
    /// 暴击文字
    /// </summary>
    public HitTextBase criticalStrikeObject;
    /// <summary>
    /// 普通受击的特效
    /// </summary>
    public GameObject NormalEffect;
    /// <summary>
    /// 添加事件
    /// </summary>
    [NonSerialized]
    public bool isAdd = false;
    /// <summary>
    /// 最大血量
    /// </summary>
    [NonSerialized]
    public float MaxBlood;

    private void Awake()
    {
        CurrentSkeleton = transform.GetComponentInChildren<SkeletonGraphic>();
        MaxBlood = CurrentBlood;
    }

    #region SpineEvent
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animName">动画名</param>
    public void PlayAnim(string animName, bool loop = false, int tarck = 0)
    {

        CurrentSkeleton.AnimationState.ClearTracks();
        CurrentTrackEntry = CurrentSkeleton.AnimationState.SetAnimation(tarck, animName, loop);

        StartHandleEvent(CurrentTrackEntry);
        CurrentTrackEntry.Complete += CompleteHandleEvent;
    }

    /// <summary>
    /// 开始事件监听
    /// </summary>
    public virtual void StartHandleEvent(TrackEntry trackEntry)
    {

    }

    /// <summary>
    /// 完成事件监听
    /// </summary>
    public virtual void CompleteHandleEvent(TrackEntry trackEntry)
    {
               
    }
    #endregion

    #region CommonFunc
    /// <summary>
    /// 攻击
    /// </summary>
    public virtual void Attack()
    {

    }

    /// <summary>
    /// 受击
    /// </summary>
    public virtual void GetHit()
    {

    }

    /// <summary>
    /// 受击
    /// </summary>
    public virtual void GetHit(float hitnumber)
    {

    }

    public virtual void StandBy()
    {

    }

    /// <summary>
    /// 创建箭矢
    /// </summary>
    /// <param name="index">箭矢序号</param>
    /// <param name="targetType">目标类型</param>
    public virtual void CreateArrow(int index, TargetType targetType)
    {
        ArrowController createArrow = GameObject.Instantiate(arrowGroup[index]);
        createArrow.transform.SetParent(transform.parent);
        createArrow.transform.localScale = new Vector3(1, 1, 1);
        createArrow.transform.position = AttackPos.transform.position;
        createArrow.SetTarget(targetType);
    }

    /// <summary>
    /// 创建受击数字
    /// </summary>
    /// <param name="hitType">受击类型</param>
    /// <param name="number">伤害数字</param>
    public void CreateHitNumberObjcet(GetHitType hitType, float number)
    {
        HitTextBase gameObject = null;
        switch (hitType)
        {
            case GetHitType.Normal:
                gameObject = GameObject.Instantiate(noramlHitObject);
                gameObject.transform.SetParent(transform.parent);
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                gameObject.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
                break;
            case GetHitType.CriticalStrike:
                gameObject = GameObject.Instantiate(criticalStrikeObject);
                gameObject.transform.SetParent(transform.parent);
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                gameObject.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + 200);
                break;
        }
        if (gameObject == null) return;
        gameObject.SetText(number);
    }
    #endregion
}
