﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int health;
    public Animator heartAnim;

    private void Start()
    {
        heartAnim.SetInteger("hp", health);
    }

    public void HitHeart()
    {
        health--;
        if (health <= 0)
        {
            DestroyHeart();
        }
        else
        {
            heartAnim.SetInteger("hp", health);
        }
    }

    public void Heal()
    {
        health = (health >= 2) ? 2 : health + 1;
        heartAnim.SetInteger("hp", health);
    }

    public void DestroyHeart()
    {
        Item item = GetComponent<Item>();
        if (item.type == CardManager.CardType.CorruptedHeart)
        {
            CardManager.Instance.corruptedHeartCount--;
        }
    }
}
