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

    public GameObject PlayerPrefab;
    public GameObject FollowCamPrefab;

    [SerializeField] private MonsterSpawnPoint[] _spawnPoints;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
        MonstersInMap = new();
        SpawnPlayer();
    }

    // Start is called before the first frame update

    void Start()
    {
        _spawnPoints = FindObjectsOfType<MonsterSpawnPoint>();
        SetMonsterIntoSpawnPoint();
    }

    private void SetMonsterIntoSpawnPoint()
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            MonsterSpawnPoint spawnPoint = _spawnPoints[i];
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

    public void RespawnPlayer()
    {
        //Debug.Log("Respwan Position " + PlayerSpwanPosition.position);
        Player.Instance.Spwan(PlayerSpwanPosition.position);
    }

    private void SpawnPlayer()
    {
        //Debug.Log("First Spwan Position " + PlayerSpwanPosition.position);
        GameObject playerObject = Instantiate(PlayerPrefab, PlayerSpwanPosition.position, Quaternion.identity);
        GameObject followObject = Instantiate(FollowCamPrefab, transform.position, Quaternion.identity);
        CinemachineVirtualCamera virtualCamera = followObject.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = Player.Instance.CameraRoot;
    }

    private Monster CreateMonster(int monsterId)
    {
        GameObject monsterObject = Instantiate(DataBase.MonsterPrefabs[monsterId]);
        Monster monster = monsterObject.GetComponent<Monster>();
        monster.SetMonsterDetails(monsterId);
        return monster;
    }
}
