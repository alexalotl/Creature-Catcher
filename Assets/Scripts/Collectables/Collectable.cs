using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    protected virtual void OnCollect()
    {
        // Replace with individual collectable functions
    }

    // General rules for collecting
    public void Collect()
    {
        OnCollect();
        gameObject.SetActive(false);
    }
}
