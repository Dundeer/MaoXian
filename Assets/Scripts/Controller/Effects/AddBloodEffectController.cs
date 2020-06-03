using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public class AddBloodEffectController : MonoBehaviour
{
    /// <summary>
    /// 监听类型
    /// </summary>
    public ListenceTarget listenceTarget;

    private UnityEngine.ParticleSystem parentParticle;

    private void Start()
    {
        string target = "";
        switch (listenceTarget)
        {
            case ListenceTarget.Enemy:
                target = "EnemyAddBlood";
                break;
            case ListenceTarget.Hero:
                target = "HeroAddBlood";
                break;
        }
        parentParticle = GetComponent<ParticleSystem>();
        parentParticle.Stop();
        EventManager.Register(target, PlayEffects);
    }

    private void PlayEffects()
    {
        parentParticle.Play();
    }
}
