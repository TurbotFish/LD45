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
    public List<Animator> lockZoneAnims = new List<Animator>();
    public List<LockList> lockLists = new List<LockList>();

    public GameObject p_fog;
    public Transform fogParent;
    SpriteRenderer[,] fogs = new SpriteRenderer[10, 10];

    public GameObject p_cell;
    public Transform cellsParent;
    GameObject[,] cells = new GameObject[10, 10];
    public Color cellHoverColor, cellDefaultColor;
    public Item[,] items = new Item[10, 10];
    List<GameObject> storeItems = new List<GameObject>();
    public List<Heart> hearts = new List<Heart>();

    public List<Item> connectedItems = new List<Item>();
    List<Item> boltChainItems = new List<Item>();

    public float arrowSpeed;

    public List<BoardConfig> configs = new List<BoardConfig>();



    public void ResetBoard(int configID)
    {
        foreach (LockList ll in lockLists)
        {
            ll.locks.Clear();
        }
        foreach (GameObject g in storeItems)
        {
            Destroy(g);
        }

        connectedItems.Clear();

        hearts.Clear();

        HideCells();

        InitializeItems(configID);
        InitializeFog(false);
        InitializeZones();
    }


    public void InitializeFog(bool instantiate)
    {
        for (int x = 0; x < maxWidth;x++)
        {
            for (int y = 0; y < maxWidth; y++)
            {
                if (instantiate)
                {
                    Vector3 fogPos = fogParent.position + new Vector3(x * step, y * step, 0);
                    GameObject fog = Instantiate(p_fog, fogPos, Quaternion.identity, fogParent) as GameObject;
                    fogs[x, y] = fog.GetComponent<SpriteRenderer>();
                }
                SetFogOpacityAt(x, y, 1);
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
        if (x >= 0 && y >= 0 && x < maxWidth && y < maxWidth)
        {
            fogs[x, y].color = new Color(fogs[x, y].color.r, fogs[x, y].color.g, fogs[x, y].color.b, a);
        }
    }

    public void CheckIfCurrentZoneOpen()
    {
        bool open = true;
        foreach(Lock l in lockLists[currentZone].locks)
        {
            open = l.open;
            if (!open)
            {
                break;
            }
        }
        if (open)
        {
            lockZoneAnims[currentZone].SetBool("dead", true);
            currentZone++;
            //lockZoneAnims[currentZone].SetTrigger("revive");

            StartCoroutine(OpenZone(currentZone, 1));
        }
    }
    
    public void InitializeZones()
    {
        currentZone = 0;
        foreach(Animator a in lockZoneAnims)
        {
            a.SetBool("dead", true);
        }
        lockZones[currentZone].sortingLayerName = "Default";
        lockZoneAnims[currentZone].SetBool("dead", false); ;

    }

    public IEnumerator OpenZone(int i, float time)
    {
        if (i > 4)
        {
            StartCoroutine(FlowManager.Instance.Win());
        }
        else
        {
            lockZoneAnims[currentZone].SetBool("dead", false); ;
            lockZones[currentZone].sortingLayerName = "AboveFog";

            int start = Mathf.Clamp(Mathf.FloorToInt(maxWidth * 0.5f) - (currentZone + 1), 0, maxWidth);
            int max = start + (currentZone + 1) * 2;

            for (int x = start; x < max; x++)
            {
                for (int y = start; y < max; y++)
                {
                    SetFogOpacityAt(x, y, 0);
                }
            }

            for (int j = start - 1; j < max + 1; j++)
            {
                SetFogOpacityAt(start - 1, j, 0.8f);
                SetFogOpacityAt(max, j, 0.8f);
                SetFogOpacityAt(j, start - 1, 0.8f);
                SetFogOpacityAt(j, max, 0.8f);
            }
            lockZoneAnims[currentZone].SetBool("dead", false); ;

            yield return new WaitForSeconds(time);

            if (i <= 4 && i != 0 && !FlowManager.Instance.tuto)
            {
                StartCoroutine(CardManager.Instance.PickCard(0, 2));
            }
            else if (i>4)
            {
                StartCoroutine(FlowManager.Instance.Win());

            }
        }
        
    }


    public void HighlightAdjacents()
    {
        ComputeConnections();

        int start = Mathf.Clamp(Mathf.FloorToInt(maxWidth * 0.5f) - (currentZone + 1), 0, maxWidth);
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

        int start = Mathf.Clamp(Mathf.FloorToInt(maxWidth * 0.5f) - (currentZone + 1), 0, maxWidth);
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

    public void HighlightSpecificCell(int x, int y)
    {
        cells[x, y].SetActive(true);
    }

    public void HideCells()
    {
        foreach(GameObject go in cells)
        {
            go.SetActive(false);
            go.GetComponent<Cell>().spriteRenderer.color = cellDefaultColor;
        }
    }



    public void InstantiateItem(int x, int y, GameObject goRef)
    {
        GameObject go = Instantiate(goRef, cells[x, y].transform.position, Quaternion.identity, boardParent) as GameObject;
        storeItems.Add(go);
        Item item = go.GetComponent<Item>();

        item.x = x;
        item.y = y;
        items[x, y] = item;

        item.Disconnect();
        ComputeConnections();

    }

    public void InstantiateLock(int x, int y, int lockListID)
    {
        GameObject go = Instantiate(lockPrefab, cells[x, y].transform.position, Quaternion.identity, boardParent) as GameObject;
        Item item = go.GetComponent<Item>();
        storeItems.Add(go);
        item.x = x;
        item.y = y;
        items[x, y] = item;
        lockLists[lockListID].locks.Add(go.GetComponent<Lock>());

    }

    public IEnumerator SwordAttack(int x, int y)
    {
        int start = Mathf.Clamp(Mathf.FloorToInt(maxWidth * 0.5f) - (currentZone + 1), 0, maxWidth);
        int max = start + (currentZone + 1) * 2;

        if (x+1 < max)
        {
            if (items[x + 1,y]!=null)
            {
                items[x + 1, y].HitItem();
                DOTween.Restart("camera_shake_1");
            }
            InstantiateFX(x + 1, y, CardManager.Instance.swordFX, 1.5f);

        }
        if (x - 1 >= start)
        {
            if (items[x - 1, y] != null)
            {
                items[x - 1, y].HitItem();
                DOTween.Restart("camera_shake_1");
            }
            InstantiateFX(x - 1, y, CardManager.Instance.swordFX, 1.5f);

        }
        if (y + 1 < max)
        {
            if (items[x, y+1] != null)
            {
                items[x, y+1].HitItem();
                DOTween.Restart("camera_shake_1");
            }
            InstantiateFX(x, y + 1, CardManager.Instance.swordFX, 1.5f);

        }
        if (y - 1 >= start)
        {
            if (items[x, y-1] != null)
            {
                items[x, y-1].HitItem();
                DOTween.Restart("camera_shake_1");
            }
            InstantiateFX(x, y - 1, CardManager.Instance.swordFX, 1.5f);

        }
        yield return new WaitForEndOfFrame();
        ComputeConnections();
    }

    public IEnumerator BombAttack(int x, int y)
    {
        int start = Mathf.Clamp(Mathf.FloorToInt(maxWidth * 0.5f) - (currentZone + 1), 0, maxWidth);
        int max = start + (currentZone + 1) * 2;

        if (x + 1 < max)
        {
            if (items[x + 1, y] != null)
            {
                items[x + 1, y].HitItem();
                DOTween.Restart("camera_shake_1");
            }
            InstantiateFX(x + 1, y, CardManager.Instance.bombFX, 1.5f);

        }
        if (x - 1 >= start)
        {
            if (items[x - 1, y] != null)
            {
                items[x - 1, y].HitItem();
                DOTween.Restart("camera_shake_1");
            }
            InstantiateFX(x - 1, y, CardManager.Instance.bombFX, 1.5f);

        }
        if (y + 1 < max)
        {
            if (items[x, y + 1] != null)
            {
                items[x, y + 1].HitItem();
                DOTween.Restart("camera_shake_1");
            }
            InstantiateFX(x, y + 1, CardManager.Instance.bombFX, 1.5f);

        }
        if (y - 1 >= start)
        {
            if (items[x, y - 1] != null)
            {
                items[x, y - 1].HitItem();
                DOTween.Restart("camera_shake_1");
            }
            InstantiateFX(x, y - 1, CardManager.Instance.bombFX, 1.5f);

        }
        yield return new WaitForEndOfFrame();
        ComputeConnections();

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
            InstantiateFX(x + 1, y, CardManager.Instance.healFX, 1.5f);

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
            InstantiateFX(x - 1, y, CardManager.Instance.healFX, 1.5f);

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
            InstantiateFX(x, y + 1, CardManager.Instance.healFX, 1.5f);

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
            InstantiateFX(x, y - 1, CardManager.Instance.healFX, 1.5f);

        }
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator BoltAttack (int x, int y)
    {
        int start = Mathf.Clamp(Mathf.FloorToInt(maxWidth * 0.5f) - (currentZone + 1), 0, maxWidth);
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
                            InstantiateFX(item.x, item.y, CardManager.Instance.lightningFX, 1.5f);
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
                            InstantiateFX(item.x, item.y, CardManager.Instance.lightningFX, 1.5f);
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
                            InstantiateFX(item.x, item.y, CardManager.Instance.lightningFX, 1.5f);
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
                            InstantiateFX(item.x, item.y, CardManager.Instance.lightningFX, 1.5f);
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

        boltChainItems[0].DestroyItem();
    }

    public IEnumerator ArrowAttack (int x, int y)
    {
        int start = Mathf.Clamp(Mathf.FloorToInt(maxWidth * 0.5f) - (currentZone + 1), 0, maxWidth);
        int max = start + (currentZone + 1) * 2;

        Vector3 arrowSpawn = cells[x,y].transform.position;
        List<Vector3> arrowGoals = new List<Vector3>();
        List<GameObject> arrows = new List<GameObject>();
        List<Item> contactItems = new List<Item>();

        GameObject go1 = Instantiate(CardManager.Instance.arrowPrefab, arrowSpawn, Quaternion.identity) as GameObject;
        go1.transform.eulerAngles = new Vector3(0, 0, -90f);
        arrows.Add(go1);
        for (int up = y; up < max;up++)
        {
            if (items[x,up] != null)
            {
                Item item = items[x, up];
                arrowGoals.Add(new Vector3(item.transform.position.x, item.transform.position.y - (step * 0.01f)));
                contactItems.Add(item);
                break;
            }
            else if (up == max-1)
            {
                arrowGoals.Add(new Vector3(cells[x, max].transform.position.x - (step * 0.01f), cells[x, max].transform.position.y));
                contactItems.Add(null);
            }
        }

        GameObject go2 = Instantiate(CardManager.Instance.arrowPrefab, arrowSpawn, Quaternion.identity) as GameObject;
        go2.transform.eulerAngles = new Vector3(0, 0, 90f);
        arrows.Add(go2);
        for (int down = y; down >= start; down--)
        {
            if (items[x, down] != null)
            {
                Item item = items[x, down];
                arrowGoals.Add(new Vector3(item.transform.position.x, item.transform.position.y + (step * 0.01f)));
                contactItems.Add(item);
                break;
            }
            else if (down == start)
            {
                arrowGoals.Add(new Vector3(cells[x, start].transform.position.x - (step * 0.01f), cells[x, start].transform.position.y));
                contactItems.Add(null);
            }
        }

        GameObject go3 = Instantiate(CardManager.Instance.arrowPrefab, arrowSpawn, Quaternion.identity) as GameObject;
        go3.transform.eulerAngles = new Vector3(0, 0, 0);
        arrows.Add(go3);
        for (int left = x; left >= start; left--)
        {
            if (items[left, y] != null)
            {
                Item item = items[left, y];
                arrowGoals.Add(new Vector3(item.transform.position.x + (step * 0.01f), item.transform.position.y));
                contactItems.Add(item);
                break;
            }
            else if (left == start)
            {
                arrowGoals.Add(new Vector3(cells[start, y].transform.position.x - (step * 0.01f), cells[start, y].transform.position.y));
                contactItems.Add(null);
            }
        }

        GameObject go4 = Instantiate(CardManager.Instance.arrowPrefab, arrowSpawn, Quaternion.identity) as GameObject;
        go4.transform.eulerAngles = new Vector3(0, 0, 180f);
        arrows.Add(go4);
        for (int right = x; right < max; right++)
        {
            if (items[right, y] != null)
            {
                Item item = items[right, y];
                arrowGoals.Add(new Vector3(item.transform.position.x - (step * 0.01f), item.transform.position.y));
                contactItems.Add(item);
                break;
            }
            else if (right == max - 1)
            {
                arrowGoals.Add(new Vector3(cells[max,y].transform.position.x - (step * 0.01f), cells[max, y].transform.position.y));
                contactItems.Add(null);
            }
        }

        float counter = 0;

        while (counter <1)
        {
            counter += Time.deltaTime * arrowSpeed;
            for (int i = 0; i < arrows.Count; i++)
            {
                if (arrows[i] != null)
                {
                    arrows[i].transform.position = Vector3.Lerp(arrowSpawn, arrowGoals[i], counter * maxWidth/(Vector3.Distance(arrowSpawn, arrowGoals[i])));
                }
                if (counter * maxWidth / (Vector3.Distance(arrowSpawn, arrowGoals[i])) >= 1)
                {              
                    if (contactItems[i] != null && arrows[i] !=null)
                    {
                        contactItems[i].HitItem();
                        InstantiateFX(contactItems[i].x, contactItems[i].y, CardManager.Instance.explosionFX, 1.5f);
                    }
                    Destroy(arrows[i]);

                }
            }
            yield return new WaitForEndOfFrame();

        }

        ComputeConnections();





    }

    public void InstantiateFX(int x, int y, GameObject goFX, float duration)
    {
        GameObject go = Instantiate(goFX, cells[x, y].transform.position, Quaternion.identity, boardParent) as GameObject;
        Destroy(go,duration);
    }

    public void ComputeConnections()
    {


        int start = Mathf.Clamp(Mathf.FloorToInt(maxWidth * 0.5f) - (currentZone + 1),0,maxWidth);
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

        foreach(Item it in connectedItems)
        {
            it.Connect();
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
