using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class FightEffect : CzfMonoBehaviour
{
	public CzfEvent OnMyStart;
	public CzfEvent<FightEffect> OnMyDestroy;

    [NonSerialized]
    public bool effectActive;
    [NonSerialized]
    public bool effectInverted = false;

	public virtual void Awake()
    {
        
    }

    public virtual void MyStart()
	{
        OnMyStart?.Invoke();
        effectActive = true;
        effectInverted = false;
    }

    public virtual void MyStartInverted()
    {
        throw new Exception("未实现");
    }

	public virtual void Update()
	{
		
	}

    // 获取效果持续时间
    public virtual float GetDuration()
    {
        throw new Exception("未实现");
    }

    // 获取效果持续时间
    public virtual void SetOrderLayer(int orderLayer)
    {
        throw new Exception("未实现");
    }

	public virtual void MyStop()
	{
        effectActive = false;
    }

    public void StartHidden()
    {
        StartCoroutine(StartHiddenTask());
    }

    IEnumerator StartHiddenTask()
    {
        var child = transform.GetChild(0);
        var scale = child.localScale;
        child.localScale = Vector3.zero;
        yield return null;
        yield return null;
        child.localScale = scale;
    }
}