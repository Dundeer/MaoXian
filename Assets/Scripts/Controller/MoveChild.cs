using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveSpeedType
{
    /// <summary>
    /// 深色
    /// </summary>
    Dark,
    /// <summary>
    /// 浅色
    /// </summary>
    Light,
    /// <summary>
    /// 前方遮挡
    /// </summary>
    Grass
}

public class MoveChild : MonoBehaviour
{
    /// <summary>
    /// 当前移动速度类型
    /// </summary>
    public MoveSpeedType currentMoveSpeedType;
    /// <summary>
    /// 移动速度
    /// </summary>
    private float moveSpeed;
    /// <summary>
    /// 移动物体列表
    /// </summary>
    private readonly List<Transform> moveObject = new List<Transform>();
    /// <summary>
    /// 移动携程
    /// </summary>
    private Coroutine moveCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        switch (currentMoveSpeedType)
        {
            case MoveSpeedType.Dark:
                moveSpeed = DataBase.DARK_BG_SPEED;
                break;
            case MoveSpeedType.Light:
                moveSpeed = DataBase.LIGHT_BG_SPEED;
                break;
            case MoveSpeedType.Grass:
                moveSpeed = DataBase.GRASS_BG_SPEED;
                break;
        }
        for (int i = 0;i < transform.childCount; i++)
        {
            moveObject.Add(transform.GetChild(i));
        }
    }
    /// <summary>
    /// 开始移动携程
    /// </summary>
    public void StartMove()
    {
        StopMove();
        moveCoroutine = StartCoroutine(Move());
    }
    /// <summary>
    /// 结束移动携程
    /// </summary>
    public void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
           
    }

    IEnumerator Move()
    {
        float ScreenWidth = DataBase.SCREEN_WIDTH;
        while (true)
        {
            yield return null;
            for (int i = 0; i < moveObject.Count; i++)
            {
                Transform Child = moveObject[i];
                Vector2 childPos = Child.localPosition;
                if (childPos.x <= -ScreenWidth)
                {
                    Vector2 frontChildPos;
                    if (i == moveObject.Count - 1)
                    {
                        frontChildPos = moveObject[0].localPosition;
                    }
                    else
                    {
                        frontChildPos = moveObject[i + 1].localPosition;
                    }
                    childPos.x = frontChildPos.x + ScreenWidth;
                    Child.localPosition = childPos;
                }

                Child.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
            }
        }
        
    }
}
