using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Item;
using RPG.Common;

using ItemId = System.Int32;


public static partial class DataBase
{
    private class ItemDataBase : IDataLoad
    {
        public Color[] GradeColors { get; private set; }

        public string DataFileName { get; private set; } = "ItemDataBase";
        public static Dictionary<ItemId, StructItemData> Items { get; private set; }
        // 아이템 랜덤 생성에서 사용할 ItemId 목록
        private static List<int> _itemIds;

        /// <summary>
        /// ItemDataBase.json을 읽어 들이기
        /// </summary>
        public void Load()
        {
            LoadGradeColor();

            if (Items == null || _itemIds == null)
                throw new System.Exception("초기화 되지 않은 Items or _itemIds");

            string filePath = DataRootDirPath + DataFileName;
            StructItemData[] itemDataArray = Utils.JsonHelper.Read<StructItemData>(filePath);
            for(int i = 0; i < itemDataArray.Length; i++)
            {
                StructItemData itemData = itemDataArray[i];
                //Debug.Log(itemData.ToString());
                itemData.LoadSprite();
                itemData.SetType();
                itemData.GradeColor = GradeColors[itemData.Grade];
                itemData.SellPrice = itemData.SellPrice == 0 ? (int)(itemData.BuyPrice * 0.7f) : itemData.SellPrice;
                //Debug.Log($"{itemData.Name} {itemData.SellPrice}");

                try
                {
                    Items.Add(itemData.Id, itemData);

                }
                catch (System.ArgumentException e)
                {
                    Debug.Log(string.Join(",", Items.Keys));
                    throw new System.Exception(e.Message);
                }
            }
            Debug.Log($"Loaded {Items.Count}/{itemDataArray.Length} of Items from {DataFileName}");
        }

        /// <summary>
        /// Items, _itemIds 초기화
        /// </summary>
        public IDataLoad Initialize()
        {
            Items = new();
            _itemIds = new();
            return this;
        }

        //public int GenerateRandomItem()
        //{
        //    int index = Item.Rand.Next(Items.Count);
        //    int id = _itemIds[index];
        //    return id
        //}

        //public Item GetItemFromItemId(int id)
        //{
        //    Item item = new(Items[id]);
        //    return item;
        //}

        private void LoadGradeColor()
        {
            GradeColors = new Color[] { 
                Color.white, Color.white, HexToColor("#00C118"), HexToColor("#00E5FF"),
                HexToColor("#2F4FFF"), HexToColor("#D858FF"), HexToColor("#FFB600"), HexToColor("#FF0000")
            };
        }

        public static Color HexToColor(string hexString)
        {
            if (ColorUtility.TryParseHtmlString(hexString, out Color color))
            {
                return color;
            }
            else
            {
                return Color.white;
            }
        }
    }
}
