using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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
    /// <summary>
    /// 箭的移动速度
    /// </summary>
    public float MoveSpeed;
    /// <summary>
    /// 箭矢类型
    /// </summary>
    public ArrowType arrowType;
    /// <summary>
    /// 造成伤害
    /// </summary>
    public int AttackBlood;

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

    private void Start()
    {
        ArrowMove();
    }
    /// <summary>
    /// 设置目标类型
    /// </summary>
    /// <param name="type"></param>
    public void SetTarget(TargetType type)
    {
        currentTarget = type;
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
    /// 箭的移动
    /// </summary>
    public void ArrowMove()
    {
        StartCoroutine(Move());
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
    #region 碰撞方法

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collistionObject = collision.gameObject;
        switch (collistionObject.tag)
        {
            case "Hero":
                CollisionHero(collistionObject);
                break;
            case "Enemy":
                CollistionEnemy(collistionObject);
                break;
            case "Arrow":
                CollistionArrow(collistionObject);
                break;
            case "Down":
                Destroy(GetComponent<Rigidbody2D>());
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collistionObject = collision.gameObject;
        switch (collistionObject.tag)
        {
            case "Hero":
                CollisionHero(collistionObject);
                break;
            case "Enemy":
                CollistionEnemy(collistionObject);
                break;
            case "Arrow":
                CollistionArrow(collistionObject);
                break;
        }
    }


    /// <summary>
    /// 碰到英雄
    /// </summary>
    public virtual void CollisionHero(GameObject collision)
    {
        switch(currentTarget)
        {
            case TargetType.Hero:
                HeroSpine heroSpine = collision.GetComponent<HeroSpine>();
                heroSpine.GetHit(AttackBlood);
                Destroy(gameObject);
                break;
        }
    }

    /// <summary>
    /// 碰到敌人
    /// </summary>
    public virtual void CollistionEnemy(GameObject collision)
    {
        switch (currentTarget)
        {
            case TargetType.Enemy:
                EnemySpine enemySpine = collision.GetComponent<EnemySpine>();
                enemySpine.GetHit(AttackBlood);
                switch (arrowType)
                {
                    case ArrowType.rain:
                        Destroy(gameObject);
                        break;
                    default:
                        Destroy(gameObject);
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// 碰到箭
    /// </summary>
    public virtual void CollistionArrow(GameObject collision)
    {
        ArrowController otherArrow = collision.GetComponent<ArrowController>();
        if(otherArrow.Target != currentTarget)
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
