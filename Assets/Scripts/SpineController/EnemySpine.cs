using Spine;
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

public class EnemySpine : SpineController
{
    /// <summary>
    /// 敌人血量
    /// </summary>
    public float EnemyBlood = 100;
    /// <summary>
    /// 碰撞核
    /// </summary>
    public BoxCollider2D currentBox;
    /// <summary>
    /// 当前血条
    /// </summary>
    public EnemyBloodLine currentBloodLine;
    /// <summary>
    /// 普通受击文字
    /// </summary>
    public HitTextBase noramlHitObject;
    /// <summary>
    /// 暴击文字
    /// </summary>
    public HitTextBase criticalStrikeObject;
    /// <summary>
    /// 普通受击的特效
    /// </summary>
    public GameObject NormalEffect;
    /// <summary>
    /// 敌人死亡记录
    /// </summary>
    private Custom deadCustom;

    private void Start()
    {
        StandBy();
        currentBloodLine.IniteBlood(EnemyBlood);
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
    public void GetHit(float hitnumber)
    {
        hitnumber = GetHitNumberShow(hitnumber);
        EnemyBlood -= hitnumber;
        currentBloodLine.CutBlood(hitnumber);
        if (EnemyBlood <= 0)
        {
            Dead();
        }
        else
        {
            GetHit();
        }
    }

    /// <summary>
    /// 受击
    /// </summary>
    /// <param name="hitnumber">伤害数字</param>
    public void GetHit(int hitnumber)
    {
        float newHitNumber = GetHitNumberShow(hitnumber);
        EnemyBlood -= newHitNumber;
        currentBloodLine.CutBlood(newHitNumber);
        if (EnemyBlood <= 0)
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
        float isCriticalStrike = UnityEngine.Random.Range(0, 4.0f);
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
    /// 创建受击数字
    /// </summary>
    /// <param name="hitType">受击类型</param>
    /// <param name="number">伤害数字</param>
    private void CreateHitNumberObjcet(GetHitType hitType, float number)
    {
        HitTextBase gameObject = null;
        switch (hitType)
        {
            case GetHitType.Normal:
                gameObject = GameObject.Instantiate(noramlHitObject);
                gameObject.transform.SetParent(transform.parent);
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                gameObject.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);
                break;
            case GetHitType.CriticalStrike:
                gameObject = GameObject.Instantiate(criticalStrikeObject);
                gameObject.transform.SetParent(transform.parent);
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                gameObject.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + 200);
                break;
        }
        if (gameObject == null) return;
        gameObject.SetText(number);
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
}
