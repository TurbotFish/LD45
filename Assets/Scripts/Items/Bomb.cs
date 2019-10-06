using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public void Explode()
    {
        Item item = GetComponent<Item>();
        StartCoroutine(BoardManager.Instance.BombAttack(item.x, item.y));
    }
}
