using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpineEffect : FightEffect
{
    public string animationName = "open";
    public bool endRevertAnim;//结束时是否反向播放动画

    SkeletonAnimation _animation;
    SkeletonAnimation Animation
    {
        get
        {
            if (_animation == null)
            {
                _animation = GetComponentInChildren<SkeletonAnimation>();
            }
            return _animation;
        }
    }

    TrackEntry entry;

    public override float GetDuration()
    {
        var anima = Animation.state.Data.skeletonData.FindAnimation(animationName);
        if (anima == null)
            throw new Exception(name + "不存在动画:" + animationName);
        return anima.Duration;
    }

    [ContextMenu("MyStart")]
    public override void MyStart()
    {
        base.MyStart();
        var entry = Animation.state.SetAnimation(0, animationName, false);
        entry.Complete -= OnComplate;
        entry.Complete += OnComplate;
    }

    [ContextMenu("MyStartInverted")]
    public override void MyStartInverted()
    {
        base.MyStart();
        this.effectInverted = true;
        var anima = Animation.state.Data.skeletonData.FindAnimation(animationName);
        entry = Animation.state.SetAnimation(0, anima, false);
        entry.timeScale = -1;
        entry.TrackTime = anima.duration;
        entry.Complete -= OnComplate;
        entry.Complete += OnComplate;
    }

    public override void SetOrderLayer(int orderLayer)
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.sortingOrder = orderLayer;
        }
    }

    public override void Update()
    {
        base.Update();
        if (effectActive && effectInverted && entry != null && !endRevertAnim)
        {
            if (entry.trackTime - Time.deltaTime * 2 < 0)
            {
                entry.timeScale = 0;
                entry.trackTime = 0;
                OnComplate(entry);
            }
        }
    }

    void OnComplate(TrackEntry trackEntry)
    {
        trackEntry.Complete -= OnComplate;
        if(!endRevertAnim)
            MyStop();
    }


    public override void MyStop()
    {
        base.MyStop();
        if (endRevertAnim)
        {
            StartCoroutine(EndTask());
        }
        else
        {
            if (!effectInverted)
                Animation.state.ClearTracks();
            if (Application.isPlaying)
            {
                if (OnMyDestroy != null)
                    OnMyDestroy(this);
            }
        }
    }

    IEnumerator EndTask()
    {
        var anima = Animation.state.Data.skeletonData.FindAnimation(animationName);
        entry = Animation.state.SetAnimation(0, anima, false);
        entry.timeScale = -1;
        entry.TrackTime = anima.duration;
        float allTime = anima.duration;
        while (allTime > 0)
        {
            yield return null;
            allTime -= Time.deltaTime;
        }
        //结束
        if (!effectInverted)
            Animation.state.ClearTracks();
        if (Application.isPlaying)
        {
            if (OnMyDestroy != null)
                OnMyDestroy(this);
        }
    }
}