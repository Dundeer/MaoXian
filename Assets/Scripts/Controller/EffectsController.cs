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
    /// <summary>
    /// 监听类型
    /// </summary>
    public ListenceTarget listenceTarget;
    /// <summary>
    /// 粒子特效类型
    /// </summary>
    public PEffectsType pEffectsType;
    /// <summary>
    /// 是否控制停止
    /// </summary>
    public bool isStop;
    /// <summary>
    /// 停止的时间
    /// </summary>
    public float stopTime;

    private UnityEngine.ParticleSystem parentParticle;

    private UnityEngine.ParticleSystem[] childParticle;

    private void Start()
    {
        string target = "";
        switch (listenceTarget)
        {
            case ListenceTarget.Enemy:
                target = "Enemy" + pEffectsType.ToString() + "Play";
                break;
            case ListenceTarget.Hero:
                target = "Hero" + pEffectsType.ToString() + "Play";
                break;
        }
        parentParticle = GetComponent<ParticleSystem>();
        childParticle = GetComponentsInChildren<UnityEngine.ParticleSystem>();
        EffectInit();
        EventManager.Register<Vector3>(target, PlayEffects);
    }

    private void EffectInit()
    {
        parentParticle.Stop();
        foreach (ParticleSystem child in childParticle)
        {
            child.gameObject.SetActive(false);
            child.Stop();
        }
    }

    private void PlayEffects(Vector3 pos)
    {
        if (debug)
            Debug.Log($"{name} 播放特效");
        transform.localPosition = pos;
        foreach (ParticleSystem child in childParticle)
        {
            child.gameObject.SetActive(true);
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

    private void OnDestroy()
    {
        string target = "";
        switch (listenceTarget)
        {
            case ListenceTarget.Enemy:
                target = "Enemy" + pEffectsType.ToString() + "Play";
                break;
            case ListenceTarget.Hero:
                target = "Hero" + pEffectsType.ToString() + "Play";
                break;
        }
        EventManager.UnRegister<Vector3>(target, PlayEffects);
    }
}
