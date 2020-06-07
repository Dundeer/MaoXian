using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleEnemySpineController : EnemySpine
{
    public EnemySpine[] childSpine;

    private List<Custom> childDeadCustom;

    public override void Create()
    {
        foreach (EnemySpine child in childSpine)
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
}
