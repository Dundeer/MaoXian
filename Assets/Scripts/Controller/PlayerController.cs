﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using QFramework;
using UnityEngine.UI;

public enum PlayerNumberMode
{
    Single,
    Double
}

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
    /// <summary>
    /// boss警告提示
    /// </summary>
    public HitTextController warningText;
   
    [Space]

    /// <summary>
    /// 英雄模式切换按钮
    /// </summary>
    public Button playerModeChangeBT;
    /// <summary>
    /// 金币回收位置
    /// </summary>
    public Transform goldPos;

    /// <summary>
    /// 当前英雄
    /// </summary>
    private List<HeroSpine> currentHero = new List<HeroSpine>();
    /// <summary>
    /// 当前创建的敌人
    /// </summary>
    private List<EnemySpine> currentEnemy = new List<EnemySpine>();
    /// <summary>
    /// 敌人移动的目标X值
    /// </summary>
    private float enemyTargetX;
    /// <summary>
    /// 用户金币数
    /// </summary>
    private int UserGold;
    /// <summary>
    /// 当前敌人死亡数量
    /// </summary>
    private int currentEnemyDead = 0;
    /// <summary>
    /// 当前敌人组
    /// </summary>
    private EnemySpine[] EnemyObject;
    /// <summary>
    /// 当前敌人下标
    /// </summary>
    private int currentEnemyIndex;
    /// <summary>
    /// 当前玩家数量模式
    /// </summary>
    private PlayerNumberMode playerNumberMode;
    /// <summary>
    /// 当前场上敌人数量
    /// </summary>
    private int currentScreenEnemyNumber = 0;
    /// <summary>
    /// 当前玩家模式最大同屏数量
    /// </summary>
    private int currentPlayerNumberModeScreenEnemyNumber = 1;

    // Start is called before the first frame update
    void Start()
    {
        enemyTargetX = DataBase.SCREEN_WIDTH * 0.25f;
        EventManager.Register("AddGold", AddGoldNumber);
        EventManager.Register<EnemySpine>("CreateAndRecycleGold", CreateAndRecycleGold);
        playerModeChangeBT.onClick.RemoveListener(PlayerModeChange);
        playerModeChangeBT.onClick.AddListener(PlayerModeChange);
    }

    [ContextMenu("PlayerModeChange")]                                                       //代码组件右键添加自定义方法
    private void PlayerModeChange()
    {
        Text btText = playerModeChangeBT.GetComponentInChildren<Text>();
        switch(playerNumberMode)
        {
            case PlayerNumberMode.Double:
                playerNumberMode = PlayerNumberMode.Single;
                btText.text = "单人模式";
                currentPlayerNumberModeScreenEnemyNumber = 1;
                break;
            case PlayerNumberMode.Single:
                playerNumberMode = PlayerNumberMode.Double;
                btText.text = "多人模式";
                currentPlayerNumberModeScreenEnemyNumber = 2;
                break;
        }
        CreateHero();
        HeroMove();
    }

    /// <summary>
    /// 创建英雄
    /// </summary>
    private void CreateHero()
    {
        foreach(HeroSpine herospine in currentHero)
        {
            Destroy(herospine.gameObject);
        }
        currentHero.Clear();
        switch (playerNumberMode)
        {
            case PlayerNumberMode.Double:
                for (int i = 0; i < 2; i++)
                {
                    HeroSpine doubleHero = GameObject.Instantiate(HeroObjcet[i]);
                    currentHero.Add(doubleHero);
                }
                for (int n = currentHero.Count - 1; n > -1; n--)
                {
                    HeroSpine newDoubleHero = currentHero[n];
                    newDoubleHero.transform.SetParent(transform);
                    newDoubleHero.transform.localScale = Vector3.one;
                    switch (n)
                    {
                        case 0:
                            newDoubleHero.transform.localPosition = new Vector2(-DataBase.SCREEN_WIDTH * 0.20f, -160);
                            break;
                        case 1:
                            newDoubleHero.transform.localPosition = new Vector2(-DataBase.SCREEN_WIDTH * 0.40f, -60);
                            break;
                    }
                    newDoubleHero.Create();
                }
                break;
            case PlayerNumberMode.Single:
                HeroSpine singleHero = GameObject.Instantiate(HeroObjcet[0]);
                singleHero.transform.SetParent(transform);
                singleHero.transform.localScale = new Vector3(1, 1, 1);
                singleHero.transform.localPosition = new Vector2(-DataBase.SCREEN_WIDTH * 0.25f, -DataBase.SCREEN_HEIGHT * 0.13f);
                singleHero.Create();
                currentHero.Add(singleHero);
                break;
        }
    }

    /// <summary>
    /// 英雄开始移动
    /// </summary>
    public void HeroMove()
    {
        playerModeChangeBT.gameObject.SetActive(true);
        if (currentHero.Count == 0)
        {
            CreateHero();
        }
        foreach (HeroSpine child in currentHero)
        {
            child.Forward();
        }
    }

    public void EnemyFunc(Custom moveCustom)
    {
        //隐藏切换按钮
        playerModeChangeBT.gameObject.SetActive(false);

        //随机怪物队列
        currentEnemyIndex = 0;
        int randomEnemyGroup = 0;//UnityEngine.Random.Range(0, 3);
        switch (randomEnemyGroup)
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

        //敌人开始移动
        MoveEnemy(moveCustom);

    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    private EnemySpine CreateEnemy()
    {
        EnemySpine createEnemy = GameObject.Instantiate(EnemyObject[currentEnemyIndex], transform, false);
        createEnemy.Create();
        float enemyHeight = 0;
        switch(createEnemy.enemyType)
        {
            case EnemyType.Bird:
                switch(currentEnemyIndex)
                {
                    case 0:
                        enemyHeight = DataBase.SCREEN_HEIGHT * -0.08f;
                        break;
                    case 1:
                        enemyHeight = 0;
                        break;
                    case 2:
                        enemyHeight = DataBase.SCREEN_HEIGHT * 0.08f;
                        break;
                }
                break;
            case EnemyType.Keight:
                enemyHeight = -DataBase.SCREEN_HEIGHT * 0.13f;
                break;
        }
        createEnemy.transform.localPosition = new Vector2(DataBase.SCREEN_WIDTH / 2, enemyHeight);
        currentEnemy.Add(createEnemy);
        currentEnemyIndex++;
        currentScreenEnemyNumber++;
        return createEnemy;
    }

    private void CreateWarningText()
    {
        HitTextController warning = GameObject.Instantiate(warningText);
        warning.transform.SetParent(transform);
        warning.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 移动敌人
    /// </summary>
    /// <returns>移动时间</returns>
    private void MoveEnemy(Custom moveCustom)
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
        while(true)
        {
            //创建敌人
            EnemySpine createEnemy = CreateEnemy();

            //移动
            Vector2 enemyPos = createEnemy.transform.localPosition;
            float randomX = 0;
            switch(currentEnemyIndex)
            {
                case 1:
                    randomX = -0.10f;
                    break;
                case 2:
                    randomX = 0;
                    break;
                case 3:
                    randomX = 0.10f;
                    break;
            }
            float newEnmeyTargetX = enemyTargetX + DataBase.SCREEN_WIDTH * randomX;
            float needTime = Math.Abs(enemyPos.x - newEnmeyTargetX) / (DataBase.LIGHT_BG_SPEED * 100);
            createEnemy.transform.DOLocalMoveX(newEnmeyTargetX, needTime);
            yield return new WaitForSeconds(needTime * 0.73f);

            //移动完成敌人开始攻击
            createEnemy.Attack();
            if (moveCustom != null)
                moveCustom.complete = true;

            //等待创建下一个敌人
            if (currentEnemyIndex == EnemyObject.Length)
            {
                yield break;
            }
            else
            {
                if (currentScreenEnemyNumber < currentPlayerNumberModeScreenEnemyNumber)
                {
                    yield return new WaitForSeconds(UnityEngine.Random.Range(3, 5));
                }
                else
                {
                    while(currentScreenEnemyNumber >= currentPlayerNumberModeScreenEnemyNumber)
                    {
                        yield return null;
                    }

                    yield return new WaitForSeconds(UnityEngine.Random.Range(3, 5));
                }
            }
        }
    }

    public Coroutine AttackForEnd()
    {
        StartAttack();

        return StartCoroutine(EnemyDead());
    }

    public void StartAttack()
    {
        foreach (HeroSpine heroSpine in currentHero)
        {
            heroSpine.Attack();
        }
    }

    IEnumerator EnemyDead()
    {
        while (true)
        {
            yield return null;

            if (currentEnemyDead == EnemyObject.Length)
            {
                currentEnemyDead = 0;
                HeroStandBy();

                yield return new WaitForSeconds(3.0f);

                yield break;
            }
        }
    }

    private void HeroStandBy()
    {
        foreach (HeroSpine heroSpine in currentHero)
        {
            heroSpine.StandBy();
        }
    }

    /// <summary>
    /// 产生金币
    /// </summary>
    private List<GoldController> CreateGold(EnemySpine createEnemy)
    {
        if (GoldObject == null)
        {
            Debug.Log("金币预制体没有找到");
            return null;
        };
        Vector2 enemyPos = createEnemy.transform.localPosition;
        List<GoldController> goldGroup = new List<GoldController>();
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
        Destroy(createEnemy.gameObject);
        currentEnemy.Remove(createEnemy);
        return goldGroup;
    }

    private void CreateAndRecycleGold(EnemySpine createEnemey)
    {
        currentEnemyDead++;
        currentScreenEnemyNumber--;
        StartCoroutine(CreateAndRecycleGoldTask(createEnemey));
    }

    /// <summary>
    /// 回收金币
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateAndRecycleGoldTask(EnemySpine createEnemey)
    {
        //创建金币
        List<GoldController> goldGroup = CreateGold(createEnemey);
        yield return new WaitForSeconds(2.0f);

        //回收金币
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
                recycleList.Clear();
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
}
