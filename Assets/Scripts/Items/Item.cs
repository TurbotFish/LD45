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
    public SpriteRenderer sprite;
    public Color connectedColor, disconnectedColor;

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
            StartCoroutine(BoardManager.Instance.BombAttack(x, y));
            DestroyItem();
        }

        if (type == CardManager.CardType.Bolt)
        {
            StartCoroutine(BoardManager.Instance.BoltAttack(x, y));
        }


        if (type == CardManager.CardType.Heal || type == CardManager.CardType.Sword)
        {
            DestroyItem();
        }

        if (type == CardManager.CardType.Orb)
        {
            GetComponent<Orb>().DestroyOrb();
        }

        BoardManager.Instance.ComputeConnections();

    }

    public void DestroyItem()
    {
        BoardManager.Instance.items[x, y] = null;
        BoardManager.Instance.ComputeConnections();
        Destroy(this.gameObject);
    }

    public void Disconnect()
    {
        connected = false;
        sprite.color = disconnectedColor;
    }

    public void Connect()
    {
        connected = true;
        sprite.color = connectedColor;
    }

    public void Consume()
    {

        BoardManager.Instance.InstantiateFX(x, y, CardManager.Instance.consumeFX, 1.5f);

        if (type == CardManager.CardType.Heart ||
            type == CardManager.CardType.TinyHeart ||
            type == CardManager.CardType.CorruptedHeart)
        {
            HitItem();
        }
        else
        {
            DestroyItem();
        }
    }

}
