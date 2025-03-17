using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraRegion : MonoBehaviour
{
    // Each region needs a unique ID and is either Follow or Static
    // By using an enumerator the camera is forced to be one of these 2 types
    public int regionID;
    public enum Type {Follow, Static}
    public Type type;

    // Values for Follow Cam
    public Vector2 followOffset;
    public bool isXLocked;
    public Vector2 xLock;
    public bool isYLocked;
    public Vector2 yLock;
    public bool isXBounded;
    public Vector2 xBounds;
    public bool isYBounded;
    public Vector2 yBounds;

    // Values for Static Cam
    public Vector3 staticCentre = new Vector3(0.0f, 0.0f, 0.0f);

    // For the main camera and it's script
    GameObject playerCam;
    CameraController cameraScript;

    public void OnDrawGizmos()
    {
        // Used to work out which region the player is in and set the 
        // camera's region appropriately within the editor
        playerCam = GameObject.Find("Player Camera");
        cameraScript = playerCam.GetComponent<CameraController>();
        Collider2D regionCollider = GetComponent<Collider2D>();
        if (regionCollider.bounds.Intersects(cameraScript.trackObject.GetComponent<Collider2D>().bounds)){
            cameraScript.currentRegion = gameObject;
        }
    }

    private void Awake()
    {
        // Finds the main camera and it's script when the level starts
        playerCam = GameObject.Find("Player Camera");
        cameraScript = playerCam.GetComponent<CameraController>();
        Collider2D regionCollider = GetComponent<Collider2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Sets the camera's region to this one when the player collides with it
        cameraScript = playerCam.GetComponent<CameraController>();
        if (other.gameObject == cameraScript.trackObject){
            cameraScript.currentRegion = gameObject;
        }
    }

    
}
