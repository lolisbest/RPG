using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnPoint : MonoBehaviour
{
    [SerializeField] private int _placeId;
    public int PlaceId { get => _placeId; }

    [SerializeField] private string _placeName;
    public string PlaceName { get => _placeName; }

    //[SerializeField] private Vector3 _respawnPosition;
    public Vector3 RespawnPosition { get => transform.position; }

    public override string ToString()
    {
        return $"PlaceId : {PlaceId}\n" +
            $"PlaceName : {PlaceName}\n" +
            $"RespawnPosition : {RespawnPosition}\n";
    }
}
