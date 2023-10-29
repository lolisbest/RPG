using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastTest : MonoBehaviour
{
    public string TargetLayerName;
    public QueryTriggerInteraction option;
    public bool BitInversion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("layer index " + LayerMask.NameToLayer(TargetLayerName));
        int layerMask = 1 << LayerMask.NameToLayer(TargetLayerName);
        Debug.Log("LayerMask " + layerMask);
        Debug.Log("LayerMask Name1 " + LayerMask.LayerToName(LayerMask.NameToLayer(TargetLayerName)));

        if (BitInversion) layerMask = ~layerMask;

        if (Physics.SphereCast(
            transform.position, 0.5f, transform.forward, out RaycastHit hit, 
            float.PositiveInfinity, layerMask, option))
        {
            Debug.Log("Hit name " + hit.transform.name);
            Debug.Log("Hit layer " + hit.transform.gameObject.layer);
            Debug.Log("Hit layer name " + LayerMask.LayerToName(hit.transform.gameObject.layer));
        }
    }
}
