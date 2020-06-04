using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMagicController : MonoBehaviour
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
                target = "EnemyWindMagic";
                break;
            case ListenceTarget.Hero:
                target = "HeroWindMagic";
                break;
        }
        parentParticle = GetComponent<ParticleSystem>();
        parentParticle.Stop();
        EventManager.Register<Vector3>(target, PlayEffects);
    }

    private void PlayEffects(Vector3 pos)
    {
        transform.localPosition = pos;
        parentParticle.Play();
    }
}
