using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 敌人
    /// </summary>
    public EnemySpine[] EnemyObject;
    /// <summary>
    /// 英雄
    /// </summary>
    public HeroSpine[] HeroObjcet;
    /// <summary>
    /// 金币预制体
    /// </summary>
    public GoldController GoldObject;
    /// <summary>
    /// 金币回收位置
    /// </summary>
    public Transform goldPos;
    /// <summary>
    /// 当前英雄
    /// </summary>
    private HeroSpine currentHero;
    /// <summary>
    /// 当前创建的敌人
    /// </summary>
    private EnemySpine currentEnemy;
    /// <summary>
    /// 敌人移动的目标X值
    /// </summary>
    private float enemyTargetX;
    /// <summary>
    /// 产生的金币组
    /// </summary>
    private List<GoldController> goldGroup = new List<GoldController>();

    // Start is called before the first frame update
    void Start()
    {
        enemyTargetX = DataBase.SCREEN_WIDTH * 0.25f;
    }

    /// <summary>
    /// 创建英雄
    /// </summary>
    private void CreateHero()
    {
        currentHero = GameObject.Instantiate(HeroObjcet[0]);
        currentHero.transform.SetParent(transform);
        currentHero.transform.localScale = new Vector3(1, 1, 1);
        currentHero.transform.localPosition = new Vector2(-DataBase.SCREEN_WIDTH * 0.25f, -DataBase.SCREEN_HEIGHT * 0.13f);
    }

    /// <summary>
    /// 英雄开始移动
    /// </summary>
    public void HeroMove()
    {
        if(currentHero == null)
        {
            CreateHero();
        }
        currentHero.Forward();
    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    public void CreateEnemy()
    {
        int randomEnemyIndex = UnityEngine.Random.Range(0, EnemyObject.Length);
        currentEnemy = GameObject.Instantiate(EnemyObject[randomEnemyIndex]);
        currentEnemy.transform.SetParent(transform);
        currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        currentEnemy.transform.localPosition = new Vector2(DataBase.SCREEN_WIDTH / 2, -DataBase.SCREEN_HEIGHT * 0.13f);
    }

    /// <summary>
    /// 移动敌人
    /// </summary>
    /// <returns>移动时间</returns>
    public void MoveEnemy(Custom moveCustom)
    {
        StartCoroutine(MoveEnemyFunc(moveCustom));
    }

    IEnumerator MoveEnemyFunc(Custom moveCustom)
    {
        Vector2 enemyPos = currentEnemy.transform.localPosition;
        float needTime = Math.Abs(enemyPos.x - enemyTargetX) / (DataBase.LIGHT_BG_SPEED * 100);
        currentEnemy.transform.DOLocalMoveX(enemyTargetX, needTime);
        yield return new WaitForSeconds(needTime * 0.73f);
        moveCustom.complete = true;
    }

    public void StartAttack()
    {
        currentHero.Attack();
        currentEnemy.Attack();
    }

    public Coroutine EnemyDeaded()
    {
        return StartCoroutine(EnemyDead());
    }

    IEnumerator EnemyDead()
    {
        Custom deadCustom = new Custom();
        currentEnemy.GetDeadCustom(deadCustom);
        yield return deadCustom;
    }

    public void HeroStandBy()
    {
        currentHero.StandBy();
    }

    /// <summary>
    /// 开始创建金币
    /// </summary>
    public Coroutine StartCreateGold()
    {
        return StartCoroutine(CreateGold());
    }

    /// <summary>
    /// 产生金币
    /// </summary>
    IEnumerator CreateGold()
    {
        if (GoldObject == null)
        {
            Debug.Log("金币预制体没有找到");
            yield break;
        };
        currentEnemy.CloseBox();
        int randomGoldNumber = UnityEngine.Random.Range(5, 10);
        while (randomGoldNumber > 0)
        {
            GoldController child = GameObject.Instantiate(GoldObject);
            child.transform.SetParent(transform);
            child.transform.localPosition = currentEnemy.transform.localPosition;
            child.transform.localScale = new Vector3(1, 1, 1);
            child.AddForce();
            goldGroup.Add(child);
            randomGoldNumber--;
        }
        Destroy(currentEnemy.gameObject);
        yield return StartCoroutine(RecycleGold());
    }

    /// <summary>
    /// 回收金币
    /// </summary>
    /// <returns></returns>
    IEnumerator RecycleGold()
    {
        yield return new WaitForSeconds(2.0f);
        Debug.Log("回收金币");
        var recycleList = new List<Custom>(goldGroup.Count);
        foreach (GoldController child in goldGroup)
        {
            recycleList.Add(child.RecycleGold(goldPos.localPosition));
        }
        while (true)
        {
            bool isComplete = true;
            foreach(Custom child in recycleList)
            {
                if(!child.complete)
                {
                    isComplete = false;
                    break;
                }
            }
            if (isComplete)
            {
                goldGroup.Clear();
                yield break;
            }
            yield return null;
        }
    }

}
