using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AddBloodText : HitTextBase
{
    public override IEnumerator Move()
    {
        //移动
        float distance = moveSpeed * maxTime;
        transform.DOLocalMoveY(distance, maxTime);
        //等待
        yield return new WaitForSeconds(0.5f);
        //删除
        Destroy(gameObject);
    }

    public override void SetText(float number)
    {
        showText.text = "+" + int.Parse(number.ToString());
    }
}
