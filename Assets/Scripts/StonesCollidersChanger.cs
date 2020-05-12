using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonesCollidersChanger : MonoBehaviour
{
    [ContextMenu("ActivateStoneColliders")]
    public void ActivateStoneColliders()
    {
        foreach(Stone stone in FindObjectsOfType<Stone>())
            stone.transform.Find("Colliders").gameObject.SetActive(true);
    }

    [ContextMenu("DeactivateStoneColliders")]
    public void DeactivateStoneColliders()
    {
        foreach (Stone stone in FindObjectsOfType<Stone>())
            stone.transform.Find("Colliders").gameObject.SetActive(false);
    }
}
