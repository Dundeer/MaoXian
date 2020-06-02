using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBase
{
    private static DataBase instance = null;

    public static DataBase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataBase();
            }
            return instance;
        }
    }
    /// <summary>
    /// 屏幕宽度
    /// </summary>
    public static int SCREEN_WIDTH = 1788;
    /// <summary>
    /// 屏幕高度
    /// </summary>
    public static int SCREEN_HEIGHT = 1000;
    /// <summary>
    /// 深色背景移动速度
    /// </summary>
    public static float DARK_BG_SPEED = 2;
    /// <summary>
    /// 浅色背景移动速度
    /// </summary>
    public static float LIGHT_BG_SPEED = 3;
    /// <summary>
    /// 前方遮挡移动速度
    /// </summary>
    public static float GRASS_BG_SPEED = 2;
    /// <summary>
    /// 产生敌人的间隔
    /// </summary>
    public static float DELAY_CREATE_TIME = 5.0f;


    public DataBase()
    {

    }



}
