using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using QFramework;

public class GoldController : MonoBehaviour
{
    /// <summary>
    /// 金币图片
    /// </summary>
    public Sprite[] GoldSprite;
    /// <summary>
    /// 当前的刚体
    /// </summary>
    public Rigidbody2D currentRigi;
    /// <summary>
    /// 金币image
    /// </summary>
    private Image goldImage;
    /// <summary>
    /// 当前图片序号
    /// </summary>
    private int currentImageIndex = 0;

    void Start()
    {
        goldImage = transform.GetComponent<Image>();
        goldImage.sprite = GoldSprite[currentImageIndex];
    }

    public Custom RecycleGold(Vector3 pos)
    {
        Custom recycleCustom = new Custom();
        float delayTime = Random.Range(1.0f, 3.0f);
        StartCoroutine(Recycle(pos, delayTime, recycleCustom));
        return recycleCustom;
    }

    IEnumerator Recycle(Vector3 pos, float delayTime , Custom recycleCustom)
    {
        Destroy(transform.GetComponent<Rigidbody2D>());
        Destroy(transform.GetComponent<BoxCollider2D>());
        transform.DOLocalMove(pos, delayTime);
        transform.DOScale(new Vector3(0, 0, 0), delayTime);
        yield return new WaitForSeconds(delayTime);
        EventManager.Send("AddGold");
        recycleCustom.complete = true;
        Destroy(gameObject);
    }
}
