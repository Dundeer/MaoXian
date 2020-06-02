using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public enum AppMode
{
    Developing,
    QA,
    Release
}

public class App : QMonoSingleton<App>
{

    public AppMode mode = AppMode.Developing;

    private App()
    {
        
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        instance = this;

        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ApplicationDidFinishLaunching());
    }

    IEnumerator ApplicationDidFinishLaunching()
    {

        if(App.Instance.mode == AppMode.Developing)
        {

        }
        else
        {
            
        }

        yield return null;
    }

    public void OnGUI()
    {
        
    }

    public void OnDestroy()
    {
        
    }
}
