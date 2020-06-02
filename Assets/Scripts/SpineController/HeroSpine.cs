using Spine;
using UnityEngine;

public class HeroSpine : SpineController
{ 
    public virtual void Forward()
    {

    }

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        if (debug)
            Debug.Log("英雄开始动画" + trackEntry);
    }

}
