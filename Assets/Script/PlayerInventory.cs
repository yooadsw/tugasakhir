using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public int NumberOfOrigamis{ get; private set; }

    public UnityEvent<PlayerInventory> OnOrigamiCollected;

    public void OrigamiCollected()
    {
        NumberOfOrigamis++;
        OnOrigamiCollected.Invoke(this);
    }
}
    