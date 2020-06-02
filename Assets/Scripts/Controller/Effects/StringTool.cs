using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;

public static class StringTool 
{
    public static string FormatBytes(this long fileSize)
    {
        return FormatBytes((ulong)fileSize);
    }

    public static string FormatBytes(this ulong fileSize)
    {
        if (fileSize < 1024)
        {
            return fileSize + "B";
        }
        else if (fileSize < (1024 * 1024))
        {
            var temp = fileSize / 1024f;
            return temp.ToString("F2") + "KB";
        }
        else if (fileSize < (1024 * 1024 * 1024))
        {
            var temp = fileSize / (1024 * 1024f);
            return temp.ToString("F2") + "MB";
        }
        else
        {
            var temp = fileSize / (1024 * 1024 * 1024f);
            return temp.ToString("F2") + "GB";
        }
    }

    /// <summary>
    /// 检测是否含有屏蔽字
    /// </summary>
    /// <returns><c>true</c>, if pb was hased, <c>false</c> otherwise.</returns>
    /// <param name="str">String.</param>
    public static bool HasPB(this string str)
    {
        str = str.Replace(" ", "");
        for (int i = 0; i < PBTool.pbs.Length; i++)
        {
            if (PBTool.pbs[i].IsNullOrEmpty())
                continue;

            if (str.Contains(PBTool.pbs[i]))
                return true;
        }

        return false;
    }


    // 替换屏蔽字
    public static string ReplacePB(this string str, string replaceStr = "**")
    {
        for (int i = 0; i < PBTool.pbs.Length; i++)
        {
            if (PBTool.pbs[i].IsNullOrEmpty())
                continue;
            
            str = str.Replace(PBTool.pbs[i], replaceStr);
        }

        return str;
    }

	/// <summary>
	/// \uXXXX转中文
	/// </summary>
	/// <returns>The G b2312.</returns>
	/// <param name="str">String.</param>
	public static string ToGB2312(this string str)
	{
		var mc = Regex.Matches(str, @"\\u([\w]{2})([\w]{2})", (RegexOptions)(8 | 1));
		var bts = new byte[2];
		foreach(Match m in mc )
		{
			bts[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
			bts[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
			var s = Encoding.Unicode.GetString(bts);
			str = str.Replace (m.Value, s);
		}
		return str;
	}

	/// <summary>
	/// 删除字符串尾
	/// </summary>
	/// <returns></returns>
	/// <param name="str">String.</param>
	/// <param name="end">End.</param>
	public static string CzfTrimEnd(this string str, string end)
	{
		var length = str.Length;
		if (length > end.Length) {
			var sub = str.Substring (length - end.Length, end.Length);
			if (sub == end)
				return str.Substring (0, length - end.Length);
		}
		return str;
	}

    // 删除字符串首尾的括号
    public static string DelBrackets(this string str)
    {
        // 快速判断提升效率
        if (str[0] != '(' || str[str.Length - 1] != ')')
            return str;

        var ret = Regex.Match(str, @"(?<=^\()((?<Open>\()|(?<-Open>\))|[^()])*(?(Open)(?!))(?=\)$)");
        if (ret.Success)
        {
            return ret.Value;
        }
        return str;
    }

	public static string CzfFormat(this string str, params object[] args)
	{
		if (args != null && args.Length > 0) {
			for (int i = 0; i < args.Length; i++) {
				str = str.Replace ("{" + i + "}", args [i].ToString());
			}
		}

		return str;
	}

	public static bool IsNullOrEmpty(this string str)
	{
        if (string.IsNullOrEmpty(str))
            return true;

        //str = str.Trim();

        //if (str == string.Empty)
        //    return true;

        //if (str.Length == 0)
        //    return true;

        return false;
	}

    public static string GetParamsString(this string[] list, int index)
    {
        if (list.Length < index)
        {
            throw new System.Exception("格式错误 index=" + index + " :" + list.ToJson());
        }
        return list[index];
    }

    public static int GetParamsInt(this string[] list, int index)
    {
        if (list.Length < index)
        {
            throw new System.Exception("格式错误 index="+ index +" :" + list.ToJson());
        }

        int ret;
        if (!int.TryParse(list[index], out ret))
            throw new System.Exception("格式错误 index=" + index + " :" + list.ToJson());

        return ret;
    }

    public static float GetParamsFloat(this string[] list, int index)
    {
        if (list.Length < index)
        {
            throw new System.Exception("格式错误 index=" + index + " :" + list.ToJson());
        }

        float ret;
        if (!float.TryParse(list[index], out ret))
            throw new System.Exception("格式错误 index=" + index + " :" + list.ToJson());

        return ret;
    }

    public static string GetMD5(this string sDataIn)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] bytValue, bytHash;
        bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
        bytHash = md5.ComputeHash(bytValue);
        md5.Clear();
        string sTemp = "";
        for (int i = 0; i < bytHash.Length; i++)
        {
            sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
        }
        return sTemp.ToLower();
    }
}
