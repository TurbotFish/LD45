using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    Animator anim;
    public CardManager.CardType cardType;
    public GameObject itemGO;
    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    public List<TextMeshPro> texts = new List<TextMeshPro>();
    public bool discarded;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnMouseEnter()
    {
        if (CardManager.Instance.canSelectCard && !discarded)
        {
            HoverCard();
        }
    }
    public void OnMouseExit()
    {
        if (CardManager.Instance.canSelectCard && !discarded)
        {
            UnhoverCard();
        }
    }


    public void HoverCard()
    {
        if (!anim.GetBool("selected"))
        {
            anim.SetBool("hover", true);
        }
    }

    public void UnhoverCard()
    {
        if (!anim.GetBool("selected"))
        {
            anim.SetBool("hover", false);
        }
    }

    public void SelectCard()
    {
        anim.SetBool("hover", false);
        anim.SetBool("selected", true);
        CardManager.Instance.selectedCard = this;
        FlowManager.Instance.SetState(FlowManager.GameState.Casting);

        if (cardType == CardManager.CardType.Heart)
        {
            BoardManager.Instance.HighlightFreeCells();
        }

        else if (cardType == CardManager.CardType.CorruptedHeart
            || cardType == CardManager.CardType.Shield
            || cardType == CardManager.CardType.Sword
            || cardType == CardManager.CardType.Orb
            || cardType == CardManager.CardType.TinyHeart
            || cardType == CardManager.CardType.Arrow
            || cardType == CardManager.CardType.Heal)
        {
            BoardManager.Instance.HighlightAdjacents();
        }

        else if (cardType == CardManager.CardType.Consume
            || cardType == CardManager.CardType.Bubble)
        {
            BoardManager.Instance.HighlightPlayerItems();
        }

    }


    public void UnselectCard()
    {
        anim.SetBool("hover", false);
        anim.SetBool("selected", false);
        BoardManager.Instance.HideCells();
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);

    }

    public void SetCardSpritesOnLayer(int layerID, int minOrder)
    {
        for(int i = 0; i < sprites.Count; i ++)
        {
            sprites[i].sortingLayerID = layerID;
            sprites[i].sortingOrder = minOrder + i;
        }

        foreach(TextMeshPro text in texts)
        {
            text.sortingLayerID = layerID;
            text.sortingOrder = minOrder + sprites.Count;
        }
    }

    public void ResolveCard(int x, int y)
    {
        BoardManager.Instance.HideCells();

        switch (cardType)
        {
            case CardManager.CardType.Heart:
                StartCoroutine(ResolveHeart(x,y));
                break;
            case CardManager.CardType.CorruptedHeart:
                StartCoroutine(ResolveCorruptedHeart(x, y));
                break;
            case CardManager.CardType.TinyHeart:
                StartCoroutine(ResolveTinyHeart(x, y));
                break;
            case CardManager.CardType.Sword:
                StartCoroutine(ResolveSword(x, y));
                break;
            case CardManager.CardType.Shield:
                StartCoroutine(ResolveShield(x, y));
                break;
            case CardManager.CardType.Heal:
                StartCoroutine(ResolveHeal(x, y));
                break;
            case CardManager.CardType.Orb:
                StartCoroutine(ResolveOrb(x, y));
                break;
            case CardManager.CardType.Arrow:
                StartCoroutine(ResolveArrows(x,y));
                break;

        }
    }


    public IEnumerator ResolveHeart(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        BoardManager.Instance.hearts.Add(BoardManager.Instance.items[x, y].GetComponent<Heart>());
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.DiscardHeart(CardManager.Instance.selectedCard.transform));
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        yield return new WaitForSeconds(1f);
        StartCoroutine(CardManager.Instance.DrawCard());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CardManager.Instance.DrawCard());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CardManager.Instance.DrawCard());
    }

    public IEnumerator ResolveSword(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        StartCoroutine(BoardManager.Instance.SwordAttack(x,y));
        yield return new WaitForSeconds(0.5f);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        StartCoroutine(CardManager.Instance.DrawCard());

    }

    public IEnumerator ResolveShield(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.DrawCard());

    }


    public IEnumerator ResolveTinyHeart(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        BoardManager.Instance.hearts.Add(BoardManager.Instance.items[x, y].GetComponent<Heart>());
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.DrawCard());

    }

    public IEnumerator ResolveCorruptedHeart(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        BoardManager.Instance.hearts.Add(BoardManager.Instance.items[x, y].GetComponent<Heart>());
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);

        CardManager.Instance.corruptedHeartCount++;
        if (CardManager.Instance.handSize < 1)
        {
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(CardManager.Instance.DrawCard());
        }
    }




    public IEnumerator ResolveHeal(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        StartCoroutine(BoardManager.Instance.Heal(x, y));
        yield return new WaitForSeconds(0.5f);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        StartCoroutine(CardManager.Instance.DrawCard());

    }

    public IEnumerator ResolveOrb(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.DrawCard());

        CardManager.Instance.orbCount++;
        if (CardManager.Instance.orbCount <= CardManager.Instance.maxOrbs)
        {
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(CardManager.Instance.DrawCard());
        }
    }

    public IEnumerator ResolveConsume(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateFX(x, y, CardManager.Instance.consumeFX, 1);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        BoardManager.Instance.items[x, y].Consume();
        yield return new WaitForSeconds(0.5f);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        StartCoroutine(CardManager.Instance.DrawCard());

    }


    public IEnumerator ResolveArrows(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        StartCoroutine(BoardManager.Instance.ArrowAttack(x,y));
        yield return new WaitForSeconds(1f);
        StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        yield return new WaitForSeconds(0.5f);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        StartCoroutine(CardManager.Instance.DrawCard());

    }
}
