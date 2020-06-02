using UnityEngine;

public class PaysticController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem particle = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = particle.main;
        mainModule.loop = false;
        mainModule.stopAction = ParticleSystemStopAction.Callback;
        particle.Play();
    }

    private void OnParticleSystemStopped()
    {
        Destroy(gameObject);
    }
}
