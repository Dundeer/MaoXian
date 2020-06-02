using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;

public static class HashtableTool
{
    // 删除指定节点并且返回下一个节点
    public static LinkedListNode<T> RemoveGetNext<T>(this LinkedList<T> list, LinkedListNode<T> node)
    {
        var ret = node.Next;
        list.Remove(node);
        return ret;
    }

    public static void AddRange<T>(this HashSet<T> hs, IEnumerable<T> hs2)
    {
        foreach (var item in hs2)
        {
            hs.Add(item);
        }
    }

    public static Hashtable ToHashtable(this object[] objs)
    {
        if (objs == null)
        {
            return null;
        }

        var ret = new Hashtable(objs.Length);
        for (int i = 0; i < objs.Length; i++)
        {
            ret[i] = objs[i];
        }

        return ret;
    }

    public static bool HasSame<T>(this HashSet<T> hs1, HashSet<T> hs2)
    {
        if (hs1 == null || hs2 == null || hs1.Count == 0 || hs2.Count == 0)
            return false;
        if (hs1 == hs2)
            return true;

        foreach (var item in hs1)
        {
            if (hs2.Contains(item))
                return true;
        }

        return false;
    }

    private static IList ToList(this object obj, Type type)
    {
        if (obj == null)
        {
            return null;
        }

        if (obj.GetType().Equals(type) || obj.GetType().IsSubclassOf(type))
        {
            return (IList)obj;
        }

        if (obj is object[] objs)
        {
            var list = (IList)Activator.CreateInstance(type, false);

            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(List<>)))
            {
                var t = type.GetGenericArguments()[0];
                for (int i = 0; i < objs.Length; i++)
                {
                    if (t.IsEnum)
                    {
                        list.Add(System.Enum.Parse(t, objs[i].ToString()));
                    }
                    else if (t.Equals(typeof(Vector2)))
                    {
                        object[] array = (object[])objs[i];
                        list.Add(new Vector2((float)array[0], (float)array[1]));
                    }
                    else if (t.Equals(typeof(Vector3)))
                    {
                        object[] array = (object[])objs[i];
                        list.Add(new Vector3((float)array[0], (float)array[1], (float)array[2]));
                    }
                    else
                    {
                        list.Add(Convert.ChangeType(objs[i], t));
                    }
                }
            }
            else
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    list.Add(objs[i]);
                }
            }

            return list;
        }
        else
        {
            throw new Exception("错误的类型转换:" + obj.ToString());
        }
    }

    public static List<T> ToList<T>(this object obj)
    {
        if (obj == null)
        {
            return null;
        }

        if (obj is List<T>)
        {
            return (List<T>)obj;
        }

        if (obj is object[] objs)
        {
            var list = new List<T>(objs.Length);
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is T)
                    list.Add((T)objs[i]);
                else
                    list.Add((T)Convert.ChangeType(objs[i], typeof(T)));
            }
            return list;
        }
        else if (obj is T[] ts)
        {
            return new List<T>(ts);
        }
        else if (obj is IEnumerable enumerable)
        {
            List<T> lists = new List<T>();
            foreach (var item in enumerable)
            {
                if (item is T)
                {
                    lists.Add((T)item);
                }
            }
            return lists;
        }
        else
        {
            throw new Exception("错误的类型转换:" + obj.ToString());
        }
    }

    public static Dictionary<T1, T2> ToDict2<T1, T2>(this object obj, string idKey = "id") where T2 : Hashtable
    {
        if (obj == null)
        {
            Debug.LogError("Error Load ToDict2");
            return new Dictionary<T1, T2>();
        }

        if (obj is object[] objs)
        {
            var ret = new Dictionary<T1, T2>(objs.Length);
            for (int i = 0; i < objs.Length; i++)
            {
                var ht = (Hashtable)objs[i];
                var key = ht.GetVal<T1>(idKey);
                ret[key] = (T2)ht;
            }
            return ret;
        }
        else
        {
            throw new Exception("错误的类型转换:" + obj.ToString());
        }
    }

    private static IList GetValList(this Hashtable ht, Type type, object key, IList def = default)
    {
        if (ht.ContainsKey(key))
        {
            try
            {
                if (ht[key] != null)
                    return ht[key].ToList(type);
                return def;
            }
            catch (Exception e)
            {
                Debug.LogError("Error Convert:" + type.ToString() + "|" + key + "|" + ht.ToJson());
                throw e;
            }
        }
        else
        {
            if (key is long k)
            {
                if (k >= int.MinValue && k <= int.MaxValue)
                {
                    return ht.GetValList(type, (int)k, def);
                }
            }
            else if (key is int k1)
            {
                if (k1 >= byte.MinValue && k1 <= byte.MaxValue)
                {
                    return ht.GetValList(type, (byte)k1, def);
                }
            }
            return def;
        }
    }

    public static List<T> GetValList<T>(this Hashtable ht, object key, List<T> def = default)
    {
        if (ht.ContainsKey(key))
        {
            try
            {
                return ht[key].ToList<T>();
            }
            catch (Exception e)
            {
                Debug.LogError("Error Convert:" + typeof(T).ToString() + "|" + key + "|" + ht.ToJson());
                throw e;
            }
        }
        else
        {
            if (key is long k)
            {
                if (k >= int.MinValue && k <= int.MaxValue)
                {
                    return ht.GetValList<T>((int)k, def);
                }
            }
            else if (key is int k1)
            {
                if (k1 >= byte.MinValue && k1 <= byte.MaxValue)
                {
                    return ht.GetValList<T>((byte)k1, def);
                }
            }
            return def;
        }
    }

    public static List<float> ToFList(this Vector3 vec)
    {
        return new List<float>(new float[] { vec.x, vec.y, vec.z });
    }

    public static List<float> ToFList(this Vector2 vec)
    {
        return new List<float>(new float[] { vec.x, vec.y });
    }

    public static object GetVal2(this Hashtable ht, Type type, object key, object def = null)
    {
        if (type == typeof(Vector3))
        {
            var vals = ht.GetValList<float>(key);
            return new Vector3(vals[0], vals[1], vals[2]);
        }
        else if (type == typeof(Vector2))
        {
            var vals = ht.GetValList<float>(key);
            return new Vector2(vals[0], vals[1]);
        }
        else if (ht.ContainsKey(key))
        {
            try
            {
                if (typeof(IList).IsAssignableFrom(type))
                {
                    return ht.GetValList(type, key);
                }

                if (type.IsEnum)
                {
                    return Convert.ChangeType(ht[key], typeof(int));
                }

                return Convert.ChangeType(ht[key], type);
            }
            catch (Exception e)
            {
                Debug.LogError("Error Convert:" + type.ToString() + "|" + key + "|" + ht.ToJson());
                throw e;
            }
        }
        else
        {
            if (key is long k)
            {
                if (k >= int.MinValue && k <= int.MaxValue)
                {
                    return ht.GetVal2(type, (int)k, def);
                }
            }
            else if (key is int k1)
            {
                if (k1 >= byte.MinValue && k1 <= byte.MaxValue)
                {
                    return ht.GetVal2(type, (byte)k1, def);
                }
            }
            return def;
        }
    }

    public static object ChangeType(object val, Type type)
    {
        if (val == null)
            return val;

        try
        {
            if (val.GetType() == type)
                return val;

            if (typeof(IList).IsAssignableFrom(type))
            {
                return val.ToList(type);
            }

            if (type.IsEnum)
            {
                return Convert.ChangeType(val, typeof(int));
            }

            return Convert.ChangeType(val, type);
        }
        catch (Exception e)
        {
            Debug.LogError("Error Convert:" + type.ToString() + "|" + val.ToJson());
            throw e;
        }
    }

    public static object ToSerialize(this object val)
    {
        if (val is Vector3)
            return ((Vector3)val).ToFList();
        else if (val is Vector2)
            return ((Vector2)val).ToFList();
        else if (val.GetType().IsEnum)
            return (int)val;
        else
            return val;
    }

    public static bool IsNullOrEmpty(this Hashtable ht)
    {
        return ht == null || ht.Count == 0;
    }

    public static void HtTo2List(this Hashtable ht, out string[] keys, out object[] vals)
    {
        if(ht.IsNullOrEmpty())
        {
            keys = null;
            vals = null;
            return;
        }

        keys = new string[ht.Count];
        vals = new object[ht.Count];

        int index = 0;
        foreach (string key in ht.Keys)
        {
            keys[index] = key;
            vals[index] = ht[key];
            index++;
        }
    }

    public static Vector3 GetVector3(this Hashtable ht, object key, Vector3 def = default)
    {
        Vector3 defaultV3 = (Vector3)(object)def;
        var vals = ht.GetValList<float>(key, new List<float>(new float[] { defaultV3.x, defaultV3.y, defaultV3.z }));
        return new Vector3(vals[0], vals[1], vals[2]);
    }

    public static Vector2 GetVector2(this Hashtable ht, object key, Vector2 def = default)
    {
        Vector2 defaultV2 = (Vector2)(object)def;
        var vals = ht.GetValList<float>(key, new List<float>(new float[] { defaultV2.x, defaultV2.y }));
        return new Vector2(vals[0], vals[1]);
    }

    public static T GetVal<T>(this Hashtable ht, object key, T def = default)
    {
        var val = ht[key];
        if (val != null)
        {
            try
            {
                if (val is T t)
                    return t;

                try
                {
                    if (val == DBNull.Value)
                        return def;

                    if (typeof(T).IsEnum)
                    {
                        return (T)Convert.ChangeType(ht[key], typeof(int));
                    }
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                catch (Exception e1)
                {
#if UNITY_EDITOR
                    Debug.LogError($"Error Convert from {val.GetType()} to {typeof(T)}:\n" + e1.ToString());
#endif
                    return def;
                }
            }
            catch (Exception e)
            {
                if (typeof(T).IsSubclassOf(typeof(IList)))
                    Debug.LogError("IList Error Convert:" + typeof(T).ToString() + "|" + key + "|" + ht.ToJson());
                
                else
                    Debug.LogError("Error Convert:" + typeof(T).ToString() + "|" + key + "|" + ht.ToJson());
                throw e;
            }
        }
        else
        {
            if (key is int k1)
            {
                if (k1 >= byte.MinValue && k1 <= byte.MaxValue)
                {
                    return ht.GetVal<T>((byte)k1, def);
                }
            }
            else if (key is long k)
            {
                if (k >= int.MinValue && k <= int.MaxValue)
                {
                    return ht.GetVal<T>((int)k, def);
                }
            }
            return def;
        }
    }

    static HashtableTool()
    {
        LitJson.ImporterFunc<double, Single> double2float = new LitJson.ImporterFunc<double, float>(HashtableTool.Double2float);
        LitJson.JsonMapper.RegisterImporter<double, float>(double2float);

        LitJson.ExporterFunc<float> float2double = new LitJson.ExporterFunc<float>(HashtableTool.Float2double);
        LitJson.JsonMapper.RegisterExporter<float>(float2double);

        LitJson.ExporterFunc<DBNull> dbNull2Null = new LitJson.ExporterFunc<DBNull>(HashtableTool.DBNull2Null);
        LitJson.JsonMapper.RegisterExporter<DBNull>(dbNull2Null);

        //获取unity的默认类型
        Type[] types = typeof(Vector2).Assembly.GetTypes();
        string[] splitStr = new string[] { "." };
        foreach (var type in types)
        {
            string fullName = type.FullName;
            unityDefaultTypeDic[type.FullName] = type;
            string[] spilits = fullName.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            if (spilits.Length == 2 && spilits[0].Equals("UnityEngine"))
            {
                unityDefaultTypeDic[spilits[1]] = type;
            }
        }
    }

    private static void DBNull2Null(DBNull dbNull, LitJson.JsonWriter jw)
    {
        jw.Write(null);
    }

    private static void Float2double(float value, LitJson.JsonWriter jw)
    {
        jw.Write(value);
    }

    private static float Double2float(double value)
    {
        return (float)value;
    }

    public static string ToLitJson(this object obj)
    {

        var w = new LitJson.JsonWriter
        {
            PrettyPrint = false
        };
        LitJson.JsonMapper.ToJson(obj, w);
        w.TextWriter.Flush();
        return w.TextWriter.ToString();
    }

    public static T JsonToObj<T>(this string json)
    {
        return LitJson.JsonMapper.ToObject<T>(json);
    }

    public static Hashtable JsonToHashtable(this string json)
    {
        try
        {
            var reader = LitJson.JsonMapper.ToObject(json);
            return (Hashtable)ReadJsonData(reader);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return null;
    }

    public static object JsonToObject(this string json)
    {
        try
        {
            var reader = LitJson.JsonMapper.ToObject(json);
            return ReadJsonData(reader);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return null;
    }

    public static List<Hashtable> JsonToListHt(this string json)
    {
        try
        {
            var reader = LitJson.JsonMapper.ToObject(json);
            var readObj = ReadJsonData(reader);
            if(readObj is object[] arr)
            {
                var ret = new List<Hashtable>();
                foreach (var item in arr)
                {
                    if (item is Hashtable)
                        ret.Add((Hashtable)item);
                }
                return ret;
            }
            else if(readObj is Hashtable ht)
            {
                var ret = new List<Hashtable>();
                foreach (var item in ht.Values)
                {
                    if (item is Hashtable)
                        ret.Add((Hashtable)item);
                }
                return ret;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"不支持的类型转换,类型:{readObj.GetType()}, 内容:{readObj.ToJson()}\njson:{json}");
#endif
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return null;
    }

    static object ReadJsonData(LitJson.JsonData val)
    {
        if (val == null)
            return null;
        else if (val.IsDouble)
            return Convert.ToSingle((double)val);
        else if (val.IsString)
            return (string)val;
        else if (val.IsInt)
            return (int)val;
        else if (val.IsArray)
        {
            var list = (IList)val;
            var objs = new object[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                objs[i] = ReadJsonData((LitJson.JsonData)list[i]);
            }
            return objs;
        }
        else if (val.IsBoolean)
        {
            return (bool)val;
        }
        else if (val.IsObject)
        {
            var dict = (IDictionary)val;
            var ht = new Hashtable();
            foreach (var k in dict.Keys)
            {
                ht[k] = ReadJsonData((LitJson.JsonData)dict[k]);
            }

            return ht;
        }
        else
        {
            throw new Exception("未处理的类型:" + val);
        }
    }

    public static float GetFloat(this LitJson.JsonData jsonData)
    {
        if (jsonData.IsInt)
            return Convert.ToSingle((int)jsonData);
        else if (jsonData.IsDouble)
            return Convert.ToSingle((double)jsonData);
        else
        {
            Debug.LogError("未处理的类型:{0}|{1}|{2}|{3}|{4}|{5}|{6}".CzfFormat(
                jsonData,
                jsonData.IsInt,
                jsonData.IsDouble,
                jsonData.IsString,
                jsonData.IsArray,
                jsonData.IsBoolean,
                jsonData.IsLong
            ));
            return 0;
        }
    }

    public static string ToJson(this object obj, int space = 0)
    {
        if (obj == null)
        {
            return "null";
        }
        else if (obj is string)
        {
            return "\"" + obj.ToString() + "\"";
        }
        else if (obj is Hashtable ht)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetSpace(space) + "{");
            if (ht.Count > 0)
            {
                foreach (var k in ht.Keys)
                {
                    sb.AppendLine(GetSpace(space + 4) + k.ToJson() + ":" + ht[k].ToJson(space + 4) + ",");
                }
                sb.Length -= 2;
                sb.AppendLine();
            }
            sb.AppendLine(GetSpace(space) + "}");
            return sb.ToString();
        }
        else if (obj is IDictionary dict)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetSpace(space) + "{");
            foreach (var k in dict.Keys)
            {
                sb.AppendLine(GetSpace(space + 4) + k.ToJson() + ":" + dict[k].ToJson(space + 4));
            }
            sb.AppendLine(GetSpace(space) + "}");
            return sb.ToString();
        }
        else if (obj is IList list)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetSpace(space) + "[");
            if (list.Count > 0)
            {
                foreach (var v in list)
                {
                    sb.AppendLine(GetSpace(space + 4) + v.ToJson() + ",");
                }
                sb.Length -= 2;
            }
            sb.AppendLine();
            sb.AppendLine(GetSpace(space) + "]");
            return sb.ToString();
        }
        else if (obj is IEnumerable)
        {
            StringBuilder sb = new StringBuilder();
            var ienumerable = (IEnumerable)obj;
            sb.AppendLine(GetSpace(space) + "[");
            foreach (var item in ienumerable)
            {
                sb.AppendLine(GetSpace(space + 4) + item.ToJson() + ",");
            }
            sb.AppendLine();
            sb.AppendLine(GetSpace(space) + "]");
            return sb.ToString();
        }
        else if (obj is DateTime)
        {
            return "\"" + obj.ToString() + "\"";
        }
        else
        {
            return obj.ToString();
        }
    }

    public static string GetSpace(int count)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            sb.Append(" ");
        }
        return sb.ToString();
    }

    public static Hashtable Merge(this Hashtable oldT, Hashtable newT)
    {
        foreach (var k in newT.Keys)
        {
            oldT[k] = newT[k];
        }
        return oldT;
    }

    public static Hashtable HtClone(this Hashtable ht)
    {
        var ret = new Hashtable();
        foreach (var k in ht.Keys)
        {
            ret[k] = ht[k];
        }
        return ret;
    }

    public static List<T> Clone<T>(this List<T> list)
    {
        if (list == null)
            list = new List<T>();
        var ret = new List<T>(list.Count);

        if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
        {
            foreach (ICloneable item in list)
            {
                ret.Add((T)item.Clone());
            }
        }
        else
        {
            ret.AddRange(list);
        }
        return ret;
    }

    public static HashSet<T> Clone<T>(this HashSet<T> hashset)
    {
        var ret = new HashSet<T>();
        foreach (var item in hashset)
        {
            ret.Add(item);
        }
        return ret;
    }


    public static bool IsSubGenericType(this Type type, Type targetType, Type stopType)
    {
        if (type == null)
            return false;
        bool result = false;
        Type tempType = type;
        while (tempType != null)
        {
            if (!tempType.IsGenericType)
                tempType = tempType.BaseType;
            else
            {
                result = tempType.GetGenericTypeDefinition().Equals(targetType);
                if (!result)
                {
                    tempType = tempType.BaseType;
                }
                else
                {
                    tempType = null;
                }
            }
            if (tempType != null && tempType.Equals(stopType))
                tempType = null;
        }
        return result;
    }

    public static Type[] GetSubGenericType(this Type type, Type targetType, Type stopType)
    {
        if (type == null)
            return null;
        bool result;
        Type tempType = type;
        Type checkType = null;
        while (tempType != null)
        {
            if (!tempType.IsGenericType)
                tempType = tempType.BaseType;
            else
            {
                result = tempType.GetGenericTypeDefinition().Equals(targetType);
                if (!result)
                {
                    tempType = tempType.BaseType;
                }
                else
                {
                    checkType = tempType;
                    tempType = null;
                }
            }
            if (tempType != null && tempType.Equals(stopType))
                tempType = null;
        }
        if (checkType != null)
            return checkType.GetGenericArguments();
        return null;
    }

    public static Dictionary<string, Type> unityDefaultTypeDic = new Dictionary<string, Type>();

    public static Type StringToType(this string typeStr)
    {
        Type defaultType;
        if (unityDefaultTypeDic.TryGetValue(typeStr, out defaultType))
            return defaultType;
        if (typeStr.Contains("<") && typeStr.Contains(">"))
        {
            int startIndex = typeStr.IndexOf("<");
            string genTypesStr = typeStr.Substring(0, startIndex);
            int endIndex = typeStr.IndexOf(">");
            string genStrings = typeStr.Substring(startIndex + 1, endIndex - startIndex - 1);
            string[] genArray = genStrings.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            Type[] genTypes = new Type[genArray.Length];
            for (int i = 0; i < genArray.Length; i++)
            {
                genTypes[i] = Type.GetType(genArray[i]);
                if (genTypes[i] == null)
                {
                    genTypes[i] = unityDefaultTypeDic[genArray[i]];
                }
            }
            Type type = Type.GetType(genTypesStr + "`" + genTypes.Length);
            return type.MakeGenericType(genTypes);
        }
        return Type.GetType(typeStr);
    }

    // 打乱List
    public static void Shuffle<T>(this List<T> list) {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }
}
