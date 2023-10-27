using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class Utils
{
    public static void CheckNull(UnityEngine.Object obj, string message)
    {
        Debug.Assert(obj != null, message);
    }

    public static void CheckNull(object obj, string message)
    {
        Debug.Assert(obj != null, message);
    }


    // https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
    public static class JsonHelper
    {
        /// <summary>
        /// string -> Struct[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            //Debug.Log("json " + json);
            return wrapper.Data;
        }

        /// <summary>
        /// Struct[] -> string. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Data = array;
            return JsonUtility.ToJson(wrapper);
        }

        /// <summary>
        /// Struct[] -> string. pretty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Data = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        /// <summary>
        /// filePath -> Resources.Load<TextAsset> -> JsonHelper.FromJson<T> -> string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T[] Read<T>(string filePath)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);
            Debug.Log($"{filePath} : {File.Exists(Application.dataPath + "/Resources/" + filePath + ".json")}");

            if (textAsset == null)
                throw new Exception($"TextAsset Load Fail. {filePath}");

            if (textAsset.text == null || textAsset.text == string.Empty)
                throw new Exception($"TextAsset is Empty. {filePath}");

            T[] dataArray = FromJson<T>(textAsset.text);

            if (dataArray == null || dataArray.Length == 0)
                throw new Exception(
                    $"Fail to convert json to {typeof(T)}. {filePath}\n" +
                    $"Check {filePath} and [Serializable] and {typeof(T)}");

            return dataArray;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Data;
        }
    }

    public static T StringToEnum<T>(string e)
    {
        return (T)Enum.Parse(typeof(T), e);
    }

    public static class Calculate
    {
        public static int RealDamage(int damage, int def)
        {
            float reduceRate = damage / (float)(damage + def);
            float realDamage = damage * reduceRate;
            return (int)realDamage;
        }

        /// <summary>
        /// (atk, def, maxHp, maxMp)
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static (int, int, int, int) RealStatus(RPG.Common.StructStatus status)
        {
            float atk = 0.7f * status.Str;
            float def = 0.7f * status.End;
            float maxHp = 0.7f * status.Sta;
            float maxMp = 0.7f * status.Mag;
            return ((int)atk, (int)def, (int)maxHp, (int)maxMp);
        }

        public static (int, int, int, int) RealStatus(int str, int end, int sta, int mag)
        {
            float atk = 0.7f * str;
            float def = 0.7f * end;
            float maxHp = 0.7f * sta;
            float maxMp = 0.7f * mag;
            return ((int)atk, (int)def, (int)maxHp, (int)maxMp);
        }

        public static int Damage(int atk)
        {
            return atk;
        }
    }

    public static float NormalizeAngle180(float angle)
    {
        angle %= 360; // 각도를 360으로 나눈 나머지를 사용
        if (angle > 180)
        {
            angle -= 360;
        }
        else if (angle < -180)
        {
            angle += 360;
        }

        return angle;
    }

    public static float NormalizeAngle360(float angle)
    {
        angle %= 360f; // 각도를 360으로 나눠 나머지를 계산합니다.

        if (angle < 0f)
        {
            angle += 360f; // 각도가 음수이면 360을 더하여 0 이상 360 미만의 범위로 변환합니다.
        }

        return angle;
    }
}
