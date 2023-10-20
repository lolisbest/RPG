using RPG.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Monster;
using System;

public class MapManager : Singleton<MapManager>
{
    public List<Monster> MonstersInMap;

    public int CurrentMapId = 1;

    void Awake()
    {
        Initialize();
        MonstersInMap = new();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach(var monster in MonstersInMap)
        {
            if(monster.IsDie)
            {
                monster.LeftTimeToSpawn -= Time.deltaTime;
                if (monster.LeftTimeToSpawn <= 0f)
                {
                    monster.Spawn();
                }
            }
        }
    }

    public void AddMonster(Monster monster)
    {
        MonstersInMap.Add(monster);
    }

    public override void Initialize()
    {
        ;
    }
}
