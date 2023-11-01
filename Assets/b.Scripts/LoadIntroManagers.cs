//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LoadIntroManagers : MonoBehaviour
//{
//    [SerializeField] private GameObject _gameManagerPrefab;
//    [SerializeField] private GameManager gameManager;
//    [SerializeField] private GameObject _introSceneUIManagerPrefab;
//    [SerializeField] private UIManager _introSceneUIManager;

//    void Awake()
//    {
//        Instantiate(_gameManagerPrefab).TryGetComponent(out gameManager);
//        gameManager.name = "GameManager[Init]";
//        Instantiate(_introSceneUIManagerPrefab).TryGetComponent(out _introSceneUIManager);

//        Destroy(this.gameObject);
//    }
//}
