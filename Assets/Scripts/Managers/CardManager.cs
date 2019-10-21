using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public enum CardType { Heart, CorruptedHeart, TinyHeart, Sword, Heal, Shield, Arrow, Consume, Orb, Bomb, Bolt, Rock, PlusCard, Lock, Bubble}

    public CardDatabase cardDB;
    public List<GameObject> baseDeck = new List<GameObject>();
    public Card selectedCard;
    public bool canSelectCard;

    public List<GameObject> cardsInDeck = new List<GameObject>();
    public int totalCards;
    public TextMeshPro cardNumber;
    public float drawSpeed;
    public Transform deckParent;
    public Animator deckAnim;


    public List<GameObject> cardsInDiscard = new List<GameObject>();
    public Transform discardParent;
    int posCycle;
    public List<Vector3> discardPositions = new List<Vector3>();
    public float discardSpeed;
    public Transform discardHiddenPos;
    public float shuffleSpeed;
    public float shuffleSpace;

    public List<GameObject> cardsInHand = new List<GameObject>();
    public Transform cardHandSpawn;
    public Transform heartHandStore;
    public GameObject heartCard;
    public int handSize;
    public float cardSpace;
    public Transform handParent;
    List<Vector3> cardPositions = new List<Vector3>();

    float counter1;
    float counter2;
    float counter3;

    public int orbCount;
    public int maxOrbs = 2;

    public int corruptedHeartCount;
    public int maxCorruptedHearts = 2;

    public Transform cardPickSpawn;
    public float pickYpos, pickYhide, handYhidePos;
    public float cardPickSpace;
    List<GameObject> cardPickGOs = new List<GameObject>();
    public float cardPickAnimSpeed;
    public Animator lockPickAnim;
    public Transform pickCardText;

    public GameObject consumeFX;
    public GameObject swordFX;
    public GameObject explosionFX;
    public GameObject lightningFX;
    public GameObject bombFX;
    public GameObject healFX;
    public GameObject arrowPrefab;

    bool handUp;
    bool cardPick;
    Vector3 handDefaultPos;

    private void Start()
    {
        handDefaultPos = handParent.position;
    }

    public void ResetDeck()
    {
        handUp = true;
        handParent.position = handDefaultPos;
        handSize = 0;
        foreach(GameObject g in cardsInDeck)
        {
            Destroy(g);
        }
        cardsInDeck.Clear();
        foreach (GameObject g in cardsInDiscard)
        {
            Destroy(g);
        }
        cardsInDiscard.Clear();
        foreach (GameObject g in cardsInHand)
        {
            Destroy(g);
        }
        cardsInHand.Clear();

        totalCards = 0;
        
        for (int i = 0; i <baseDeck.Count;i++)
        {
            GameObject go = Instantiate(baseDeck[i], cardHandSpawn.position, Quaternion.identity, deckParent) as GameObject;
            cardsInDeck.Add(go);
            totalCards++;
        }

        UpdateCardNumber();
    }

    public void UpdateCardNumber()
    {
        cardNumber.text = cardsInDeck.Count.ToString() + " / " + totalCards.ToString();

    }

    public IEnumerator DrawCard()
    {
        if (handSize<5)
        {
            if (cardsInDeck.Count == 0)
            {
                StartCoroutine(ShuffleDeck());
            }

            handSize++;

            Transform drawnCard = cardsInDeck[0].transform;
            drawnCard.position = cardHandSpawn.position;
            drawnCard.parent = handParent;

            cardsInDeck.RemoveAt(0);
            cardsInHand.Add(drawnCard.gameObject);

            List<Vector3> cardOldPositions = new List<Vector3>();
            for (int i = 0; i < handSize; i++)
            {
                cardOldPositions.Add(cardsInHand[i].transform.localPosition);
            }


            cardPositions.Clear();
            for (int i = 0; i < handSize; i++)
            {
                cardPositions.Add((handParent.right * -1) * (handSize * cardSpace * 0.5f) + (handParent.right * (i + 0.5f) * cardSpace) + (handParent.right * -0.073f * (5 - handSize)));
            }

            yield return new WaitForSeconds(0.01f);
            counter1 = 0;
            while (counter1 < 1)
            {
                counter1 += Time.deltaTime * drawSpeed;
                for (int i = 0; i < handSize; i++)
                {
                    cardsInHand[i].transform.localPosition = Vector3.Lerp(cardOldPositions[i], cardPositions[i], counter1);
                }
                yield return new WaitForEndOfFrame();
            }



            UpdateCardNumber();
        }
        
    }

    public IEnumerator DrawHeart(float delay)
    {
        yield return new WaitForSeconds(delay);
        handSize++;
        Transform drawnCard = heartCard.transform;
        drawnCard.position = cardHandSpawn.position;
        drawnCard.parent = handParent;
        cardsInHand.Add(drawnCard.gameObject);

        List<Vector3> cardOldPositions = new List<Vector3>();
        for (int i = 0; i < handSize; i++)
        {
            cardOldPositions.Add(cardsInHand[i].transform.localPosition);
        }

        cardPositions.Clear();
        for (int i = 0; i < handSize; i++)
        {
            cardPositions.Add((handParent.right * -1) * (handSize * cardSpace * 0.5f) + (handParent.right * (i + 0.5f) * cardSpace) + (handParent.right*-0.073f*(5-handSize)));
        }

        counter1 = 0;
        while (counter1 < 1)
        {
            counter1 += Time.deltaTime * drawSpeed;
            for (int i = 0; i < handSize; i++)
            {
                if (cardsInHand[i] != null)
                {
                    cardsInHand[i].transform.localPosition = Vector3.Lerp(cardOldPositions[i], cardPositions[i], counter1);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator ShuffleDeck()
    {
        //yield return new WaitForSeconds(0.25f);
        for(int i = 0; i<cardsInDiscard.Count;i++)
        {
            cardsInDiscard[i].GetComponent<Card>().SetCardSpritesOnLayer(25625860, 10 * i);
            cardsInDiscard[i].GetComponent<Card>().discarded = false;
            cardsInDiscard[i].transform.parent = deckParent;

            for (int k = 0; k < cardsInDiscard.Count; k++)
            {
                GameObject temp = cardsInDiscard[k];
                int randomIndex = Random.Range(k, cardsInDiscard.Count);
                cardsInDiscard[k] = cardsInDiscard[randomIndex];
                cardsInDiscard[randomIndex] = temp;
            }

            for (int l = 0; l < cardsInDiscard.Count;l++)
            {
                cardsInDeck.Add(cardsInDiscard[l]);
                cardsInDeck[l].GetComponent<Card>().discarded = false;
            }

            List<Vector3> cardOldPositions = new List<Vector3>();
            for (int m = 0; m < cardsInDiscard.Count; m++)
            {
                cardOldPositions.Add(cardsInDiscard[m].transform.position);
            }
            counter3 = 0;
            while (counter3 < 1)
            {
                counter3 += Time.deltaTime * shuffleSpeed*cardsInDiscard.Count;
                for (int n = 0; n < cardsInDiscard.Count; n++)
                {
                    cardsInDiscard[n].transform.position = Vector3.Lerp(cardOldPositions[n], discardHiddenPos.position, counter3);
                }
                yield return new WaitForEndOfFrame();
            }

            cardsInDiscard.Clear();
            UpdateCardNumber();
        }
    }

    public IEnumerator Discard(Transform discardedCard, bool followedByDraw)
    {
        handSize--;

        discardedCard.parent = discardParent;

        Card dcard = discardedCard.GetComponent<Card>();
        dcard.SetCardSpritesOnLayer(888, 10 + cardsInDiscard.Count * 10);
        dcard.UnselectCard();

        selectedCard = null; // GOOD IDEA? OR NOT?


        cardsInDiscard.Add(discardedCard.gameObject);
        cardsInHand.Remove(discardedCard.gameObject);
        discardedCard.GetComponent<Card>().discarded = true;

        List<Vector3> cardOldPositions = new List<Vector3>();
        for (int i = 0; i < handSize; i++)
        {
            cardOldPositions.Add(cardsInHand[i].transform.position);
        }

        cardPositions.Clear();
        for (int i = 0; i < handSize; i++)
        {
            cardPositions.Add(handParent.position + (handParent.right * -1) * (handSize * cardSpace * 0.5f) + (handParent.right * (i + 0.5f) * cardSpace));
        }

        Vector3 discardedOldPos = discardedCard.position;

        counter1 = 0;
        while (counter1 < 1)
        {
            counter1 += Time.deltaTime * drawSpeed;
            discardedCard.position = Vector3.Lerp(discardedOldPos, discardParent.position + discardPositions[posCycle], counter1);
            yield return new WaitForEndOfFrame();
        }

        if (!followedByDraw)
        {
            counter2 = 0;
            while (counter2 < 1)
            {
                counter2 += Time.deltaTime * drawSpeed;
                for (int i = 0; i < handSize - 1; i++)
                {
                    cardsInHand[i].transform.position = Vector3.Lerp(cardOldPositions[i], cardPositions[i], counter2);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        posCycle = (posCycle == discardPositions.Count - 1) ? 0 : posCycle++;
    }

    public IEnumerator DiscardHeart(Transform discardedCard)
    {
        handSize--;

        discardedCard.parent = discardParent;
        CardManager.Instance.selectedCard.UnselectCard();
        CardManager.Instance.selectedCard = null;
        cardsInHand.Remove(discardedCard.gameObject);   

        Vector3 discardedOldPos = discardedCard.position;

        counter1 = 0;
        while (counter1 < 1)
        {
            counter1 += Time.deltaTime * drawSpeed;
            discardedCard.position = Vector3.Lerp(discardedOldPos, heartHandStore.position, counter1);
            yield return new WaitForEndOfFrame();
        }       

    }

    public IEnumerator PickCard(int tier, int picks)
    {
        cardPick = true;

        pickCardText.position = cardPickSpawn.position;

        FlowManager.Instance.SetState(FlowManager.GameState.ChoosingCard);
        StartCoroutine(HideHand());
        StartCoroutine(FlowManager.Instance.OverlayIn());
        yield return new WaitForSeconds(1);
        lockPickAnim.SetTrigger("unlock");

        List<GameObject> cardPicks = new List<GameObject>();
        
        float s = 0;
        foreach (CardElement c in cardDB.cardTiers[tier].cards)
        {
            s += c.weight;
        }

        for (int n = 0; n < picks;n++)
        {

            float p = Random.Range(0, s);
            float w = 0;
            for (int i = 0; i < cardDB.cardTiers[tier].cards.Count; i++)
            {
                w += cardDB.cardTiers[tier].cards[i].weight;
                if (p <= w)
                {
                    cardPicks.Add(cardDB.cardTiers[tier].cards[i].cardGO);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(2.5f);
        //SAFETY:
        FlowManager.Instance.SetState(FlowManager.GameState.ChoosingCard);

        cardPickGOs.Clear();
        for (int j = 0; j< cardPicks.Count;j++)
        {
            GameObject cardGO = Instantiate(cardPicks[j], cardPickSpawn.position, Quaternion.identity) as GameObject;
            cardGO.transform.position += (cardPickSpawn.right * -1) * ((cardPicks.Count-1) * cardPickSpace * 0.5f) + (cardPickSpawn.right * j * cardPickSpace);
            cardGO.GetComponent<Card>().SetCardSpritesOnLayer(999, 0);
            cardPickGOs.Add(cardGO);
        }

        List<Vector3> pickOldPos = new List<Vector3>();
        for(int k = 0; k<cardPickGOs.Count;k++)
        {
            pickOldPos.Add(cardPickGOs[k].transform.position);
        }

        Vector3 pickCardTextOldPos = pickCardText.position;
        float counter = 0;
        while (counter < 1)
        {
            counter += Time.deltaTime * cardPickAnimSpeed;
            float t = Mathf.Sin(counter * Mathf.PI * 0.5f);
            for(int l = 0; l<cardPickGOs.Count;l++)
            {
                cardPickGOs[l].transform.position = Vector3.Lerp(pickOldPos[l], pickOldPos[l] + new Vector3(0, -pickYpos, 0), t);
            }


            pickCardText.position = Vector3.Lerp(pickCardTextOldPos, pickCardTextOldPos + new Vector3(0, -pickYpos, 0), t);


            yield return new WaitForEndOfFrame();

        }
    }

    public IEnumerator ChooseCard(GameObject go)
    {
        if (cardPick)
        {
            cardPick = false;

            int i = cardPickGOs.IndexOf(go.gameObject);

            List<Vector3> pickOldPos = new List<Vector3>();
            for (int k = 0; k < cardPickGOs.Count; k++)
            {
                pickOldPos.Add(cardPickGOs[k].transform.position);
            }

            Vector3 pickCardTextOldPos = pickCardText.position;
            float counter = 0;
            while (counter < 1)
            {
                counter += Time.deltaTime * cardPickAnimSpeed;
                for (int l = 0; l < cardPickGOs.Count; l++)
                {
                    if (l != i)
                    {
                        cardPickGOs[l].transform.position = Vector3.Lerp(pickOldPos[l], pickOldPos[l] + new Vector3(0, -pickYhide, 0), counter);
                    }
                }

                pickCardText.position = Vector3.Lerp(pickCardTextOldPos, pickCardTextOldPos + new Vector3(0, -pickYhide, 0), counter);


                yield return new WaitForEndOfFrame();
            }

            Vector3 oldPos = cardPickGOs[i].transform.position;
            float counter2 = 0;
            while (counter2 < 1)
            {
                counter2 += Time.deltaTime * cardPickAnimSpeed;
                cardPickGOs[i].transform.position = Vector3.Lerp(oldPos, oldPos + new Vector3(pickYhide, 0, 0), counter2);

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(0.25f);
            StartCoroutine(FlowManager.Instance.OverlayOut());
            AddCardToDeck(cardPickGOs[i]);
            yield return new WaitForSeconds(1);
            StartCoroutine(ShowHand());
            FlowManager.Instance.SetState(FlowManager.GameState.Idle);

            if (FlowManager.Instance.tutoStep == 5)
            {
                FlowManager.Instance.tutoStep = -1;
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(DrawCard());
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(DrawCard());
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(DrawCard());
            }

            for (int j = 0; j < cardPickGOs.Count; j++)
            {
                if (j != i)
                {
                    Destroy(cardPickGOs[j]);
                }
            }
        }
        
        

    }

    public void AddCardToDeck(GameObject card)
    {
        cardsInDeck.Add(card);
        card.transform.position = cardHandSpawn.position;
        totalCards++;
        //deckAnim.SetTrigger("Add");
        UpdateCardNumber();

    }

    public IEnumerator HideHand()
    {
        if (handUp)
        {
            handUp = false;
            Vector3 handOldPos = handParent.position;
            float counter = 0;
            while (counter < 1)
            {
                counter += Time.deltaTime * FlowManager.Instance.overlaySpeed;
                float t = Mathf.Sin(counter * Mathf.PI * 0.5f);
                handParent.position = Vector3.Lerp(handOldPos, handOldPos + new Vector3(0, -handYhidePos, 0), t);
                yield return new WaitForEndOfFrame();
            }
        }

    }

    public IEnumerator ShowHand()
    {
        if (!handUp)
        {
            handUp = true;
            Vector3 handOldPos = handParent.position;
            float counter = 0;
            while (counter < 1)
            {
                counter += Time.deltaTime * FlowManager.Instance.overlaySpeed;
                float t = Mathf.Sin(counter * Mathf.PI * 0.5f);
                handParent.position = Vector3.Lerp(handOldPos, handOldPos + new Vector3(0, handYhidePos, 0), t);
                yield return new WaitForEndOfFrame();
            }
        }

    }

    public void DestroyOrb()
    {
        orbCount--;
        if (cardsInHand[cardsInHand.Count - 1] != null)
        {
            if (orbCount<maxOrbs)
                StartCoroutine(Discard(cardsInHand[cardsInHand.Count - 1].transform, false));
        }

    }
}



