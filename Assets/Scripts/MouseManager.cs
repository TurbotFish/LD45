using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hitInfo.collider != null)
            {

                if (hitInfo.collider.GetComponent<Card>())
                {
                    Card card = hitInfo.collider.GetComponent<Card>();

                    if (CardManager.Instance.canSelectCard)
                    {
                        if (CardManager.Instance.selectedCard != null)
                        {
                            CardManager.Instance.selectedCard.UnselectCard();
                        }
                        card.SelectCard();
                    }
                }
            }
            else
            {
                if (CardManager.Instance.selectedCard != null)
                {
                    CardManager.Instance.selectedCard.UnselectCard();
                }
            }



        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (CardManager.Instance.selectedCard != null)
            {
                CardManager.Instance.selectedCard.UnselectCard();
            }
        }

    }
}
