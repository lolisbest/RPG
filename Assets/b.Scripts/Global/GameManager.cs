using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using RPG.UI;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;

public partial class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool _isInIntroScene;

    public Player @Player { get; private set; }

    public GameObject FollowCamPrefab;

    [SerializeField] private GameObject _playerPrefab;

    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    protected override void Awake()
    {
        Debug.Log("GameManater.Awake()");
        InitQuitting();

        base.Awake();

        DontDestroyOnLoad(gameObject);
        DataBase.Initialize();
        DataBase.Load();

        CurrentPlayerData = StructPlayerData.GetTempData();
        Debug.Log("Test " + CurrentPlayerData);

        InteractableObject.SetLayerMaskValue();

        Debug.Log("Awake() _playerPrefab " + _playerPrefab);
    }

    void Start()
    {
        if (_isInIntroScene)
        {
            _uiManager.SwitchToIntro();
        }
        else
        {
            _uiManager.SwitchToInGame();
        }

        Debug.Log("GameManager.Start");
        Debug.Log("Start() _playerPrefab " + _playerPrefab);
    }

    private void OnEnable()
    {
        Debug.Log("GameManager.OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log("GameManager.OnDisable");
    }

    void FixedUpdate()
    {

    }

    void Update()
    {

    }

    void LateUpdate()
    {

    }

    /// <summary>
    /// CurrentPlayerData.IsNewCharacter 가 true 라면, spwanPosition에 스폰. false라면 저장된 스폰 위치에 생성
    /// </summary>
    /// <param name="spwanPosition"></param>
    public void SpawnPlayer(Vector3 spwanPosition)
    {
        Vector3 spawnPosition = new Vector3(CurrentPlayerData.SpwanX, CurrentPlayerData.SpwanY, CurrentPlayerData.SpwanZ);
        if (CurrentPlayerData.IsNewCharacter)
        {
            spawnPosition = spwanPosition;
        }

        Debug.Log("SpawnPlayer() PlayerPrefab " + _playerPrefab);
        Debug.Log("spawnPosition " + spawnPosition);
        GameObject playerObject = Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);
        Player player = playerObject.GetComponent<Player>();
        if (!player) Utils.CheckNull(player, "loaded player is null");

        GameObject followObject = Instantiate(FollowCamPrefab, transform.position, Quaternion.identity);
        CinemachineVirtualCamera virtualCamera = followObject.GetComponent<CinemachineVirtualCamera>();

        Debug.Log("virtualCamera: " + virtualCamera);
        Debug.Log("Player.Instance.CameraRoot: " + player.CameraRoot);

        virtualCamera.Follow = player.CameraRoot;

        @Player = player;
    }

    public void RespawnPlayer(Vector3 respawnPosition)
    {
        //Debug.Log("Respwan Position " + PlayerSpwanPosition.position);
        @Player.Spwan(respawnPosition);
    }

}
