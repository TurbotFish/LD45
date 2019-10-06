using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "ScriptableObjects/CardDatabase", order = 1)]
public class CardDatabase : ScriptableObject
{
    public List<CardTier> cardTiers = new List<CardTier>();

}

[System.Serializable]
public class CardTier
{
    public List<CardElement> cards = new List<CardElement>();
}

[System.Serializable]
public class CardElement
{
    public GameObject cardGO;
    public float weight;
}
