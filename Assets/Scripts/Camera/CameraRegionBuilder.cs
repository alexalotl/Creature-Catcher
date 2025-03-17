using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRegionBuilder : MonoBehaviour
{
    // Declares variables
    public GameObject region;

    public int regionID;
    public CameraRegion.Type type;

    // Values for Follow Cam
    public Vector2 followOffset;
    public bool isXLocked;
    public bool isYLocked;

    // Values for Static Cam
    public Vector3 staticCentre = new Vector3(0.0f, 0.0f, 0.0f);
    private CameraRegion regionScript;
    
    // Produces a camera region from a prefab with the correct values
    public void BuildRegion()
    {
        region = (GameObject)Resources.Load("Prefabs/CameraRegion");

        GameObject builtRegion = Instantiate(region);

        builtRegion.name = "Region";
        regionScript = builtRegion.GetComponent<CameraRegion>();

        regionScript.type = type;
        regionScript.followOffset = followOffset;
        regionScript.isXLocked = isXLocked;
        regionScript.isYLocked = isYLocked;
        regionScript.staticCentre = staticCentre;
    }
}
