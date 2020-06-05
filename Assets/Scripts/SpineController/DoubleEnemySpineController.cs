using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleEnemySpineController : EnemySpine
{
    public EnemySpine[] childSpine;

    private List<Custom> childDeadCustom;

    public override void Create()
    {
        foreach(EnemySpine child in childSpine)
        {
            child.Create();
        }
    }

    public override void Attack()
    {
        foreach (EnemySpine child in childSpine)
        {
            child.Attack();
        }
    }

    /// <summary>
    /// 获取记录敌人死亡的Custom
    /// </summary>
    /// <param name="deadCustom">记录用的Custom</param>
    public override void GetDeadCustom(Custom deadCustom)
    {
        childDeadCustom = new List<Custom>(childSpine.Length);
        for (int i = 0; i < childSpine.Length; i++)
        {
            EnemySpine child = childSpine[i];
            Custom childCustom = new Custom();
            child.GetDeadCustom(childCustom);
            childDeadCustom.Add(childCustom);
        }
        this.deadCustom = deadCustom;
        StartCoroutine(GetChildDeadCustom());
    }

    IEnumerator GetChildDeadCustom()
    {
        while(true)
        {
            yield return null;

            bool isComplete = true;
            for(int i = 0; i < childDeadCustom.Count; i++)
            {
                if (!childDeadCustom[i].complete)
                {
                    isComplete = false;
                    break;
                }
            }

            if (isComplete)
            {
                deadCustom.complete = true;
                yield break;
            }
        }
    }
}
