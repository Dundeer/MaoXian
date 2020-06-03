using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodLine : MonoBehaviour
{
    /// <summary>
    /// 血条绿色部分
    /// </summary>
    public Image GreenBlood;
    /// <summary>
    /// 血条slider
    /// </summary>
    public Slider BloodSlider;
    /// <summary>
    /// 血量动画
    /// </summary>
    public RectTransform BloodAnim;
    /// <summary>
    /// 最大血量
    /// </summary>
    private float MaxBlood;
    /// <summary>
    /// 当前血量
    /// </summary>
    private float CurrentBlood;
    /// <summary>
    /// 血量动画携程
    /// </summary>
    private Coroutine BloodAnimCoroutine;
    /// <summary>
    /// 血量动画速度
    /// </summary>
    private float BloodAnimSpeed;

    /// <summary>
    /// 初始化现有
    /// </summary>
    /// <param name="blood"></param>
    public void IniteBlood(float blood)
    {
        CurrentBlood = MaxBlood = blood;
        GreenBlood.color = new Color(0, 1, 41 / 255, 1);
        BloodSlider.value = 1;
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
        BloodSlider.value = currentScale;
        GreenBlood.color = new Color(0, 1, 41 / 255, currentScale);
        StartBloodAnim(blood);
    }

    /// <summary>
    /// 开启减少血量动画
    /// </summary>
    /// <param name="blood">需要减少的血量</param>
    private void StartBloodAnim(float blood)
    {
        float cutBloodScale = blood / MaxBlood;
        BloodAnimSpeed = (cutBloodScale * 300) / 1.0f;
        if (BloodAnimCoroutine != null) return;
        BloodAnimCoroutine = StartCoroutine(BloodAnimFunc());
    }

    /// <summary>
    /// 血量减少动画携程
    /// </summary>
    /// <returns></returns>
    IEnumerator BloodAnimFunc()
    {
        while (true)
        {
            yield return null;
            float bloodAnimWidth = BloodAnim.rect.width;
            bloodAnimWidth -= BloodAnimSpeed * Time.deltaTime;
            BloodAnim.sizeDelta = new Vector2(bloodAnimWidth, 0);
            BloodAnim.localPosition = new Vector2(-(150 - (bloodAnimWidth / 2)), 0.0f);
            if(bloodAnimWidth <= 300 * (CurrentBlood / MaxBlood))
            {
                BloodAnimCoroutine = null;
                yield break;
            }
        }
    }

    /// <summary>
    /// 增加血量
    /// </summary>
    /// <param name="addNumber">血量数字</param>
    public void AddBlood(float addNumber)
    {
        CurrentBlood += addNumber;
        float bloodScale = CurrentBlood / MaxBlood;
        BloodSlider.value = bloodScale;
        GreenBlood.color = new Color(0, 1, 41 / 255, bloodScale);
        float bloodAnimWidth = 300 * bloodScale;
        BloodAnim.sizeDelta = new Vector2(bloodAnimWidth, 0);
        BloodAnim.localPosition = new Vector2(-(150 - (bloodAnimWidth / 2)), 0.0f);
    }
}
