using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBloodLine : MonoBehaviour
{
    /// <summary>
    /// 血条绿色部分
    /// </summary>
    public Image greenBlood;
    /// <summary>
    /// 血条slider
    /// </summary>
    public Slider bloodSlider;
    /// <summary>
    /// 最大血量
    /// </summary>
    private float MaxBlood;
    /// <summary>
    /// 当前血量
    /// </summary>
    private float CurrentBlood;
    /// <summary>
    /// 初始化现有
    /// </summary>
    /// <param name="blood"></param>
    public void IniteBlood(float blood)
    {
        CurrentBlood = MaxBlood = blood;
        greenBlood.color = new Color(0, 1, 41 / 255, 1);
        bloodSlider.value = 1;
    }
    /// <summary>
    /// 减少血量
    /// </summary>
    /// <param name="blood">需要减少的血量</param>
    public void CutBlood(float blood)
    {
        CurrentBlood -= blood;
        float currentScale = CurrentBlood / MaxBlood;
        if (currentScale < 0) currentScale = 0;
        bloodSlider.value = currentScale;
        greenBlood.color = new Color(0, 1, 41 / 255, currentScale);
    }
}
