using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBoundary : MonoBehaviour
{
    [SerializeField] private string _enteringPlaceName;
    public string EnteringPlaceName { get => _enteringPlaceName; }
}
