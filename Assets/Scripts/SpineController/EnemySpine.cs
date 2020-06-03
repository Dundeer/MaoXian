using Spine;
using System.Collections;
using UnityEngine;


public enum GetHitType
{
    /// <summary>
    /// 普通
    /// </summary>
    Normal,
    /// <summary>
    /// 暴击
    /// </summary>
    CriticalStrike
}

public enum EnemyType
{
    Keight,
    Bird
}

public class EnemySpine : SpineController
{
    /// <summary>
    /// 当前敌人类型
    /// </summary>
    public EnemyType enemyType;
    /// <summary>
    /// 碰撞核
    /// </summary>
    public BoxCollider2D currentBox;
    /// <summary>
    /// 敌人死亡记录
    /// </summary>
    private Custom deadCustom;
    /// <summary>
    /// 创造箭矢的携程
    /// </summary>
    private Coroutine createCoroutine;

    private void Start()
    {
        StandBy();
        currentBloodLine.IniteBlood(CurrentBlood);
    }

    /// <summary>
    /// 获取记录敌人死亡的Custom
    /// </summary>
    /// <param name="deadCustom">记录用的Custom</param>
    public void GetDeadCustom(Custom deadCustom)
    {
        this.deadCustom = deadCustom;
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Dead()
    { 
        deadCustom.complete = true;
    }

    /// <summary>
    /// 受击
    /// </summary>
    /// <param name="hitnumber">伤害数字</param>
    public override void GetHit(float hitnumber)
    {
        hitnumber = GetHitNumberShow(hitnumber);
        CurrentBlood -= hitnumber;
        currentBloodLine.CutBlood(hitnumber);
        if (CurrentBlood <= 0)
        {
            Dead();
        }
        else
        {
            GetHit();
        }
    }

    /// <summary>
    /// 计算攻击数据
    /// </summary>
    /// <param name="hitNumber">伤害数字</param>
    private float GetHitNumberShow(float hitNumber)
    {
        float isCriticalStrike = Random.Range(0, 4.0f);
        if(isCriticalStrike > 3.0f)
        {
            //暴击了
            hitNumber *= 1.5f;
            CreateHitNumberObjcet(GetHitType.CriticalStrike, hitNumber);
        }
        else
        {
            //没有暴击
            CreateHitNumberObjcet(GetHitType.Normal, hitNumber);
            GameObject gameObject = GameObject.Instantiate(NormalEffect);
            float strechScale = (Screen.width * Screen.height) / (DataBase.SCREEN_HEIGHT * DataBase.SCREEN_WIDTH);
            Vector3 uiBoomPos = new Vector3(transform.localPosition.x * strechScale + (0.5f * DataBase.SCREEN_WIDTH), (transform.localPosition.y + 200) * strechScale + (0.5f * DataBase.SCREEN_HEIGHT), 10);
            Debug.Log("爆炸位置" + uiBoomPos);
            gameObject.transform.position = Camera.main.ScreenToWorldPoint(uiBoomPos);
        }
        return hitNumber;
    }

    /// <summary>
    /// 关闭碰撞器
    /// </summary>
    public void CloseBox()
    {
        currentBox.enabled = false;
    }

    public override void StartHandleEvent(TrackEntry trackEntry)
    {
        if (debug)
            Debug.Log("敌人开始动画" + trackEntry);
    }

    public void DelayCreate(float waitTime, int arrowType)
    {
        createCoroutine = StartCoroutine(DelayCreateFunc(waitTime, arrowType));
    }

    public void StopCreate()
    {
        if (createCoroutine != null) StopCoroutine(createCoroutine);
    }

    IEnumerator DelayCreateFunc(float waitTime, int arrowType)
    {
        yield return new WaitForSeconds(waitTime);
        CreateArrow(arrowType, TargetType.Hero);
        createCoroutine = null;
    }
}
