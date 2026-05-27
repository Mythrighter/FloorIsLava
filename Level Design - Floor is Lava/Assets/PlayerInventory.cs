using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [SerializeField] private Transform slotContainer; // Parent in the canvas where icons appear
    [SerializeField] private GameObject keyIconPrefab; // A UI prefab with an Image component

    private readonly List<string> keys = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddKey(string keyId, Sprite icon)
    {
        keys.Add(keyId);

        GameObject iconGO = Instantiate(keyIconPrefab, slotContainer);
        Image image = iconGO.GetComponent<Image>();
        if (image != null && icon != null)
        {
            image.sprite = icon;
        }

        Debug.Log($"Picked up key: {keyId}. Total keys: {keys.Count}");
    }

    public bool HasKey(string keyId) => keys.Contains(keyId);
    public bool UseKey(string keyId) => keys.Remove(keyId);
}
