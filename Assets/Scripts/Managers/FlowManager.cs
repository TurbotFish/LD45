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

    public SpriteRenderer overlay;
    public Color overlayOn, overlayOff;
    public float overlaySpeed;

    public void SetState(GameState s)
    {
        state = s;
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
        overlay.color = overlayOff;
        SetState(GameState.Idle);
    }


    public IEnumerator OverlayIn()
    {
        float counter = 0;
        while (counter<1)
        {
            counter += Time.deltaTime * overlaySpeed;
            float t = Mathf.Sin(counter * Mathf.PI * 0.5f);
            overlay.color = Color.Lerp(overlayOff, overlayOn, t);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator OverlayOut()
    {
        float counter = 0;
        while (counter < 1)
        {
            counter += Time.deltaTime * overlaySpeed;
            float t = Mathf.Sin(counter * Mathf.PI * 0.5f);
            overlay.color = Color.Lerp(overlayOn, overlayOff, t);
            yield return new WaitForEndOfFrame();
        }
    }
}
