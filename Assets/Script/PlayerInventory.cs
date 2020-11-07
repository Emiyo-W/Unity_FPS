using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<int> keyArr;

    void Start()
    {
        keyArr = new List<int>();
    }

    public void AddKey(int keyId)
    {
        if (!keyArr.Contains(keyId))
            keyArr.Add(keyId);
    }

    public bool HasKey(int doorId)
    {
        if (keyArr.Contains(doorId))
            return true;
        return false;
    }
}
