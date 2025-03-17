using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    // Declares variables - the object to track, and the current camera region and its script
    // and the target position for the camera
    public GameObject trackObject;
    public GameObject currentRegion = null;
    
    private CameraRegion regionScript;
    private Vector3 targetPosition;

    // OnDrawGizmos is used to draw useful icons on the unity editor to make it easier to debug
    private void OnDrawGizmos()
    {
        // Sets the camera's position to where it should be
        targetPosition = Vector3.zero;
        ComputeTargetPosition();
        transform.position = targetPosition;

        // Draws "gizmos" in the camera's new position
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, 1);
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(15f, 8.4375f, 2.0f));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(trackObject.transform.position - new Vector3(0.5f, 0.0f, 0.0f), trackObject.transform.position + new Vector3(0.5f, 0.0f, 0.0f));
        Gizmos.DrawLine(trackObject.transform.position - new Vector3(0.0f, 0.5f, 0.0f), trackObject.transform.position + new Vector3(0.0f, 0.5f, 0.0f));
        
    }
    
    // Works out where the camera should be
    protected void ComputeTargetPosition()
    {
        // Initially sets the target coordinates to the current coordinates
        float targetX = transform.position.x;
        float targetY = transform.position.y;

        // Gets the script attached to the region the camera is in
        regionScript = currentRegion.GetComponent<CameraRegion>();

        // There are 2 cases in this version, a follow cam or a static cam
        switch (regionScript.type)
        {
            case CameraRegion.Type.Follow:
                // Checks through the different options and works out the correct target position
                // based on the players position
                if (regionScript.isXLocked)
                {
                    targetX = regionScript.xLock.x;
                }
                else
                {
                    targetX = trackObject.transform.position.x + regionScript.followOffset.x;
                }

                if (regionScript.isYLocked)
                {
                    targetY = regionScript.yLock.y;
                }
                else
                {
                    targetY = trackObject.transform.position.y + regionScript.followOffset.y;
                }

                if (regionScript.isXBounded)
                {
                    targetX = Mathf.Clamp(targetX, regionScript.xBounds.x, regionScript.xBounds.y);
                }
                if (regionScript.isYBounded)
                {
                    targetY = Mathf.Clamp(targetY, regionScript.yBounds.x, regionScript.yBounds.y);
                }
                break;

            case CameraRegion.Type.Static:
                // Sets the target positon to a vector defined in the region object
                targetX = regionScript.staticCentre.x;
                targetY = regionScript.staticCentre.y;
                break;
        }
        
        // The camera uses a 3D vector so the 2D target needs to be combined with a Z value
        targetPosition = new Vector3(targetX, targetY, transform.position.z);
    }

    // Smooths the motion between the camera's current position and target position by 
    // using a linear interpolation function
    protected Vector3 ComputeNewPosition(Vector3 targetPosition)
    {
        float targetX = targetPosition.x;
        float targetY = targetPosition.y;

        float newX = transform.position.x;
        newX = Mathf.Lerp(transform.position.x, targetX, 0.1f);

        float newY = transform.position.y;
        newY = Mathf.Lerp(transform.position.y, targetY, 0.1f);

        return new Vector3(newX, newY, transform.position.z);
    }

    void FixedUpdate()
    {
        // Each frame the camera calculates and moves towards a target position
        targetPosition = Vector3.zero;
        ComputeTargetPosition();

        Vector3 newPosition = ComputeNewPosition(targetPosition);

        transform.position = newPosition;
    }
}
