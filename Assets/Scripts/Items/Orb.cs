using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public void DestroyOrb()
    {
        CardManager.Instance.orbCount--;

        if (CardManager.Instance.orbCount<CardManager.Instance.maxOrbs)
        {
            StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.cardsInHand[CardManager.Instance.cardsInHand.Count - 1].transform, false));

        }
    }
}
