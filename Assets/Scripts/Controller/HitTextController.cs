using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HitTextType
{
    Warning,
    Add,
    CriticalStricke,
    Normal
}

public class HitTextController : MonoBehaviour
{
    /// <summary>
    /// 文字显示
    /// </summary>
    public Text showText;

    [Space]

    /// <summary>
    /// 移动的最大时间
    /// </summary>
    public float maxTime;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// 提示类型
    /// </summary>
    public HitTextType hitTextType;

    /// <summary>
    /// 当前移动的时间
    /// </summary>
    [NonSerialized]
    public float currentTime;

    /// <summary>
    /// 移动携程
    /// </summary>
    private Coroutine moveCoroutine;

    private void Start()
    {
        StartMove();
    }

    public virtual void SetText(float number)
    {
        string specialStr = "";
        switch(hitTextType)
        {
            case HitTextType.Add:
                specialStr = "+";
                break;
            case HitTextType.CriticalStricke:
                specialStr = "爆";
                break;
            case HitTextType.Normal:
                specialStr = "-";
                break;
        }
        showText.text = specialStr + int.Parse(number.ToString());
    }

    private void StartMove()
    {
        StopMove();
        gameObject.SetActive(true);
        switch (hitTextType)
        {
            case HitTextType.Add:
                moveCoroutine = StartCoroutine(AddBloodMove());
                break;
            case HitTextType.CriticalStricke:
                moveCoroutine = StartCoroutine(CriticalStrikeMove());
                break;
            case HitTextType.Normal:
                moveCoroutine = StartCoroutine(NormalMove());
                break;
            case HitTextType.Warning:
                moveCoroutine = StartCoroutine(WariningMove());
                break;
        }
    }

    private void StopMove()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        gameObject.SetActive(false);
    }

    IEnumerator NormalMove()
    {
        //移动
        float distance = moveSpeed * maxTime;
        transform.DOLocalMoveY(distance, maxTime);
        //等待
        yield return new WaitForSeconds(0.5f);
        //删除
        Destroy(gameObject);
    }

    IEnumerator CriticalStrikeMove()
    {
        //放大
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }

    IEnumerator AddBloodMove()
    {
        //移动
        float distance = moveSpeed * maxTime;
        transform.DOLocalMoveY(distance, maxTime);
        //等待
        yield return new WaitForSeconds(0.5f);
        //删除
        Destroy(gameObject);
    }

    IEnumerator WariningMove()
    {
        transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f).SetLoops(4, LoopType.Yoyo);
        
        yield return new WaitForSeconds(2.0f);

        Destroy(gameObject);
    }
}
