using DG.Tweening;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldIconController : MonoBehaviour
{
    private void Start()
    {
        EventManager.Register("GoldScaleAnim", StartAnim);
    }

    /// <summary>
    /// 开启动画
    /// </summary>
    private void StartAnim()
    {
        AnimInit();
        transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.05f).SetLoops(2, LoopType.Yoyo);
    }

    private void AnimInit()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
    }
}
