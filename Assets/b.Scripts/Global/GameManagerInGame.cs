using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using RPG.UI;
using System;
using UnityEngine.SceneManagement;


public partial class GameManager : Singleton<GameManager>
{
    [SerializeField] private InGameUIManager _inGameUIManager;
    [SerializeField] private QuestManager _questManager;
    [SerializeField] private GameObject _minimapCameraPrefab;
    [SerializeField] private FollowPosition _minimapFollowPosition;

    [SerializeField] private GameObject _playerPrefab;

    public void InGame()
    {
        Debug.Log("GameManager UIManager");
        _inGameUIManager = InGameUIManager.Instance;
        _inGameUIManager.Initialize();

        _questManager = QuestManager.Instance;
        _questManager.Initialize();

        Debug.Log("@Player.Initialize()");

        LoadMinimapCamera();
    }

    private void LoadMinimapCamera()
    {
        if (!_minimapCameraPrefab) { Debug.Log("No MinimapCamera Prefab"); return; }

        if (_minimapFollowPosition) return;

        GameObject minimapCamera = Instantiate(_minimapCameraPrefab);
        if (minimapCamera.TryGetComponent<FollowPosition>(out _minimapFollowPosition))
        {
            _minimapFollowPosition.SetTarget(@Player.Instance.transform);
        }
    }


}
