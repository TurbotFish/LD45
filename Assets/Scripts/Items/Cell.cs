using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    public int x;
    public int y;

    public void OnMouseEnter()
    {
        spriteRenderer.color = BoardManager.Instance.cellHoverColor;
        spriteRenderer.sortingLayerName = "AboveFog";
        spriteRenderer.sortingOrder = 1000;

    }
    public void OnMouseExit()
    {
        spriteRenderer.color = BoardManager.Instance.cellDefaultColor;
        spriteRenderer.sortingLayerName = "AboveFog";
        spriteRenderer.sortingOrder = 100;
    }
}
