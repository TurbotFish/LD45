using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    protected static FlowManager _Instance;
    public static FlowManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<FlowManager>();
            }
            return _Instance;
        }
    }

    public enum GameState { Idle, Casting, Resolving, ChoosingCard}

    public GameState state;

    public void SetState(GameState s)
    {
        switch (s)
        {
            case GameState.Idle:
                Debug.Log("Idle");
                break;
            case GameState.Casting:
                Debug.Log("Casting");
                break;
            case GameState.Resolving:
                Debug.Log("Resolving");
                break;
            case GameState.ChoosingCard:
                Debug.Log("ChoosingCard");
                break;
        }
    }

    private void Start()
    {
        StartCoroutine(CardManager.Instance.DrawHeart());

    }
}
