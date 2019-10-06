using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public bool open;

    public void OpenLock()
    {
        open = true;
        GetComponent<Item>().DestroyItem();
        BoardManager.Instance.CheckIfCurrentZoneOpen();
    }
}
