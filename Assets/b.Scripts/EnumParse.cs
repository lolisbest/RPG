using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace RPG.Utils
{
    public static class EnumParse
    {
        public static T StringToEnum<T>(string e)
        {
            return (T)Enum.Parse(typeof(T), e);
        }
    }

    public static class ReadJson
    {
        public static T[] Read<T>(string filePath)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            Debug.Log($"{filePath} : {File.Exists(Application.dataPath + "/Resources/" + filePath + ".json")}");
            if (textAsset == null)
                throw new Exception($"TextAsset Load Fail. {filePath}");

            if (textAsset.text == null || textAsset.text == string.Empty)
                throw new Exception($"TextAsset is Empty. {filePath}");

            T[] dataArray = JsonHelper.FromJson<T>(textAsset.text);

            if (dataArray == null || dataArray.Length == 0)
                throw new Exception(
                    $"Fail to convert json to {typeof(T)}. {filePath}\n" +
                    $"Check {filePath} and [Serializable] and {typeof(T)}");

            return dataArray;
        }
    }

    public static class WriteJson
    {
        public static bool Write<T>(T[] dataArray, string filePath)
        {
            string toSaveJsonString = JsonHelper.ToJson<T>(dataArray, true);
            File.WriteAllText(filePath, toSaveJsonString);
            return true;
        }
    }
}
