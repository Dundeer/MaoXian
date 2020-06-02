using QFramework;
using System.Collections;
using UnityEngine;

public class Custom : CustomYieldInstruction
{
    public bool complete = false;

    public override bool keepWaiting
    {
        get
        {
            return !complete;
        }
    }
}

public class MainManager : QMonoSingleton<MainManager>
{
    #region Member
    /// <summary>
    /// 移动物体的组别
    /// </summary>
    public MoveChild[] moveGroup;
    /// <summary>
    /// 人物界面控制
    /// </summary>
    public PlayerController playerController;
    #endregion

    private void Awake()
    {
        StartCoroutine(MainTask());
    }

    IEnumerator MainTask()
    {
        while (true)
        {
            //英雄移动
            playerController.HeroMove();

            //移动背景
            foreach (MoveChild child in moveGroup)
            {
                child.StartMove();
            }

            //等待产生敌人
            yield return new WaitForSeconds(DataBase.DELAY_CREATE_TIME);

            //创建敌人
            playerController.CreateEnemy();

            //敌人开始移动
            Custom moveCustom = new Custom();
            playerController.MoveEnemy(moveCustom);

            //停止背景
            yield return StartCoroutine(WaitEnemyMoveStop(moveCustom));

            //开始战斗
            playerController.StartAttack();

            //等待敌人死亡
            yield return playerController.EnemyDeaded();

            //英雄等待金币收取
            playerController.HeroStandBy();

            //产生金币结束
            yield return playerController.StartCreateGold();
        }
    }

    IEnumerator WaitEnemyMoveStop(Custom moveCustom)
    {
        
        while (true)
        {
            yield return null;
            if (moveCustom.complete)
            {
                foreach (MoveChild child in moveGroup)
                {
                    child.StopMove();
                }
                yield break;
            }
        }
    }

}
