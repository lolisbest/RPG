using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using RPG.Item;
using System.Linq;

public class ItemDropper : Singleton<ItemDropper>
{
    public GameObject LowLevelItemBoxPrefab;
    //public GameObject MiddleLevelItemBoxPrefab;
    //public GameObject HighLevelItemBoxPrefab;

    public override void Initialize()
    {
        ;
    }

    public void DropItemBox(int monsterId, Vector3 spawnPosition)
    {
        FieldItemBox itemBox = CreateFieldItemBox();
        StructIdCount[] selectedItems = GetItemsFromMonsterId(monsterId);
        itemBox.SetItems(selectedItems);
        itemBox.Pop(spawnPosition);
        return;
    }

    private FieldItemBox CreateFieldItemBox()
    {
        GameObject itemBoxObject = Instantiate(LowLevelItemBoxPrefab);
        FieldItemBox itemBox = itemBoxObject.GetComponent<FieldItemBox>();
        itemBox.Initialize();
        return itemBox;
    }

    /// <summary>
    /// ��� ������ �����۵� �߿� �����ϰ� �����Ͽ� �迭�� ��ȯ. �ּ� 1������ ����
    /// </summary>
    /// <param name="monsterId"></param>
    /// <returns></returns>
    private StructIdCount[] GetItemsFromMonsterId(int monsterId)
    {
        System.Random rand = new();

        StructIdCount[] availableItems = DataBase.Monsters[monsterId].DropItems;

        // �ּ� 2��
        int exceptTypeNumber = rand.Next(availableItems.Length - 1);

        List<StructIdCount> items = availableItems.ToList();

        for (int i = 0; i < exceptTypeNumber; i++)
        {
            items.RemoveAt(Random.Range(0, items.Count));
        }

        return items.ToArray();
    }
}
