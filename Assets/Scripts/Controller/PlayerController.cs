using System;
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
    public EnemySpine[] EnemyObject1;           // 敌人组1
    public EnemySpine[] EnemyObject2;           // 敌人组2
    public EnemySpine[] EnemyObject3;           // 敌人组3
    public HeroSpine[] HeroObjcet;              // 英雄
    public GoldController GoldObject;           // 金币预制体

    [Space]
    public Button playerModeChangeBT;           // 英雄模式切换按钮
    public Transform goldIcon;                      // 金币图标
    public Text goldNumber;                     // 金币数量

    [Space]
    public Transform goldPos;                   // 金币回收位置

    private List<HeroSpine> currentHero = new List<HeroSpine>();        // 当前英雄
    private List<EnemySpine> currentEnemy = new List<EnemySpine>();     // 当前创建的敌人
    private float enemyTargetX;                 // 敌人移动的目标X值
    private int UserGold;                       // 用户金币数
    private int currentEnemyDead = 0;           // 当前敌人死亡数量
    private EnemySpine[] EnemyObject;           // 当前敌人组
    private int currentEnemyIndex;              // 当前敌人下标
    private PlayerNumberMode playerNumberMode;  // 当前玩家数量模式
    private int currentScreenEnemyNumber = 0;   // 当前场上敌人数量
    private int currentPlayerNumberModeScreenEnemyNumber = 1;           // 当前玩家模式最大同屏数量
    private Custom enemyDeadCustom;             // 敌人死亡标记

    public void Open()
    {
        enemyTargetX = DataBase.SCREEN_WIDTH * 0.25f;
        EventManager.Register("AddGold", AddGoldNumber);
        EventManager.Register<EnemySpine>("CreateAndRecycleGold", CreateAndRecycleGold);
        playerModeChangeBT.onClick.RemoveListener(PlayerModeChange);
        playerModeChangeBT.onClick.AddListener(PlayerModeChange);
        playerNumberMode = PlayerNumberMode.Single;
        CreateHero();
    }

    #region Hero
    // [ContextMenu("PlayerModeChange")]                                //代码组件右键添加自定义方法
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
        MoveHero();
    }

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
                HeroSpine singleHero = GameObject.Instantiate(HeroObjcet[0], transform, false);
                singleHero.transform.localPosition = new Vector2(-DataBase.SCREEN_WIDTH * 0.25f, -DataBase.SCREEN_HEIGHT * 0.13f);
                singleHero.Create();
                currentHero.Add(singleHero);
                break;
        }
    }

    public void MoveHero()
    {
        playerModeChangeBT.gameObject.SetActive(true);
        foreach (HeroSpine child in currentHero)
        {
            child.Forward();
        }
    }
    #endregion

    #region Enemy
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
        StartCoroutine(MoveEnemyTask(moveCustom));
    }

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
    #endregion

    #region AttackAndEnd
    public Custom AttackForEnd()
    {
        foreach (HeroSpine heroSpine in currentHero)
        {
            heroSpine.Attack();
        }
        enemyDeadCustom = new Custom();
        return enemyDeadCustom;
    }

    private List<GoldController> CreateGold(EnemySpine deadEnemy)
    {
        Vector2 enemyPos = deadEnemy.transform.localPosition;
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
        Destroy(deadEnemy.gameObject);
        currentEnemy.Remove(deadEnemy);
        return goldGroup;
    }

    private void CreateAndRecycleGold(EnemySpine createEnemey)
    {
        currentEnemyDead++;
        currentScreenEnemyNumber--;
        StartCoroutine(CreateAndRecycleGoldTask(createEnemey));

        if (currentEnemyDead == EnemyObject.Length)
        {
            currentEnemyDead = 0;
            foreach (HeroSpine heroSpine in currentHero)
            {
                heroSpine.StandBy();
            }

            DOTween.Sequence().AppendInterval(3.0f).OnComplete(() =>
            {
                enemyDeadCustom.complete = true;
            });
        }
    }

    /// <summary>
    /// 添加金币数量
    /// </summary>
    private void AddGoldNumber()
    {
        UserGold++;
        goldNumber.text = UserGold.ToString();
        StartGoldIconAnim();
    }

    private void StartGoldIconAnim()
    {
        goldIcon.DOKill();
        goldIcon.localScale = Vector3.one;
        goldIcon.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.05f).SetLoops(2, LoopType.Yoyo);
    }
    #endregion

    #region 携程
    /// <summary>
    /// 移动敌人携程
    /// </summary>
    /// <param name="moveCustom">移动监听</param>
    /// <returns></returns>
    IEnumerator MoveEnemyTask(Custom moveCustom)
    {
        while (true)
        {
            //创建敌人
            EnemySpine createEnemy = CreateEnemy();

            //移动
            Vector2 enemyPos = createEnemy.transform.localPosition;
            float randomX = 0;
            switch (currentEnemyIndex)
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
                while (currentScreenEnemyNumber >= currentPlayerNumberModeScreenEnemyNumber)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(UnityEngine.Random.Range(3, 5));
            }
        }
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
            foreach (Custom child in recycleList)
            {
                if (!child.complete)
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
    #endregion
}
