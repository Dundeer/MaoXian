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
    /// 攻击位置
    /// </summary>
    public BoneFollowerGraphic AttackPos;
    /// <summary>
    /// 可以使用的箭
    /// </summary>
    public ArrowController[] arrowGroup;
    /// <summary>
    /// 添加事件
    /// </summary>
    [NonSerialized]
    public bool isAdd = false;

    private void Awake()
    {
        CurrentSkeleton = transform.GetComponentInChildren<SkeletonGraphic>();
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
    #endregion
}
