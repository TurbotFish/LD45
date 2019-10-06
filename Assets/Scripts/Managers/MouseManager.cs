using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !FlowManager.Instance.noInput)
        {

            if (FlowManager.Instance.tuto && FlowManager.Instance.tutoStep == 4)
            {
                StartCoroutine(FlowManager.Instance.QuitTuto());
            }

            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hitInfo.collider != null)
            {

                //+++++++++++++++++++++++++++++++++++++++++++++++CARD SELECTION
                
                if (hitInfo.collider.GetComponent<Card>())
                {
                    Card card = hitInfo.collider.GetComponent<Card>();

                    if (CardManager.Instance.canSelectCard && !card.discarded)
                    {
                        if (CardManager.Instance.selectedCard != null)
                        {
                            if (CardManager.Instance.selectedCard != card)
                            {
                                CardManager.Instance.selectedCard.UnselectCard();
                            }
                        }
                        card.SelectCard();
                    }
                }


                //+++++++++++++++++++++++++++++++++++++++++++++++CASTING

                else if (hitInfo.collider.GetComponent<Cell>())
                {
                    if (FlowManager.Instance.state == FlowManager.GameState.Casting)
                    {
                        Cell cell = hitInfo.collider.GetComponent<Cell>();
                        CardManager.Instance.selectedCard.ResolveCard(cell.x, cell.y);
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
