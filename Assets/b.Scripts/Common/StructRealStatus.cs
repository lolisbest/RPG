using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Common
{
    public struct StructRealStatus
    {
        public int Atk;
        public int Def;
        public int MaxHp;
        public int MaxMp;

        public int Str;
        public int End;
        public int Sta;
        public int Mag;

        public StructRealStatus(
            int str, int end, int sta, int mag,
            int atk, int def, int maxHp, int maxMp
        )
        {
            Str = str;
            End = end;
            Sta = sta;
            Mag = mag;

            Atk = atk;
            Def = def;
            MaxHp = maxHp;
            MaxMp = maxMp;
        }
    }
}