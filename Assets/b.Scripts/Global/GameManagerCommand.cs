//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using RPG.Common;
//using RPG.UI;
//using UnityEngine.UI;

//public enum EnumCommand
//{
//    CreateNewGame,
//    OpenSavedGamesWindow,
//    CloseSavedGamesWindow,
//    ReqeustPlayerData,
//    OpenProgressIndicator,
//    CloseProgressIndicator,
//    OpenServerErrorWindow,
//    CloseServerErrorWindow,
//    DeletePlayerData,
//    SavePlayerData,
//    SceneLoad,
//}


//public partial class GameManager : Singleton<GameManager>
//{
//    public void Process(EnumCommand command, object value)
//    {
//        switch(command)
//        {
//            case EnumCommand.CreateNewGame:
//                break;
//            case EnumCommand.OpenSavedGamesWindow:
//                _introSceneUIManager.OpenSavedGamedsWindow();
//                break;
//            case EnumCommand.ReqeustPlayerData:
//                break;
//            case EnumCommand.DeletePlayerData:
//                break;
//            case EnumCommand.SavePlayerData:
//                break;
//            case EnumCommand.SceneLoad:
//                break;
//            default:
//                throw new System.Exception($"Not Implemented command: {command}");
//        }
//    }
//}
