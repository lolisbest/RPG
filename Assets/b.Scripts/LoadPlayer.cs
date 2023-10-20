using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayer : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject FollowCamPrefab;

    void Start()
    {
        GameObject playerObject = Instantiate(PlayerPrefab, transform.position, Quaternion.identity);
        GameObject followObject = Instantiate(FollowCamPrefab, transform.position, Quaternion.identity);
        CinemachineVirtualCamera virtualCamera = followObject.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = Player.Instance.CameraRoot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
