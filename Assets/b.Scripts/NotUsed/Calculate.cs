//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Common;

//namespace RPG.Utils
//{
//    public static class Calculate
//    {
//        public static int RealDamage(int damage, int def)
//        {
//            float reduceRate = damage / (float)(damage + def);
//            float realDamage = damage * reduceRate;
//            return (int)realDamage;
//        }

//        /// <summary>
//        /// (atk, def, maxHp, maxMp)
//        /// </summary>
//        /// <param name="status"></param>
//        /// <returns></returns>
//        public static (int, int, int, int) RealStatus(StructStatus status)
//        {
//            float atk = 0.7f * status.Str;
//            float def = 0.7f * status.End;
//            float maxHp = 0.7f * status.Sta;
//            float maxMp = 0.7f * status.Mag;
//            return ((int)atk, (int)def, (int)maxHp, (int)maxMp);
//        }

//        public static (int, int, int, int) RealStatus(int str, int end, int sta, int mag)
//        {
//            float atk = 0.7f * str;
//            float def = 0.7f * end;
//            float maxHp = 0.7f * sta;
//            float maxMp = 0.7f * mag;
//            return ((int)atk, (int)def, (int)maxHp, (int)maxMp);
//        }

//        public static int Damage(int atk)
//        {
//            return atk;
//        }

//    }
//}