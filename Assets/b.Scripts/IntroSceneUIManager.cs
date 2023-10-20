using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.UI;


public class IntroSceneUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainSelectionWindow;

    [SerializeField] private SavedGamesWindow _savedGamesWindow;

    [Header("통신 중일 때 진행 표시")]
    [SerializeField] private GameObject _progressIndicator;

    [Header("서버와의 통신 에러 창")]
    [SerializeField] private ServerErrorWindow _serverErrorWindow;

    void Start()
    {
        GameManager.Instance.SetIntroUIManager(this);
    }

    public void OpenSavedGamedsWindow()
    {
        string url = "http://localhost:9999/request/playerData";
        GameManager.Instance.TryHttpGet(url, _savedGamesWindow.LoadDataIntoSlots);
        _mainSelectionWindow.gameObject.SetActive(false);
        _savedGamesWindow.Open();
    }

    public void ToggleProgressIndicator(bool active)
    {
        if (!_progressIndicator) return;

        _progressIndicator.SetActive(active);
    }

    public void OpenServerCommunicationErrorWindow(string message)
    {
        _serverErrorWindow.Open(message);
    }

    public void CloseServerCommunicationErrorWindow()
    {
        _serverErrorWindow.Close();
    }

    public void ReturnToMain()
    {
        _savedGamesWindow.Close();
        _mainSelectionWindow.gameObject.SetActive(true);
    }

    public void Test()
    {
        Debug.Log("IntroSceneUIManager.Test");
    }
}
