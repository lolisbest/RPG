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

    [SerializeField] private UIManager _uiManager;

    public StructPlayerData CurrentPlayerData { get; private set; }

    public void SetUIManager(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    public void SetCurrentPlayerData(StructPlayerData playerData)
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

    public void TryHttpPost(string url, string data)
    {
        Debug.Log("_httpComunicate " + _httpComunicate.name);
        _httpComunicate.TryPost(url, data);
    }

    public void DeletePlayerData(int playerDataId)
    {
        string url = "http://localhost:9999/delete/playerData?playerDataId={0}";
        GameManager.Instance.TryHttpGet(string.Format(url, playerDataId));
    }

    public void OpenServerCommunicationErrorWindow(string message)
    {
        _uiManager.OpenServerCommunicationErrorWindow(message);
    }

    public void ToggleProgressIndicator(bool active)
    {
        _uiManager.ToggleProgressIndicator(active);
    }

    public void Test()
    {
        Debug.Log("GameManager.Test");
    }

    public void SavePlayerData()
    {
        string url = "http://localhost:9999/upload/playerData";

        StructPlayerData[] playerDataArray = new StructPlayerData[] { CurrentPlayerData };
        Debug.Log("CurrentPlayerData " + CurrentPlayerData);
        string data = Utils.JsonHelper.ToJson(playerDataArray);
        Debug.Log(data);
        GameManager.Instance.TryHttpPost(url, data);
    }
}
