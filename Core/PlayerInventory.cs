using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance; // Singleton

    public List<string> collectedItems = new List<string>();
    
    // NEW: Code digit tracking
    private Dictionary<int, string> codeDigits = new Dictionary<int, string>();
    public int codeLength = 4;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string itemName)
    {
        if (!collectedItems.Contains(itemName))
        {
            collectedItems.Add(itemName);
            Debug.Log("Picked up: " + itemName);
        }
    }

    public bool HasItem(string itemName)
    {
        return collectedItems.Contains(itemName);
    }

    // NEW: Code digit methods
    public void AddCodeDigit(int position, string value)
    {
        if (!codeDigits.ContainsKey(position))
        {
            codeDigits[position] = value;
            Debug.Log($"Code Progress: {codeDigits.Count}/{codeLength}");
        }
    }

    public string GetCodeDigit(int position)
    {
        return codeDigits.ContainsKey(position) ? codeDigits[position] : "?";
    }

    public string GetFullCode()
    {
        string code = "";
        for (int i = 0; i < codeLength; i++)
        {
            code += GetCodeDigit(i);
        }
        return code;
    }

    public bool IsCodeComplete()
    {
        return codeDigits.Count >= codeLength;
    }
}
