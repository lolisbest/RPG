using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using RPG.UI;
using System;
using UnityEngine.SceneManagement;

public partial class GameManager : Singleton<GameManager>
{
    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        Debug.Log("GameManager.Awake");

        InitQuitting();
        DontDestroyOnLoad(gameObject);
        DataBase.Initialize();
        DataBase.Load();

        CurrentPlayerData = StructPlayerData.GetTempData();
        Debug.Log("Test " + CurrentPlayerData);

        InteractableObject.SetLayerMaskValue();
    }

    void Start()
    {
        Debug.Log("GameManager.Start");
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
}
