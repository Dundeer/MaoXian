using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CriticalStrikeController : HitTextBase
{
    public override IEnumerator Move()
    {
        //放大
        transform.localScale = new Vector3(0, 0, 0);
        transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }

    public override void SetText(float number)
    {
        showText.text = "爆" + int.Parse(number.ToString());
    }
}
