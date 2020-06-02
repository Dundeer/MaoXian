using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
//using LitJson;

namespace QFramework
{
    //public class JsonSet
    //{

    //    private static JsonSet instance = null;

    //    public static JsonSet Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //            {
    //                instance = new JsonSet();
    //            }
    //            return instance;
    //        }
    //    }

    //    private string jsonPath;

    //    public JsonSet()
    //    {
    //        jsonPath = Application.streamingAssetsPath + "/Level.json";
    //        LevelData levelData = new LevelData("Slice", 3);
    //        //WriteJson<LevelData>(jsonPath, levelData);
    //        //ReadJson(jsonPath);
    //    }

    //    public void WriteJson<T>(string path, List<T> contect)
    //    {
    //        if (!File.Exists(path))
    //        {
    //            File.Create(path).Dispose();
    //        }

    //        string json = JsonMapper.ToJson(contect);

    //        File.WriteAllText(path, json);

    //        Debug.Log("执行了书写json");
    //    }

    //    public List<T> ReadJson<T>(string path)
    //    {
    //        string json = File.ReadAllText(jsonPath);

    //        JsonData<T> jsondata = new JsonData<T>();
    //        jsondata.myJsonData = JsonMapper.ToObject<List<T>>(json);

    //        Debug.Log("读取了json");
    //        return jsondata.myJsonData;
    //    }

    //}

    //public class JsonData<T>
    //{
    //    public List<T> myJsonData;

    //    public JsonData()
    //    {
    //        myJsonData = new List<T>();
    //    }
    //}

}

