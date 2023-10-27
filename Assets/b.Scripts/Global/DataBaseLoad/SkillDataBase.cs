using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

public static partial class DataBase
{
    private class SkillDataBase : IDataLoad
    {
        public string DataFileName { get; private set; } = "SkillDataBase";
        // <Skill Id, Skill Data>
        public static Dictionary<int, StructSkillData> Skills { get; private set; }

        public void Load()
        {
            string filePath = DataRootDirPath + DataFileName;

            StructSkillData[] skillDataArray = Utils.JsonHelper.Read<StructSkillData>(filePath);

            Debug.Log("skillDataArray " + skillDataArray.Length);
            Debug.Log("skillDataArray " + skillDataArray[0]);

            for (int i = 0; i < skillDataArray.Length; i++)
            {
                StructSkillData skillData = skillDataArray[i];
                Debug.Log($"{skillData.Name} {string.Join(",", skillData.WeaponTypeStrings)}");
                for(int wTypeIndex = 0; wTypeIndex < skillData.WeaponTypeStrings.Length; wTypeIndex++)
                {
                    string weaponTypeString = skillData.WeaponTypeStrings[wTypeIndex];
                    EnumWeaponType weaponType = Utils.StringToEnum<EnumWeaponType>(weaponTypeString);
                    Debug.Log($"{skillData.Name} {weaponType}");
                    skillData.WeaponType |= weaponType;
                }

                skillData.Icon = Resources.Load<Sprite>(skillData.IconPath);
                Skills.Add(skillData.Id, skillData);

                Debug.Log(skillData.ToString());
            }

            Debug.Log($"Loaded {Skills.Count}/{skillDataArray.Length} of Skills from {DataFileName}");
        }

        public IDataLoad Initialize()
        {
            Skills = new();
            return this;
        }
    }
}