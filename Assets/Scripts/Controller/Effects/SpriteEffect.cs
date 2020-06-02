using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class SpriteEffect : FightEffect {

	public bool isPlaying = false;
	public ParticleSystem parentPs;
	public Sprite[] sprites;
	public float fps = 30;
	public float timeScale = 1; // 播放速度
	public bool loop = true;
	public int loopIndex = 0; // 循环起始位置
	public bool autoPlayer = true;
	public int sortingOrder = 1;
	public float startDelay = 0;

	bool lastParentPlaying = false;

	[ContextMenu("MyStart")]
	public override void MyStart ()
	{
		base.MyStart ();
		if (sprites == null || sprites.Length == 0) {
			Debug.LogError ("没有设置图片:" + name);
			return;
		}

		index = 0;
		isPlaying = true;
		sr.sortingOrder = sortingOrder;
		if(!sr.gameObject.activeSelf)
			sr.gameObject.SetActive(true);
		time = 0;
	}

	public override void MyStop ()
	{
		base.MyStop ();
		if (parentPs == null && Application.isPlaying) {
			if (OnMyDestroy != null)
				OnMyDestroy (this);
		}
		isPlaying = false;

		if (parentPs == null || !parentPs.isPaused) {
			if(sr.gameObject.activeSelf)
				sr.gameObject.SetActive(false);
		}
	}

    #if UNITY_EDITOR
    public Hashtable GetConfig()
	{
		var config = new Hashtable ();
		config ["scale"] = sr.transform.localScale.x;
		config ["posX"] = sr.transform.localPosition.x;
		config ["posY"] = sr.transform.localPosition.y;
		config ["posZ"] = sr.transform.localPosition.z;
		config ["order"] = sortingOrder;
		config ["loop"] = loop ? 1 : 0;
		config ["fps"] = fps;
		return config;
	}

	public void SetConfig(Hashtable config)
	{
		var pos = new Vector3 (
			config.GetVal<float>("posX"),
			config.GetVal<float>("posY"),
			config.GetVal<float>("posZ")
		);

		sr.transform.localScale = Vector3.one * config.GetVal<float> ("scale", 1);
		sr.transform.localPosition = pos;
		sortingOrder = config.GetVal<int> ("order");
		fps = config.GetVal<float> ("fps", fps);
		loop = (config.GetVal<int> ("loop", 1) == 1);
	}

	[ContextMenu("Apply")]
	public void Apply()
	{
        return;
		//UnityEditor.PrefabUtility.ReplacePrefab (
		//	gameObject, 
		//	UnityEditor.PrefabUtility.GetPrefabParent(gameObject), 
		//	UnityEditor.ReplacePrefabOptions.ConnectToPrefab
		//);
		//UnityEditor.PrefabUtility.prefabInstanceUpdated (gameObject);
	}
    #endif

    //===============================================
    public float index = 0; // 当前播放到第几帧
	private SpriteRenderer _sr;
	private SpriteRenderer sr
	{
		get{
			if(_sr == null)
				_sr = GetComponentInChildren<SpriteRenderer> (true);
			return _sr;
		}
	}

	void Start()
	{
		if (autoPlayer)
			MyStart ();
	}

	float time = 0;
	public override void Update()
	{
		base.Update ();

		if (parentPs != null) {
			if (lastParentPlaying != parentPs.isPlaying) {
				if (parentPs.isPlaying) {
					MyStart ();
				} else {
					MyStop ();
				}
				lastParentPlaying = parentPs.isPlaying;
			}
		}

		if (isPlaying) {
			#if UNITY_EDITOR
			float deltaTime;
			if(parentPs == null) {
				deltaTime = Time.deltaTime;
				time += deltaTime;
			} else {
				deltaTime = parentPs.time - time;
				time = parentPs.time;
			}
			index += deltaTime * fps * timeScale;
			#else
			time += Time.deltaTime;
			index += Time.deltaTime * fps * timeScale;
			#endif

			if(time < startDelay){
				if(sr.gameObject.activeSelf)
					sr.gameObject.SetActive(false);
				index = 0;
				return;
			}
			if(!sr.gameObject.activeSelf)
				sr.gameObject.SetActive(true);
			
			if (index >= sprites.Length) {
				if (loop) {
					index = loopIndex;
				} else {
					MyStop ();
					return;
				}
			} else if (index < 0) {
				if (loop) {
					index = sprites.Length - 0.01f;
				} else {
					MyStop ();
					return;
				}
			}
			if (Mathf.FloorToInt (index) < sprites.Length)
				sr.sprite = sprites [Mathf.FloorToInt (index)];
		}
	}
}
