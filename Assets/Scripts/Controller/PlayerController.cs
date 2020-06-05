using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using QFramework;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 敌人组1
    /// </summary>
    public EnemySpine[] EnemyObject1;
    /// <summary>
    /// 敌人组2
    /// </summary>
    public EnemySpine[] EnemyObject2;
    /// <summary>
    /// 敌人组3
    /// </summary>
    public EnemySpine[] EnemyObject3;
    /// <summary>
    /// 英雄
    /// </summary>
    public HeroSpine[] HeroObjcet;
    /// <summary>
    /// 金币预制体
    /// </summary>
    public GoldController GoldObject;

    [Space]
  
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
    /// <summary>
    /// 用户金币数
    /// </summary>
    private int UserGold;
    /// <summary>
    /// 当前敌人组
    /// </summary>
    private EnemySpine[] EnemyObject;
    /// <summary>
    /// 当前敌人下标
    /// </summary>
    private int currentEnemyIndex;

    // Start is called before the first frame update
    void Start()
    {
        enemyTargetX = DataBase.SCREEN_WIDTH * 0.25f;
        EventManager.Register("AddGold", AddGoldNumber);
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
        currentHero.Create();
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
        if(EnemyObject == null || currentEnemyIndex == EnemyObject.Length)
        {
            currentEnemyIndex = 0;
            int randomEnemyGroup = 0;//UnityEngine.Random.Range(0, 3);
            switch(randomEnemyGroup)
            {
                case 0:
                    EnemyObject = EnemyObject1;
                    break;
                case 1:
                    EnemyObject = EnemyObject2;
                    break;
                case 2:
                    EnemyObject = EnemyObject3;
                    break;
            }
        }
        currentEnemy = GameObject.Instantiate(EnemyObject[currentEnemyIndex], transform, false);
        currentEnemy.Create();
        float enemyHeight = 0;
        switch(currentEnemy.enemyType)
        {
            case EnemyType.Bird:
                enemyHeight = 0;
                break;
            case EnemyType.Keight:
                enemyHeight = -DataBase.SCREEN_HEIGHT * 0.13f;
                break;
        }
        currentEnemy.transform.localPosition = new Vector2(DataBase.SCREEN_WIDTH / 2, enemyHeight);
        currentEnemyIndex++;
    }

    /// <summary>
    /// 移动敌人
    /// </summary>
    /// <returns>移动时间</returns>
    public void MoveEnemy(Custom moveCustom)
    {
        StartCoroutine(MoveEnemyFunc(moveCustom));
    }

    /// <summary>
    /// 移动敌人携程
    /// </summary>
    /// <param name="moveCustom">移动监听</param>
    /// <returns></returns>
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
        Vector2 enemyPos = currentEnemy.transform.localPosition;
        int randomGoldNumber = UnityEngine.Random.Range(5, 10);
        while (randomGoldNumber > 0)
        {
            GoldController child = GameObject.Instantiate(GoldObject, transform, false);
            child.transform.localPosition = enemyPos;
            Vector2 randomPos = new Vector2(enemyPos.x + UnityEngine.Random.Range(-150, 150), -DataBase.SCREEN_HEIGHT * 0.13f - UnityEngine.Random.Range(0, 200));
            child.transform.DOLocalMove(randomPos, 0.45f);
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

    /// <summary>
    /// 添加金币数量
    /// </summary>
    private void AddGoldNumber()
    {
        UserGold++;
        EventManager.Send("GoldScaleAnim");
        EventManager.Send<int>("SetGoldNumber", UserGold);
    }

    /// <summary>
    /// 获得当前敌人的坐标
    /// </summary>
    /// <returns>当前敌人的坐标</returns>
    public Vector2 GetEnemyPos()
    {
        return currentEnemy.transform.localPosition;
    }

}
