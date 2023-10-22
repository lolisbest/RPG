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


    protected override void Awake()
    {
        base.Awake();
        Initialize();
        MonstersInMap = new();
        SpwanPlayer();
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

    public void RespwanPlayer()
    {
        //Debug.Log("Respwan Position " + PlayerSpwanPosition.position);
        Player.Instance.Spwan(PlayerSpwanPosition.position);
    }

    private void SpwanPlayer()
    {
        //Debug.Log("First Spwan Position " + PlayerSpwanPosition.position);
        GameObject playerObject = Instantiate(PlayerPrefab, PlayerSpwanPosition.position, Quaternion.identity);
        GameObject followObject = Instantiate(FollowCamPrefab, transform.position, Quaternion.identity);
        CinemachineVirtualCamera virtualCamera = followObject.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = Player.Instance.CameraRoot;
    }
}
