using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    Animator anim;
    
    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnMouseEnter()
    {
        if (CardManager.Instance.canSelectCard)
        {
            HoverCard();
        }
    }
    public void OnMouseExit()
    {
        if (CardManager.Instance.canSelectCard)
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
    }


    public void UnselectCard()
    {
        anim.SetBool("hover", false);
        anim.SetBool("selected", false);
    }
}
