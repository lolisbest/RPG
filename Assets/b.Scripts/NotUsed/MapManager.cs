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

    [SerializeField] private MonsterSpawnPoint[] _monsterSpawnPoints;

    [SerializeField] private PlayerRespawnPoint _playerRespawnPoint;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    // Start is called before the first frame update

    void Start()
    {
        InstantiateMonsters();
        GameManager.Instance.SpawnPlayer(_playerRespawnPoint.RespawnPosition);
    }

    /// <summary>
    /// 몬스터 프리팹을 인스턴스화하고, 각 몬스터 별 transform 설정. MonstersInMap에 추가
    /// </summary>
    private void InstantiateMonsters()
    {
        for (int i = 0; i < _monsterSpawnPoints.Length; i++)
        {
            MonsterSpawnPoint spawnPoint = _monsterSpawnPoints[i];
            spawnPoint.name = $"MonsterSpawnPoint[{i}] - ({spawnPoint.transform.position})";
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
        MonstersInMap = new();

        _monsterSpawnPoints = FindObjectsOfType<MonsterSpawnPoint>();

        PlayerRespawnPoint[] playerRespawnPoints = FindObjectsOfType<PlayerRespawnPoint>();
        Debug.Assert(playerRespawnPoints.Length == 1, "PlayerSpawnPoint in Current Map is not single.");
        _playerRespawnPoint = playerRespawnPoints[0];
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
        GameManager.Instance.RespawnPlayer(_playerRespawnPoint.RespawnPosition);
    }
}
