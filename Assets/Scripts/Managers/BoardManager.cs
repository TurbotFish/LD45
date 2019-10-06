using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{

    protected static BoardManager _Instance;

    public static BoardManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<BoardManager>();
            }
            return _Instance;
        }
    }



    public int maxWidth = 10;
    public float step;
    public Transform boardParent;
    public int currentZone;
    public GameObject lockPrefab;
    public List<SpriteRenderer> lockZones = new List<SpriteRenderer>();
    public List<LockList> lockLists = new List<LockList>();

    public GameObject p_fog;
    public Transform fogParent;
    SpriteRenderer[,] fogs = new SpriteRenderer[10, 10];

    public GameObject p_cell;
    public Transform cellsParent;
    GameObject[,] cells = new GameObject[10, 10];
    public Color cellHoverColor, cellDefaultColor;
    public Item[,] items = new Item[10, 10];
    public List<Heart> hearts = new List<Heart>();

    public List<Item> connectedItems = new List<Item>();
    List<Item> boltChainItems = new List<Item>();

    public List<BoardConfig> configs = new List<BoardConfig>();

    public void Update()
    {

    }

    public void Start()
    {
        InitializeFog();
        InitializeZones();
        OpenZone(currentZone);
        InitializeCells();
        InitializeItems(0);

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
                Cell c = cell.GetComponent<Cell>();
                c.x = x;
                c.y = y;
                cell.SetActive(false);
            }
        }
    }

    public void InitializeItems(int configID)
    {
        for (int i = 0; i < configs[configID].spawnItems.Count;i++)
        {
            InstantiateItem(Mathf.RoundToInt(configs[configID].spawnItems[i].pos.x), Mathf.RoundToInt(configs[configID].spawnItems[i].pos.y), configs[configID].spawnItems[i].item);
        }
        for (int i = 0; i < configs[configID].spawnLocks.Count; i++)
        {
            InstantiateLock(Mathf.RoundToInt(configs[configID].spawnLocks[i].pos.x), Mathf.RoundToInt(configs[configID].spawnLocks[i].pos.y), configs[configID].spawnLocks[i].zone );
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
        int max = start + (currentZone + 1) * 2;

        for (int x = start;x<max;x++)
        {
            for (int y = start; y < max; y++)
            {
                SetFogOpacityAt(x, y, 0);
            }
        }

        lockZones[currentZone].gameObject.SetActive(true);
        lockZones[currentZone].sortingLayerName = "AboveFog";

    }


    public void HighlightAdjacents()
    {
        ComputeConnections();

        int start = Mathf.FloorToInt(maxWidth * 0.5f) - ((currentZone + 1));
        int max = start + (currentZone + 1) * 2;

        for(int i = 0; i < connectedItems.Count;i++)
        {
            if (connectedItems[i].x + 1 < max)
            {
                if (items[connectedItems[i].x + 1, connectedItems[i].y] == null)
                {
                    cells[connectedItems[i].x + 1, connectedItems[i].y].SetActive(true);
                }
            }
            if (connectedItems[i].x - 1 >= start)
            {
                if (items[connectedItems[i].x - 1, connectedItems[i].y] == null)
                {
                    cells[connectedItems[i].x - 1, connectedItems[i].y].SetActive(true);
                }
            }
            if (connectedItems[i].y + 1 < max)
            {
                if (items[connectedItems[i].x, connectedItems[i].y + 1] == null)
                {
                    cells[connectedItems[i].x, connectedItems[i].y + 1].SetActive(true);
                }
            }
            if (connectedItems[i].y - 1 >= start)
            {
                if (items[connectedItems[i].x, connectedItems[i].y - 1] == null)
                {
                    cells[connectedItems[i].x, connectedItems[i].y - 1].SetActive(true);
                }
            }
        }
        
    }

    public void HighlightPlayerItems()
    {
        for (int x = 0; x<maxWidth;x++)
        {
            for (int y = 0; y < maxWidth; y++)
            {
                if (items[x,y] != null)
                {
                    if (items[x, y].player)
                    {
                        cells[x, y].SetActive(true);
                    }
                }
            }
        }
    }

    public void HighlightFreeCells()
    {

        int start = Mathf.FloorToInt(maxWidth * 0.5f) - ((currentZone + 1));
        int max = start+ (currentZone + 1) * 2;

        for (int x = start; x < max; x++)
        {
            for (int y = start; y < max; y++)
            {
                if (items[x, y] == null)
                {
                    cells[x, y].SetActive(true);

                }
            }
        }
    }

    public void HideCells()
    {
        foreach(GameObject go in cells)
        {
            go.SetActive(false);
        }
    }



    public void InstantiateItem(int x, int y, GameObject goRef)
    {
        GameObject go = Instantiate(goRef, cells[x, y].transform.position, Quaternion.identity, boardParent) as GameObject;
        Item item = go.GetComponent<Item>();
        item.x = x;
        item.y = y;
        items[x, y] = item;

    }

    public void InstantiateLock(int x, int y, int lockListID)
    {
        GameObject go = Instantiate(lockPrefab, cells[x, y].transform.position, Quaternion.identity, boardParent) as GameObject;
        Item item = go.GetComponent<Item>();
        item.x = x;
        item.y = y;
        items[x, y] = item;
        lockLists[lockListID].locks.Add(go.GetComponent<Lock>());

    }

    public IEnumerator SwordAttack(int x, int y)
    {
        int start = Mathf.FloorToInt(maxWidth * 0.5f) - ((currentZone + 1));
        int max = start + (currentZone + 1) * 2;

        if (x+1 < max)
        {
            if (items[x + 1,y]!=null)
            {
                items[x + 1, y].HitItem();
            }
        }
        if (x - 1 >= start)
        {
            if (items[x - 1, y] != null)
            {
                items[x - 1, y].HitItem();
            }
        }
        if (y + 1 < max)
        {
            if (items[x, y+1] != null)
            {
                items[x, y+1].HitItem();
            }
        }
        if (y - 1 >= start)
        {
            if (items[x, y-1] != null)
            {
                items[x, y-1].HitItem();
            }
        }
        yield return new WaitForEndOfFrame();
        DOTween.Restart("camera_shake_1");
    }

    public IEnumerator BombAttack(int x, int y)
    {
        int start = Mathf.FloorToInt(maxWidth * 0.5f) - ((currentZone + 1));
        int max = start + (currentZone + 1) * 2;

        if (x + 1 < max)
        {
            if (items[x + 1, y] != null)
            {
                items[x + 1, y].HitItem();
            }
        }
        if (x - 1 >= start)
        {
            if (items[x - 1, y] != null)
            {
                items[x - 1, y].HitItem();
            }
        }
        if (y + 1 < max)
        {
            if (items[x, y + 1] != null)
            {
                items[x, y + 1].HitItem();
            }
        }
        if (y - 1 >= start)
        {
            if (items[x, y - 1] != null)
            {
                items[x, y - 1].HitItem();
            }
        }
        yield return new WaitForEndOfFrame();
        DOTween.Restart("camera_shake_1");

    }

    public IEnumerator Heal(int x, int y)
    {
        if (x + 1 < maxWidth)
        {
            if (items[x + 1, y] != null)
            {
                if (items[x + 1, y].GetComponent<Heart>() != null)
                {
                    items[x + 1, y].GetComponent<Heart>().Heal();
                }
            }
        }
        if (x - 1 >= 0)
        {
            if (items[x - 1, y] != null)
            {
                if (items[x - 1, y].GetComponent<Heart>() != null)
                {
                    items[x - 1, y].GetComponent<Heart>().Heal();
                }
            }
        }
        if (y + 1 < maxWidth)
        {
            if (items[x, y + 1] != null)
            {
                if (items[x, y + 1].GetComponent<Heart>() != null)
                {
                    items[x, y + 1].GetComponent<Heart>().Heal();
                }
            }
        }
        if (y - 1 >= 0)
        {
            if (items[x, y - 1] != null)
            {
                if (items[x, y - 1].GetComponent<Heart>() != null)
                {
                    items[x, y - 1].GetComponent<Heart>().Heal();
                }
            }
        }
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator BoltAttack (int x, int y)
    {
        int start = Mathf.FloorToInt(maxWidth * 0.5f) - ((currentZone + 1));
        int max = start + (currentZone + 1) * 2;

        boltChainItems.Clear();
        boltChainItems.Add(items[x, y]);

        for (int j = 0; j < boltChainItems.Count; j++)
        {

            if (boltChainItems[j].x + 1 < max)
            {
                if (items[boltChainItems[j].x + 1, boltChainItems[j].y] != null)
                {
                    Item item = items[boltChainItems[j].x + 1, boltChainItems[j].y];
                    if (item.player)
                    {
                        if (!boltChainItems.Contains(item))
                        {
                            boltChainItems.Add(item);
                            if (item.type == CardManager.CardType.Heart || item.type == CardManager.CardType.CorruptedHeart || item.type == CardManager.CardType.TinyHeart)
                            {
                                item.HitItem();
                                DOTween.Restart("camera_shake_1");
                            }
                        }
                    }
                }
            }
            if (boltChainItems[j].x - 1 >= start)
            {
                if (items[boltChainItems[j].x - 1, boltChainItems[j].y] != null)
                {
                    Item item = items[boltChainItems[j].x - 1, boltChainItems[j].y];
                    if (item.player)
                    {
                        if (!boltChainItems.Contains(item))
                        {
                            boltChainItems.Add(item);
                            if (item.type == CardManager.CardType.Heart || item.type == CardManager.CardType.CorruptedHeart || item.type == CardManager.CardType.TinyHeart)
                            {
                                item.HitItem();
                                DOTween.Restart("camera_shake_1");
                            }
                        }
                    }
                }
            }
            if (boltChainItems[j].y+ 1 < max)
            {
                if (items[boltChainItems[j].x, boltChainItems[j].y + 1] != null)
                {
                    Item item = items[boltChainItems[j].x, boltChainItems[j].y + 1];
                    if (item.player)
                    {
                        if (!boltChainItems.Contains(item))
                        {
                            boltChainItems.Add(item);
                            if (item.type == CardManager.CardType.Heart || item.type == CardManager.CardType.CorruptedHeart || item.type == CardManager.CardType.TinyHeart)
                            {
                                item.HitItem();
                                DOTween.Restart("camera_shake_1");
                            }
                        }
                    }
                }
            }
            if (boltChainItems[j].y-1 >= start)
            {
                if (items[boltChainItems[j].x, boltChainItems[j].y-1] != null)
                {
                    Item item = items[boltChainItems[j].x, boltChainItems[j].y-1];
                    if (item.player)
                    {
                        if (!boltChainItems.Contains(item))
                        {
                            boltChainItems.Add(item);
                            if (item.type == CardManager.CardType.Heart || item.type == CardManager.CardType.CorruptedHeart || item.type == CardManager.CardType.TinyHeart)
                            {
                                item.HitItem();
                                DOTween.Restart("camera_shake_1");
                            }
                        }
                    }
                }
            }
        }

        yield return new WaitForEndOfFrame();
    }

    public void ComputeConnections()
    {


        int start = Mathf.FloorToInt(maxWidth * 0.5f) - ((currentZone + 1));
        int max = start + (currentZone + 1) * 2;
        
        for (int h = 0;h<connectedItems.Count;h++)
        {
            connectedItems[h].Disconnect();
            connectedItems[h].coFrom = null;
            connectedItems[h].coTos.Clear();
        }

        connectedItems.Clear();

        for (int i = 0; i < hearts.Count;i++)
        {
            Item heartItem = hearts[i].GetComponent<Item>();
            heartItem.Connect();
            heartItem.coFrom = heartItem;
            connectedItems.Add(heartItem);
        }

        for (int j = 0; j < connectedItems.Count;j++)
        {

            if (connectedItems[j].x + 1 < max)
            {
                if (items[connectedItems[j].x + 1, connectedItems[j].y] != null)
                {
                    if (items[connectedItems[j].x + 1, connectedItems[j].y].player)
                    {
                        items[connectedItems[j].x + 1, connectedItems[j].y].coFrom = connectedItems[j];
                        items[connectedItems[j].x + 1, connectedItems[j].y].Connect();
                        if (!connectedItems.Contains(items[connectedItems[j].x + 1, connectedItems[j].y]))
                        {
                            connectedItems[j].coTos.Add(items[connectedItems[j].x + 1, connectedItems[j].y]);
                            connectedItems.Add(items[connectedItems[j].x + 1, connectedItems[j].y]);
                        }
                    }
                }
            }
            if (connectedItems[j].x - 1 >= start)
            {
                if (items[connectedItems[j].x - 1, connectedItems[j].y] != null)
                {
                    if (items[connectedItems[j].x - 1, connectedItems[j].y].player)
                    {
                        items[connectedItems[j].x - 1, connectedItems[j].y].coFrom = connectedItems[j];
                        items[connectedItems[j].x - 1, connectedItems[j].y].Connect();
                        if (!connectedItems.Contains(items[connectedItems[j].x - 1, connectedItems[j].y]))
                        {
                            connectedItems[j].coTos.Add(items[connectedItems[j].x - 1, connectedItems[j].y]);
                            connectedItems.Add(items[connectedItems[j].x - 1, connectedItems[j].y]);                        
                        }
                    }
                }
            }
            if (connectedItems[j].y + 1 < max)
            {
                if (items[connectedItems[j].x, connectedItems[j].y+1] != null)
                {
                    if (items[connectedItems[j].x, connectedItems[j].y + 1].player)
                    {
                        items[connectedItems[j].x, connectedItems[j].y + 1].coFrom = connectedItems[j];
                        items[connectedItems[j].x, connectedItems[j].y + 1].Connect();
                        if (!connectedItems.Contains(items[connectedItems[j].x, connectedItems[j].y+1]))
                        {
                            connectedItems[j].coTos.Add(items[connectedItems[j].x, connectedItems[j].y + 1]);
                            connectedItems.Add(items[connectedItems[j].x, connectedItems[j].y + 1]);
                        }
                    }
                }
            }
            if (connectedItems[j].y - 1 >= start)
            {
                if (items[connectedItems[j].x, connectedItems[j].y- 1] != null)
                {
                    if (items[connectedItems[j].x, connectedItems[j].y - 1].player)
                    {
                        items[connectedItems[j].x, connectedItems[j].y - 1].coFrom = connectedItems[j];
                        items[connectedItems[j].x, connectedItems[j].y - 1].Connect();
                        if (!connectedItems.Contains(items[connectedItems[j].x, connectedItems[j].y-1]))
                        {
                            connectedItems[j].coTos.Add(items[connectedItems[j].x, connectedItems[j].y - 1]);
                            connectedItems.Add(items[connectedItems[j].x, connectedItems[j].y - 1]);
                        }
                    }
                }
            }
        }
    }


}

[System.Serializable]
public class BoardConfig
{
    public List<SpawnItem> spawnItems;
    public List<SpawnLock> spawnLocks;

}

[System.Serializable]
public class SpawnItem
{
    public GameObject item;
    public Vector2 pos;
}

[System.Serializable]
public class SpawnLock
{
    public int zone;
    public Vector2 pos;
}

[System.Serializable]
public class LockList
{
    public List<Lock> locks = new List<Lock>();
}
