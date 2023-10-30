//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Common;
//using RPG.UI;
//using UnityEngine.UI;

//public partial class GameManager : Singleton<GameManager>
//{
//    [SerializeField] private SavedGamesWindow _savedGamesWindow;
//    public void OnClickedLoadGame()
//    {
//        _savedGamesWindow.Open();
//        StructPlayerData[] playerDataArray = LoadGameData();
//        _savedGamesWindow.LoadDataIntoSlots(playerDataArray);
//    }

//    private StructPlayerData[] LoadGameData()
//    {
//        return new StructPlayerData[1];
//    }
//}
