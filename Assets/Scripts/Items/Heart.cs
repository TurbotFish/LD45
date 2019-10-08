using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int health;
    public Animator heartAnim;
    int startHealth;

    private void Start()
    {
        heartAnim.SetInteger("hp", health);
        startHealth = health;
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
        health = (health >= startHealth) ? startHealth : health + 1;
        heartAnim.SetInteger("hp", health);
    }

    public void DestroyHeart()
    {
        Item item = GetComponent<Item>();
        if (item.type == CardManager.CardType.CorruptedHeart)
        {
            CardManager.Instance.corruptedHeartCount--;
        }

        BoardManager.Instance.hearts.Remove(this);
        if (BoardManager.Instance.hearts.Count==0)
        {

            StartCoroutine(FlowManager.Instance.GameOver());
        }
        item.DestroyItem();
    }
}
