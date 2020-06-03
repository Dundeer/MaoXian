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
        parentParticle.Stop();
        childParticle = GetComponentsInChildren<UnityEngine.ParticleSystem>();
        EventManager.Register<Vector3>(target, PlayEffects);
    }

    private void PlayEffects(Vector3 pos)
    {
        transform.localPosition = pos;
        parentParticle.Play();
        StartCoroutine(StopEffects());
    }

    IEnumerator StopEffects()
    {
        yield return new WaitForSeconds(2.0f);
        parentParticle.Stop();
        foreach(ParticleSystem child in childParticle)
        {
            child.Stop();
        }
    }
}
