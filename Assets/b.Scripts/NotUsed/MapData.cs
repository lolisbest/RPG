using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// order : Assets/Create 메뉴 내 메뉴 항목의 위치.
//[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Object/Map Data", order = int.MaxValue)]
public class MapData : ScriptableObject
{
    [SerializeField] private int _placeId;
    public int PlaceId { get => _placeId; }

    [SerializeField] private string _placeName;
    public string PlaceName { get => _placeName; }

    [SerializeField] private Vector3 _respawnPosition;
    public Vector3 RespawnPosition { get => _respawnPosition; }

    [SerializeField] private bool _canRespawn;
    public bool CanRespawn { get => _canRespawn; }

    public override string ToString()
    {
        return $"PlaceId : {PlaceId}\n" +
            $"PlaceName : {PlaceName}\n" +
            $"RespawnPosition : {RespawnPosition}\n" +
            $"CanRespawn : {CanRespawn}\n";
    }
}
