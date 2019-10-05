using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int maxWidth = 10;
    public float step;
    public Transform boardParent;
    public int currentZone;
    public List<SpriteRenderer> lockZones = new List<SpriteRenderer>();
    public List<LockList> lockLists = new List<LockList>();

    public GameObject p_fog;
    public Transform fogParent;
    SpriteRenderer[,] fogs = new SpriteRenderer[10, 10];

    public GameObject p_cell;
    public Transform cellsParent;
    GameObject[,] cells = new GameObject[10, 10];


    GameObject[,] items = new GameObject[10, 10];



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckIfCurrentZoneOpen();
        }
    }

    public void Start()
    {
        InitializeFog();
        InitializeZones();
        OpenZone(currentZone);
        InitializeCells();
    }

    public void InitializeFog()
    {
        for (int x = 0; x < maxWidth;x++)
        {
            for (int y = 0; y < maxWidth; y++)
            {
                Vector3 fogPos = fogParent.position + new Vector3(x*step, y*step, 0);
                GameObject fog = Instantiate(p_fog, fogPos, Quaternion.identity, fogParent) as GameObject;
                fogs[x, y] = fog.GetComponent<SpriteRenderer>();
            }
        }
    }

    public void InitializeCells()
    {
        for (int x = 0; x < maxWidth; x++)
        {
            for (int y = 0; y < maxWidth; y++)
            {
                Vector3 cellPos = cellsParent.position + new Vector3(x * step, y * step, 0);
                GameObject cell = Instantiate(p_cell, cellPos, Quaternion.identity, cellsParent) as GameObject;
                cells[x, y] = cell;
                cell.SetActive(false);
            }
        }
    }

    public void SetFogOpacityAt(int x, int y, float a)
    {
        fogs[x, y].color = new Color(fogs[x, y].color.r, fogs[x, y].color.g, fogs[x, y].color.b, a);
    }

    public void CheckIfCurrentZoneOpen()
    {
        bool open = true;
        foreach(Lock l in lockLists[currentZone].locks)
        {
            open = l.open;
        }
        if (open)
        {
            lockZones[currentZone].gameObject.SetActive(false);
            currentZone++;
            OpenZone(currentZone);
        }
    }
    
    public void InitializeZones()
    {
        foreach(SpriteRenderer sp in lockZones)
        {
            sp.gameObject.SetActive(false);
        }

        lockZones[currentZone].gameObject.SetActive(true);
    }

    public void OpenZone(int i)
    {
        int start = Mathf.FloorToInt(maxWidth * 0.5f) - ((currentZone + 1));
        int max = (currentZone + 1) * 2;

        for (int x = start;x<start+max;x++)
        {
            for (int y = start; y < start+ max; y++)
            {
                SetFogOpacityAt(x, y, 0);
            }
        }

        lockZones[currentZone].gameObject.SetActive(true);
        lockZones[currentZone].sortingLayerName = "AboveFog";

    }
}

[System.Serializable]
public class BoardConfig
{
    
}

[System.Serializable]
public class LockList
{
    public List<Lock> locks = new List<Lock>();
}
