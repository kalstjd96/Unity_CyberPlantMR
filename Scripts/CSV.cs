using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class CSV
{
    //static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string SPLIT_RE = @"@";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };
    //key : Column
    //value : value
    public static List<Dictionary<string, string>> Read(string file)
    {
        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
        TextAsset data = Resources.Load(file) as TextAsset;
        string[] lines = Regex.Split(data.text, LINE_SPLIT_RE);
        if (lines.Length <= 1) return list;
        string[] header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;
            Dictionary<string, string> entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                entry[header[j]] = value;
            }
            list.Add(entry);
        }
        return list;

    }
}
