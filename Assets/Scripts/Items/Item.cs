using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public CardManager.CardType type;
    public bool player;
    public int x, y;

    public Item coFrom;
    public List<Item> coTos = new List<Item>();
    public bool connected;


    public void HitItem()
    {
        if (type == CardManager.CardType.Lock)
        {
            GetComponent<Lock>().OpenLock();
        }

        if (type == CardManager.CardType.Heart || type == CardManager.CardType.CorruptedHeart || type == CardManager.CardType.TinyHeart)
        {
            GetComponent<Heart>().HitHeart();
        }


        if (type == CardManager.CardType.Bomb)
        {
            GetComponent<Bomb>().Explode();
        }
    }

    public void DestroyItem()
    {
        BoardManager.Instance.items[x, y] = null;
        BoardManager.Instance.ComputeConnections();
        Destroy(this);
    }
}
