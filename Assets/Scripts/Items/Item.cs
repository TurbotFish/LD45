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


        if (type == CardManager.CardType.Heal || type == CardManager.CardType.Sword || type == CardManager.CardType.Orb)
        {
            DestroyItem();
        }

        if (type == CardManager.CardType.Shield)
        {
            SoundManager.Instance.PlaySound(2, SoundManager.Instance.sfx_shield,0.1f);
        }



        BoardManager.Instance.ComputeConnections();

    }

    public void DestroyItem()
    {

        BoardManager.Instance.items[x, y] = null;
        BoardManager.Instance.ComputeConnections();
        if (type == CardManager.CardType.Orb)
        {
            CardManager.Instance.DestroyOrb();
        }
        Destroy(this.gameObject);
    }

    public void Disconnect()
    {
        if (type != CardManager.CardType.CorruptedHeart &&
            type != CardManager.CardType.TinyHeart &&
            type != CardManager.CardType.Heart &&
            type != CardManager.CardType.Bolt &&
            type != CardManager.CardType.Rock &&
            type != CardManager.CardType.Bomb &&
            type != CardManager.CardType.Lock)
        {
            connected = false;
            if (sprite!= null)
                sprite.color = disconnectedColor;
        }

    }

    public void Connect()
    {
        connected = true;
        if (sprite!= null)
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
