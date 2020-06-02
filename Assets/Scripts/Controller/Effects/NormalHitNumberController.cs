using DG.Tweening;
using System.Collections;
using UnityEngine;

public class NormalHitNumberController : HitTextBase
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

    
}
