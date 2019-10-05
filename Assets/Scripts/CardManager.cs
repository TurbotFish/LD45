using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    protected static CardManager _Instance;

    public static CardManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<CardManager>();
            }
            return _Instance;
        }
    }

    public Card selectedCard;
    public bool canSelectCard;


}
