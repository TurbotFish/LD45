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
            if (!anim.GetBool("hover"))
            {
                HoverCard();

            }
        }
    }
    public void OnMouseExit()
    {
        if (CardManager.Instance.canSelectCard && !discarded)
        {
            if (!anim.GetBool("selected"))
            {
                UnhoverCard();

            }
        }
    }


    public void HoverCard()
    {
        if (!anim.GetBool("selected"))
        {
            if (!anim.GetBool("hover"))
            {
                anim.SetBool("hover", true);
                SoundManager.Instance.PlaySound(1, SoundManager.Instance.hover, 0.8f);
            }
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
        if (!anim.GetBool("selected"))
        {
            SoundManager.Instance.PlaySound(1, SoundManager.Instance.select);
            anim.SetBool("selected", true);
            CardManager.Instance.selectedCard = this;

            if (FlowManager.Instance.tuto)
            {
                if (FlowManager.Instance.tutoStep == 1 && cardType == CardManager.CardType.Heart)
                {
                    StartCoroutine(FlowManager.Instance.TutoStepTwo());
                }
                else if (FlowManager.Instance.tutoStep == 3 && cardType == CardManager.CardType.Sword)
                {
                    StartCoroutine(FlowManager.Instance.TutoStepThreeHalf());
                }
            }
            else if (FlowManager.Instance.state == FlowManager.GameState.Idle)
            {
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
            else if (FlowManager.Instance.state == FlowManager.GameState.ChoosingCard)
            {
                StartCoroutine(CardManager.Instance.ChooseCard(this.gameObject));
            }
        }           
        
    }


    public void UnselectCard()
    {
        if (anim.GetBool("selected"))
        {
            if (FlowManager.Instance.tuto)
            {
                if (FlowManager.Instance.tutoStep == 2)
                {
                    StartCoroutine(FlowManager.Instance.TutoStepOne());
                }
                else if (FlowManager.Instance.tutoStep == 35)
                {
                    StartCoroutine(FlowManager.Instance.TutoStepThree(false));
                }
            }
            anim.SetBool("hover", false);
            anim.SetBool("selected", false);
            BoardManager.Instance.HideCells();
            FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        }


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
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.HideCells();
        if (CardManager.Instance.selectedCard != null)
        {
            CardManager.Instance.cardsInHand.Remove(CardManager.Instance.selectedCard.gameObject);
        }

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
            case CardManager.CardType.Consume:
                StartCoroutine(ResolveConsume(x, y));
                break;

        }
    }


    public IEnumerator ResolveHeart(int x, int y)
    {
        Debug.Log("mmmh est ce que c'est lancé au moins???");
        if (FlowManager.Instance.tuto)
        {
            Debug.Log("tuto step 3 heart");
            FlowManager.Instance.tutoStep = 3;
        }
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        SoundManager.Instance.PlaySound(1, SoundManager.Instance.spawn);
        BoardManager.Instance.hearts.Add(BoardManager.Instance.items[x, y].GetComponent<Heart>());
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CardManager.Instance.DiscardHeart(CardManager.Instance.selectedCard.transform));
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        if (!FlowManager.Instance.tuto)
        {
            yield return new WaitForSeconds(1f);
            //StartCoroutine(CardManager.Instance.DrawCard());
            CardManager.Instance.Draw();

            yield return new WaitForSeconds(0.5f);
            //StartCoroutine(CardManager.Instance.DrawCard());
            CardManager.Instance.Draw();

            yield return new WaitForSeconds(0.5f);
            //StartCoroutine(CardManager.Instance.DrawCard());
            CardManager.Instance.Draw();
            Debug.Log("draw draw draw");
        }
        else
        {
            Debug.Log("oh oh");
            StartCoroutine(FlowManager.Instance.TutoStepThree(true));
        }

    }

    public IEnumerator ResolveSword(int x, int y)
    {
        if (FlowManager.Instance.tuto)
        {
            FlowManager.Instance.tutoStep = 4;
        }
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        SoundManager.Instance.PlaySound(1, SoundManager.Instance.spawn);
        yield return new WaitForSeconds(0.25f);
        //StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        CardManager.Instance.DiscardBis(CardManager.Instance.selectedCard.transform, true);
        StartCoroutine(BoardManager.Instance.SwordAttack(x,y));
        SoundManager.Instance.PlaySound(2, SoundManager.Instance.sfx_sword);
        yield return new WaitForSeconds(0.5f);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);

        if(FlowManager.Instance.tuto)
        {
            StartCoroutine(FlowManager.Instance.TutoStepFour());
        }
        else
        {
            //StartCoroutine(CardManager.Instance.DrawCard());
            CardManager.Instance.Draw();

        }

    }

    public IEnumerator ResolveShield(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        SoundManager.Instance.PlaySound(1, SoundManager.Instance.spawn);
        BoardManager.Instance.ComputeConnections();
        yield return new WaitForSeconds(0.25f);
        //StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        CardManager.Instance.DiscardBis(CardManager.Instance.selectedCard.transform, true);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        yield return new WaitForSeconds(0.25f);
        //StartCoroutine(CardManager.Instance.DrawCard());
        CardManager.Instance.Draw();


    }


    public IEnumerator ResolveTinyHeart(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        SoundManager.Instance.PlaySound(1, SoundManager.Instance.spawn);
        BoardManager.Instance.hearts.Add(BoardManager.Instance.items[x, y].GetComponent<Heart>());
        BoardManager.Instance.ComputeConnections();
        yield return new WaitForSeconds(0.25f);
        //StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        CardManager.Instance.DiscardBis(CardManager.Instance.selectedCard.transform, true);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        yield return new WaitForSeconds(0.25f);
        //StartCoroutine(CardManager.Instance.DrawCard());
        CardManager.Instance.Draw();


    }

    public IEnumerator ResolveCorruptedHeart(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        SoundManager.Instance.PlaySound(1, SoundManager.Instance.spawn);
        BoardManager.Instance.hearts.Add(BoardManager.Instance.items[x, y].GetComponent<Heart>());
        BoardManager.Instance.ComputeConnections();
        yield return new WaitForSeconds(0.25f);
       
        //StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        CardManager.Instance.DiscardBis(CardManager.Instance.selectedCard.transform, true);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);

        CardManager.Instance.corruptedHeartCount++;
        if (CardManager.Instance.cardsInHand.Count < 1 || CardManager.Instance.corruptedHeartCount<CardManager.Instance.orbCount)
        {
            yield return new WaitForSeconds(0.25f);
            //StartCoroutine(CardManager.Instance.DrawCard());
            CardManager.Instance.Draw();
        }
    }




    public IEnumerator ResolveHeal(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        SoundManager.Instance.PlaySound(1, SoundManager.Instance.spawn);
        BoardManager.Instance.ComputeConnections();
        yield return new WaitForSeconds(0.25f);
        //StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        CardManager.Instance.DiscardBis(CardManager.Instance.selectedCard.transform, true);

        StartCoroutine(BoardManager.Instance.Heal(x, y));
        SoundManager.Instance.PlaySound(2, SoundManager.Instance.sfx_heal);
        yield return new WaitForSeconds(0.5f);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        //StartCoroutine(CardManager.Instance.DrawCard());
        CardManager.Instance.Draw();

    }

    public IEnumerator ResolveOrb(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateItem(x, y, CardManager.Instance.selectedCard.itemGO);
        SoundManager.Instance.PlaySound(1, SoundManager.Instance.spawn);
        BoardManager.Instance.ComputeConnections();
        yield return new WaitForSeconds(0.25f);
        //StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
        CardManager.Instance.DiscardBis(CardManager.Instance.selectedCard.transform, true);

        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        yield return new WaitForSeconds(0.25f);
        //StartCoroutine(CardManager.Instance.DrawCard());
        CardManager.Instance.Draw();

        CardManager.Instance.orbCount++;
        if (CardManager.Instance.orbCount <= CardManager.Instance.maxOrbs && CardManager.Instance.orbCount>=CardManager.Instance.corruptedHeartCount)
        {
            yield return new WaitForSeconds(0.75f);
            //StartCoroutine(CardManager.Instance.DrawCard());
            CardManager.Instance.Draw();

        }
    }

    public IEnumerator ResolveConsume(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        BoardManager.Instance.InstantiateFX(x, y, CardManager.Instance.consumeFX, 1);
        SoundManager.Instance.PlaySound(2, SoundManager.Instance.sfx_consume);
        BoardManager.Instance.items[x, y].Consume();
        BoardManager.Instance.ComputeConnections();
        yield return new WaitForSeconds(0.25f);
        //if (CardManager.Instance.selectedCard!=null)
        //{
            //StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
            CardManager.Instance.DiscardBis(CardManager.Instance.selectedCard.transform, true);

        //}
        yield return new WaitForSeconds(0.5f);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);
        //StartCoroutine(CardManager.Instance.DrawCard());
        CardManager.Instance.Draw();

    }


    public IEnumerator ResolveArrows(int x, int y)
    {
        FlowManager.Instance.SetState(FlowManager.GameState.Resolving);
        StartCoroutine(BoardManager.Instance.ArrowAttack(x,y));
        yield return new WaitForSeconds(0.25f);
        //if (CardManager.Instance.selectedCard != null)
        //{
            //StartCoroutine(CardManager.Instance.Discard(CardManager.Instance.selectedCard.transform, true));
            CardManager.Instance.DiscardBis(CardManager.Instance.selectedCard.transform, true);


        //}
        yield return new WaitForSeconds(0.5f);
        FlowManager.Instance.SetState(FlowManager.GameState.Idle);

        //StartCoroutine(CardManager.Instance.DrawCard());
        CardManager.Instance.Draw();

    }
}
