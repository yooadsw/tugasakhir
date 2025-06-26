using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Origami : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInventory.OrigamiCollected();
            gameObject.SetActive(false);
        }
    }
}
