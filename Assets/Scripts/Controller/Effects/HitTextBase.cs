using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitTextBase : MonoBehaviour
{
    /// <summary>
    /// 文字显示
    /// </summary>
    public Text showText;
    /// <summary>
    /// 移动的最大时间
    /// </summary>
    public float maxTime;
    /// <summary>
    /// 移动速度
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// 当前移动的时间
    /// </summary>
    [NonSerialized]
    public float currentTime;

    private void Start()
    {
        StartCoroutine(Move());
    }

    public virtual IEnumerator Move()
    {
        yield return null;
    }

    public virtual void SetText(float number)
    {
        showText.text = "-" + int.Parse(number.ToString());
    }
}
