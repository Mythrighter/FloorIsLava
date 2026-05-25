using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<string> keys = new List<string>();

    public bool HasKey(string keyId) => keys.Contains(keyId);

    public void AddKey(string keyId)
    {
        if (!keys.Contains(keyId)) keys.Add(keyId);
    }

    public void RemoveKey(string keyId) => keys.Remove(keyId);
}
