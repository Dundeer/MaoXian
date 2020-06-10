using QFramework;
using System.Collections;
using UnityEngine;

public enum ArrowType
{
    /// <summary>
    /// 普通
    /// </summary>
    normal,
    /// <summary>
    /// 快速
    /// </summary>
    speed,
    /// <summary>
    /// 重量级
    /// </summary>
    heavy,
    /// <summary>
    /// 剑雨
    /// </summary>
    rain
}

/// <summary>
/// 目标类型
/// </summary>
public enum TargetType
{
    /// <summary>
    /// 英雄
    /// </summary>
    Hero,
    /// <summary>
    /// 敌人
    /// </summary>
    Enemy
}

public class ArrowController : MonoBehaviour
{
    public ArrowType arrowType;         // 箭矢类型

    [Space]
    public float MoveSpeed;             // 箭的移动速度
    public int AttackBlood;             // 造成伤害

    /// <summary>
    /// 目标标签
    /// </summary>
    private TargetType currentTarget;
    /// <summary>
    /// 只读的目标类型
    /// </summary>
    public TargetType Target
    {
        get
        {
            return currentTarget;
        }
    }

    /// <summary>
    /// 存活时间
    /// </summary>
    private float liveTime;
    /// <summary>
    /// 是否已经攻击过
    /// </summary>
    private bool isAttacked = false;

    /// <summary>
    /// 箭的移动
    /// </summary>
    public void ArrowMove()
    {
        StartCoroutine(Move());
    }

    /// <summary>
    /// 设置目标类型
    /// </summary>
    /// <param name="type"></param>
    public void SetTarget(TargetType type, bool setEuler = true)
    {
        currentTarget = type;
        if (!setEuler) return;
        switch(type)
        {
            case TargetType.Enemy:
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case TargetType.Hero:
                transform.eulerAngles = new Vector3(0, 0, 180);
                break;
        }
    }

    /// <summary>
    /// 移动方法
    /// </summary>
    /// <returns></returns>
    IEnumerator Move()
    {
        while (true)
        {
            yield return null;
            transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime);
            liveTime += Time.deltaTime;
            if (liveTime >= 3.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collistionObject = collision.gameObject;
        switch (collistionObject.tag)
        {
            case "Hero":
                if (currentTarget == TargetType.Hero)
                {
                    HeroSpine heroSpine = collistionObject.GetComponent<HeroSpine>();
                    heroSpine.GetHit(AttackBlood);
                    Destroy(gameObject);
                }
                break;
            case "Enemy":
                if (currentTarget == TargetType.Enemy)
                {
                    if (isAttacked) return;
                    EnemySpine enemySpine = collistionObject.GetComponent<EnemySpine>();
                    enemySpine.GetHit(AttackBlood);
                    switch (arrowType)
                    {
                        case ArrowType.rain:
                            if (!isAttacked)
                                isAttacked = true;
                            break;
                        default:
                            Destroy(gameObject);
                            break;
                    }
                }
                break;
            case "Arrow":
                ArrowController otherArrow = collistionObject.GetComponent<ArrowController>();
                if (otherArrow.Target != currentTarget)
                {
                    Destroy(gameObject);
                }
                break;
            case "Down":
                if (arrowType == ArrowType.rain)
                {
                    EventManager.Send("ArrowRainRecord");
                    EventManager.Send<Vector3>("HeroRainPlay", transform.localPosition);
                }
                Destroy(gameObject);
                break;
        }
    }
}
