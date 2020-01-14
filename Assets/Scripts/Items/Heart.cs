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
            StartCoroutine(DestroyHeart());
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

    public IEnumerator DestroyHeart()
    {
        Item item = GetComponent<Item>();

        BoardManager.Instance.hearts.Remove(this);

        if (item.type == CardManager.CardType.CorruptedHeart)
        {
            CardManager.Instance.corruptedHeartCount--;
            if (CardManager.Instance.corruptedHeartCount < CardManager.Instance.maxCorruptedHearts)
            {
                //StartCoroutine(CardManager.Instance.DrawCard());
                yield return new WaitForSecondsRealtime(0.25f);
                CardManager.Instance.Draw();
            }
        }

        if (BoardManager.Instance.hearts.Count==0)
        {

            StartCoroutine(FlowManager.Instance.GameOver());
        }
        item.DestroyItem();
    }
}
