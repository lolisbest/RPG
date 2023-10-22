using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using RPG.UI;
using System;
using UnityEngine.SceneManagement;

public partial class GameManager : Singleton<GameManager>
{
    [Header("Http 통신")]
    [SerializeField] private HttpCommunicate _httpComunicate;

    [SerializeField] private IntroSceneUIManager _introSceneUIManager;

    public StructPlayerData CurrentPlayerData { get; private set; }

    public void SetIntroUIManager(IntroSceneUIManager uiManager)
    {
        _introSceneUIManager = uiManager;
    }

    public void SetPlayerDataToStart(StructPlayerData playerData)
    {
        CurrentPlayerData = playerData;
    }

    public void LoadInGameScene()
    {
        Debug.Log("LoadInGameScene GameManager");
        //UIManager.Instance.CloseIntroUIs();
        LoadingSceneController.Load(StringStatic.SceneInGame);
    }

    public void TryHttpGet(string url, Action<string> callback)
    {
        _httpComunicate.TryGet(url, callback);
    }

    public void TryHttpGet(string url)
    {
        _httpComunicate.TryGet(url);
    }

    public void DeletePlayerData(int playerDataId)
    {
        string url = "http://localhost:9999/delete/playerData?playerDataId={0}";
        GameManager.Instance.TryHttpGet(string.Format(url, playerDataId));
    }

    public void OpenServerCommunicationErrorWindow(string message)
    {
        _introSceneUIManager.OpenServerCommunicationErrorWindow(message);
    }

    public void ToggleProgressIndicator(bool active)
    {
        _introSceneUIManager.ToggleProgressIndicator(active);
    }

    public void Test()
    {
        Debug.Log("GameManager.Test");
    }
}
