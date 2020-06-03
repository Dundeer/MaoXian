using DG.Tweening;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldIconController : MonoBehaviour
{
    /// <summary>
    /// 动画携程
    /// </summary>
    private Coroutine AnimCoroutine;

    private void Start()
    {
        EventManager.Register("GoldScaleAnim", StartAnim);
    }

    /// <summary>
    /// 开启动画携程
    /// </summary>
    private void StartAnim()
    {
        if (AnimCoroutine != null) return;
        AnimCoroutine = StartCoroutine(ScaleAnim());
    }

    /// <summary>
    /// 动画携程
    /// </summary>
    /// <returns></returns>
    IEnumerator ScaleAnim()
    {
        Tweener tweener = transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.05f);
        tweener.SetLoops(2, LoopType.Yoyo);
        yield return new WaitForSeconds(0.1f);
        AnimCoroutine = null;
    }
}
