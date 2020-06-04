using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgicEffectController : MonoBehaviour
{
    /// <summary>
    /// 监听类型
    /// </summary>
    public ListenceTarget listenceTarget;

    private UnityEngine.ParticleSystem parentParticle;

    private UnityEngine.ParticleSystem[] childParticle;

    private void Start()
    {
        string target = "";
        switch (listenceTarget)
        {
            case ListenceTarget.Enemy:
                target = "EnemyMagicPlay";
                break;
            case ListenceTarget.Hero:
                target = "HeroMagicPlay";
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
        transform.localPosition = pos;
        foreach (ParticleSystem child in childParticle)
        {
            child.gameObject.SetActive(true);
        }
        parentParticle.Play();
        StartCoroutine(StopEffects());
    }

    IEnumerator StopEffects()
    {
        yield return new WaitForSeconds(2.0f);
        EffectInit();
    }
}
