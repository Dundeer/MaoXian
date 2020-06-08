using DG.Tweening;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PEffectsType
{
    Wind,
    Magic,
    Rain,
    Care,
    Aim,
    AddBlood,
    Boom
}

public class EffectsController : MonoBehaviour
{
    public bool debug;

    [Space]

    /// <summary>
    /// 监听类型
    /// </summary>
    public ListenceTarget listenceTarget;
    /// <summary>
    /// 粒子特效类型
    /// </summary>
    public PEffectsType pEffectsType;

    [Space]

    /// <summary>
    /// 是否控制停止
    /// </summary>
    public bool isStop;
    /// <summary>
    /// 停止的时间
    /// </summary>
    public float stopTime;

    //

    private bool isSpecial;

    private int specialMode = 0;

    private Coroutine specialCorotine;

    private ParticleSystem parentParticle;

    private ParticleSystem[] childParticle;

    private void Start()
    {
        string Play = "";
        string Stop = "";
        string Time = "";
        string Set = "";
        string Mode = "";
        string Pos = "";
        switch (listenceTarget)
        {
            case ListenceTarget.Enemy:
                Play = "Enemy" + pEffectsType.ToString() + "Play";
                Stop = "Enemy" + pEffectsType.ToString() + "Stop";
                Time = "Enemy" + pEffectsType.ToString() + "Time";
                Set = "Enemy" + pEffectsType.ToString() + "Set";
                Mode = "Enemy" + pEffectsType.ToString() + "Mode";
                Pos = "Enemy" + pEffectsType.ToString() + "Pos";
                break;
            case ListenceTarget.Hero:
                Play = "Hero" + pEffectsType.ToString() + "Play";
                Stop = "Hero" + pEffectsType.ToString() + "Stop";
                Time = "Hero" + pEffectsType.ToString() + "Time";
                Set = "Hero" + pEffectsType.ToString() + "Set";
                Mode = "Hero" + pEffectsType.ToString() + "Mode";
                Pos = "Hero" + pEffectsType.ToString() + "Pos";
                break;
        }
        parentParticle = GetComponent<ParticleSystem>();
        childParticle = GetComponentsInChildren<ParticleSystem>();
        EffectInit();
        EventManager.Register<Vector3>(Play, PlayEffects);
        EventManager.Register(Stop, EffectInit);
        EventManager.Register<float>(Time, SetTime);
        EventManager.Register<bool>(Set, SetSpecial);
        EventManager.Register<int>(Mode, SetSpecialMode);
        EventManager.Register<Vector3>(Pos, SetPos);
        EventManager.Register("EffectAllStop", EffectInit);
    }

    private void EffectInit()
    {
        parentParticle.Stop();
        foreach (ParticleSystem child in childParticle)
        {
            child.gameObject.SetActive(false);
            child.Stop();
        }
        if (specialCorotine != null)
            StopCoroutine(specialCorotine);
    }

    public void PlayEffects(Vector3 pos)
    {
        if (debug)
            Debug.Log($"{name} 播放特效");
        transform.localPosition = pos;
        switch(pEffectsType)
        {
            case PEffectsType.Magic:
                if (!isSpecial)
                {
                    foreach (ParticleSystem child in childParticle)
                    {
                        child.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        child.gameObject.SetActive(true);
                    }
                }
                else
                {
                    foreach (ParticleSystem child in childParticle)
                    {
                        child.transform.localScale = Vector3.zero;
                        child.gameObject.SetActive(true);
                    }
                    MagicEffectShow();
                }
                break;
            default:
                foreach (ParticleSystem child in childParticle)
                {
                    child.gameObject.SetActive(true);
                }
                break;
        }
        parentParticle.Play();
        if (isStop)
            StartCoroutine(StopEffects());
    }

    IEnumerator StopEffects()
    {
        yield return new WaitForSeconds(stopTime);
        EffectInit();
    }

    /// <summary>
    /// 设置时间
    /// </summary>
    /// <param name="times">目标时间</param>
    private void SetTime(float times)
    {
        if (debug)
            Debug.Log($"设置停止时间{times}");
        stopTime = times;
    }

    /// <summary>
    /// 设置是否开始特殊显示
    /// </summary>
    /// <param name="set">是否设置</param>
    private void SetSpecial(bool set)
    {
        isSpecial = set;
    }

    /// <summary>
    /// 设置特殊模式类型
    /// </summary>
    /// <param name="mode"></param>
    private void SetSpecialMode(int mode)
    {
        if (debug)
            Debug.Log($"设置的特殊模式为{mode}");
        specialMode = mode;
    }

    /// <summary>
    /// 刷新位置
    /// </summary>
    /// <param name="pos">坐标位置</param>
    private void SetPos(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    /// <summary>
    /// 开启魔法的特殊展示
    /// </summary>
    private void MagicEffectShow()
    {
        if (debug)
            Debug.Log($"魔法特效展示模式{specialMode}");
        switch (specialMode)
        {
            case 0:
                specialCorotine = StartCoroutine(MagicEffectShowTask());
                break;
            case 1:
                specialCorotine = StartCoroutine(MagicEffectShotTask());
                break;
        }
    }

    /// <summary>
    /// 魔法特殊展示携程
    /// </summary>
    /// <returns></returns>
    IEnumerator MagicEffectShowTask()
    {
        //逐渐出现
        foreach (ParticleSystem child in childParticle)
        {
            child.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f).SetEase(Ease.OutQuint);
        }
        yield return new WaitForSeconds(0.7f);

        //向上移动
        transform.DOLocalMoveY(transform.localPosition.y + 150, 1.0f).SetEase(Ease.InOutQuint);
        yield return new WaitForSeconds(1.2f);

        //变大
        foreach (ParticleSystem child in childParticle)
        {
            child.transform.DOScale(Vector3.one, 0.5f);
        }
        specialCorotine = null;
    }

    IEnumerator MagicEffectShotTask()
    {
        //逐渐出现
        foreach (ParticleSystem child in childParticle)
        {
            child.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.7f).SetEase(Ease.OutQuint);
        }
        yield return new WaitForSeconds(1.15f);

        transform.DOLocalMove(new Vector3(DataBase.SCREEN_WIDTH / 2, DataBase.SCREEN_HEIGHT / 2 + 200), 0.5f);
        yield return new WaitForSeconds(0.5f);

        foreach (ParticleSystem child in childParticle)
        {
            child.transform.DOScale(Vector3.one, 0.5f);
        }
    }

    private void OnDestroy()
    {
        string Set = "";
        string Play = "";
        string Stop = "";
        string Time = "";
        string Pos = "";
        string Mode = "";
        switch (listenceTarget)
        {
            case ListenceTarget.Enemy:
                Play = "Enemy" + pEffectsType.ToString() + "Play";
                Stop = "Enemy" + pEffectsType.ToString() + "Stop";
                Time = "Enemy" + pEffectsType.ToString() + "Time";
                Set = "Enemy" + pEffectsType.ToString() + "Set";
                Mode = "Enemy" + pEffectsType.ToString() + "Mode";
                Pos = "Enemy" + pEffectsType.ToString() + "Pos";
                break;
            case ListenceTarget.Hero:
                Play = "Hero" + pEffectsType.ToString() + "Play";
                Stop = "Hero" + pEffectsType.ToString() + "Stop";
                Time = "Hero" + pEffectsType.ToString() + "Time";
                Set = "Hero" + pEffectsType.ToString() + "Set";
                Mode = "Hero" + pEffectsType.ToString() + "Mode";
                Pos = "Hero" + pEffectsType.ToString() + "Pos";
                break;
        }
        EventManager.UnRegister<Vector3>(Play, PlayEffects);
        EventManager.UnRegister(Stop, EffectInit);
        EventManager.UnRegister<float>(Time, SetTime);
        EventManager.UnRegister<bool>(Set, SetSpecial);
        EventManager.UnRegister<int>(Mode, SetSpecialMode);
        EventManager.UnRegister<Vector3>(Pos, SetPos);
    }
}
