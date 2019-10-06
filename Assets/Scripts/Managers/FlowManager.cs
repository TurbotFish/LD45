﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public GameObject menu, thankYou;
    public ScreenTransitionImageEffect screenTransition;

    public List<GameObject> tutoTexts = new List<GameObject>();
    public int tutoStep;
    public bool tuto = true;
    public bool noInput;
    public GameObject gameOverText;

    public void SetState(GameState s)
    {
        state = s;
        switch (s)
        {
            case GameState.Idle:
                //Debug.Log("Idle");
                break;
            case GameState.Casting:
                //Debug.Log("Casting");
                break;
            case GameState.Resolving:
                //Debug.Log("Resolving");
                break;
            case GameState.ChoosingCard:
                //Debug.Log("ChoosingCard");
                break;
        }
    }

    private void Start()
    {
        overlay.color = overlayOff;
        SetState(GameState.Idle);

        BoardManager.Instance.InitializeFog(true);
        BoardManager.Instance.InitializeZones();
        BoardManager.Instance.InitializeCells();
        //BoardManager.Instance.InitializeItems(0);
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
    public IEnumerator ScreenTransition(float speed, float inBetweenTime, bool menuState)
    {
        //yield return new WaitForSeconds(inBetweenTime*0.25f);

        float counter = 0;
        while (counter < 1)
        {
            counter += Time.deltaTime * speed;
            float t = Mathf.Sin(counter * Mathf.PI * 0.5f);
            screenTransition.maskValue = Mathf.Lerp(0,1,t);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(inBetweenTime);
        menu.SetActive(menuState);

        float counter2 = 0;
        while (counter2 < 1)
        {
            counter2 += Time.deltaTime * speed;
            float t = Mathf.Sin(counter2 * Mathf.PI * 0.5f);
            screenTransition.maskValue = Mathf.Lerp(1, 0, t);
            yield return new WaitForEndOfFrame();
        }

        if (tuto)
        {
            noInput = true;
            yield return new WaitForSeconds(0.01f);
            DOTween.Restart("tuto_01");
            yield return new WaitForSeconds(1);
            tutoTexts[0].SetActive(true);
            noInput = false;
            tutoStep = 1;
        }
        else
        {
            noInput = false;
        }
    }

    public void PlayGameFromMenu()
    {
        StartCoroutine(ScreenTransition(1.5f, 0.75f, false));
        BoardManager.Instance.ResetBoard(0);
        StartCoroutine(BoardManager.Instance.OpenZone(BoardManager.Instance.currentZone, 0));
        CardManager.Instance.ResetDeck();
        StartCoroutine(CardManager.Instance.DrawHeart());

    }

    public IEnumerator GameOver()
    {
        StopAllCoroutines();
        BoardManager.Instance.StopAllCoroutines();
        CardManager.Instance.StopAllCoroutines();


        if (tuto)
        {
            StartCoroutine(QuitTuto());
        }
        StartCoroutine(ScreenTransition(1.5f, 2f, true));
        yield return new WaitForSeconds(0.3f);
        gameOverText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        gameOverText.SetActive(false);


    }


    public IEnumerator Win()
    {
        StopAllCoroutines();
        BoardManager.Instance.StopAllCoroutines();
        CardManager.Instance.StopAllCoroutines();

        StartCoroutine(ScreenTransition(1.5f, 2f, true));
        yield return new WaitForSeconds(0.5f);
        menu.SetActive(false);
        thankYou.SetActive(true);

    }

    //casting heart


    public IEnumerator TutoStepOne()
    {
        //Debug.Log("one");
        tutoStep = 1;

        SetState(GameState.Idle);
        tutoTexts[1].SetActive(false);
        tutoTexts[2].SetActive(false);
        tutoTexts[3].SetActive(false);

        DOTween.Restart("tuto_01");
        yield return new WaitForSeconds(0.5f);
        tutoTexts[0].SetActive(true);
    }

    public IEnumerator TutoStepTwo()
    {
        //Debug.Log("two");
        tutoStep = 2;

        SetState(GameState.Casting);
        tutoTexts[0].SetActive(false);
        tutoTexts[2].SetActive(false);
        tutoTexts[3].SetActive(false);
        DOTween.Restart("tuto_02");
        BoardManager.Instance.HighlightSpecificCell(4, 5);
        yield return new WaitForSeconds(0.5f);
        tutoTexts[1].SetActive(true);
    }

    public IEnumerator TutoStepThree(bool draw)
    {
        //Debug.Log("three");
        if (draw)
        {
            DOTween.Restart("tuto_out");
        }
        tutoStep = 3;
        SetState(GameState.Idle);
        if (draw)
        {
            StartCoroutine(CardManager.Instance.DrawCard());
        }
        tutoTexts[0].SetActive(false);
        tutoTexts[1].SetActive(false);
        tutoTexts[3].SetActive(false);
        yield return new WaitForSeconds(1);
        DOTween.Restart("tuto_03");
        yield return new WaitForSeconds(0.5f);
        tutoTexts[2].SetActive(true);
    }

    public IEnumerator TutoStepThreeHalf()
    {
        //Debug.Log("threeHalf");
        tutoStep = 3;
        SetState(GameState.Casting);
        BoardManager.Instance.HighlightAdjacents();
        tutoTexts[0].SetActive(false);
        tutoTexts[2].SetActive(false);
        tutoTexts[1].SetActive(false);
        tutoTexts[3].SetActive(false);
        DOTween.Restart("tuto_out");
        yield return new WaitForSeconds(0.01f);
    }

    public IEnumerator TutoStepFour()
    {
        //Debug.Log("four");
        tutoStep = 4;
        SetState(GameState.Idle);
        tutoTexts[0].SetActive(false);
        tutoTexts[1].SetActive(false);
        tutoTexts[2].SetActive(false);
        DOTween.Restart("tuto_04");
        yield return new WaitForSeconds(0.5f);
        tutoTexts[3].SetActive(true);

    }

    public IEnumerator QuitTuto()
    {
        //Debug.Log("Quit");
        tutoStep = 5;
        tuto = false;
        tutoTexts[0].SetActive(false);
        tutoTexts[1].SetActive(false);
        tutoTexts[2].SetActive(false);
        tutoTexts[3].SetActive(false);
        DOTween.Restart("tuto_out");
        yield return new WaitForSeconds(1);

        StartCoroutine(CardManager.Instance.PickCard(0, 2));

    }
}
