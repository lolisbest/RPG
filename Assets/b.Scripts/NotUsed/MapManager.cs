using RPG.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Monster;
using System;
using Cinemachine;

public class MapManager : Singleton<MapManager>
{
    public List<Monster> MonstersInMap;

    public int CurrentMapId = 1;

    public Transform PlayerSpwanPosition;


    [SerializeField] private MonsterSpawnPoint[] _monsterSpawnPoints;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
        MonstersInMap = new();
    }

    // Start is called before the first frame update

    void Start()
    {
        SpawnPlayer();
        _monsterSpawnPoints = FindObjectsOfType<MonsterSpawnPoint>();
        SetMonsterIntoSpawnPoint();
    }

    private void SetMonsterIntoSpawnPoint()
    {
        for (int i = 0; i < _monsterSpawnPoints.Length; i++)
        {
            MonsterSpawnPoint spawnPoint = _monsterSpawnPoints[i];
            Monster monster = CreateMonster(spawnPoint.SpawnMonsterId);
            monster.transform.position = spawnPoint.SpawnPosition;
            monster.SetIntialPoseRot(spawnPoint.SpawnPosition, spawnPoint.transform.rotation);
            MonstersInMap.Add(monster);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var monster in MonstersInMap)
        {
            if(monster.IsDie)
            {
                //Debug.Log($"{monster.name} LeftTimeToSpawn: {monster.LeftTimeToSpawn}");
                monster.LeftTimeToSpawn -= Time.deltaTime;
                if (monster.LeftTimeToSpawn <= 0f)
                {
                    monster.Spawn();
                }
            }
        }
    }

    public override void Initialize()
    {
        ;
    }


    private Monster CreateMonster(int monsterId)
    {
        GameObject monsterObject = Instantiate(DataBase.MonsterPrefabs[monsterId]);
        Monster monster = monsterObject.GetComponent<Monster>();
        monster.SetMonsterDetails(monsterId);
        return monster;
    }

    public void RespawnPlayerAtStartPosition()
    {
        GameManager.Instance.RespawnPlayer(PlayerSpwanPosition.position);
    }

    private void SpawnPlayer()
    {
        GameManager.Instance.SpawnPlayer();
    }
}
