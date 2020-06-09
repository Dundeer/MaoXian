using QFramework;
using System.Collections;
using UnityEngine;

public enum ListenceTarget
{
    Hero,
    Enemy
}

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

    private void Init()
    {
        playerController.Open();
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

            //敌人方法
            Custom moveCustom = new Custom();
            playerController.EnemyFunc(moveCustom);
            yield return moveCustom;

            //停止背景
            foreach (MoveChild child in moveGroup)
            {
                child.StopMove();
            }

            //开始战斗直至敌人死亡
            yield return playerController.AttackForEnd();
        }
    }
}
