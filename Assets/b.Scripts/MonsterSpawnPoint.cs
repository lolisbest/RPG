using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Monster;

public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField] private int _spawMonsterId;
    public int SpawnMonsterId { get => _spawMonsterId; }
    public Vector3 SpawnPosition { get => transform.position; }
}
