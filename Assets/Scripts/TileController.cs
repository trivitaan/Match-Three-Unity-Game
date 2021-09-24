using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int id;
    private BoardManager board;
    private SpriteRenderer render;

    private static readonly Color selectedColor = new Color(0.5f, 0.5f, 0.5f);

    private static readonly Color normalColor = Color.white;
    private static readonly float moveDuration = 0.5f;

    private static TileController previousSelected = null;

    private bool isSelected = false;

    private void Awake()
    {
        board = BoardManager.Instance;
        render = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        //Non Selectable conditions
        if(render.sprite == null)
        {
            return;
        }

        //Already selected this tile?
        if(isSelected)
        {
            Deselect();
        }
        else
        {
            //if nothing is selected yet
            if(previousSelected == null)
            {
                Select();
            }
            else
            {
                previousSelected.Deselect();
                Select();
            }
        }
    }

    public IEnumerator MoveTilePosition(Vector2 targetPosition, System.ActiononCompleted)
    {
        Vector2 startPosition = transform.position;
        float time = 0.0f;

        //run animation on next frame for safety reasons
        yield return new WaitForEndOfFrame();
    }

    #region Select and Deselect
    private void Select()
    {
        isSelected = true;
        render.color = selectedColor;
        previousSelected = this;
    }

    private void Deselect()
    {
        isSelected = false;
        render.color = normalColor;
        previousSelected = null;
    }

    #endregion
    
    public void ChangeId(int id, int x, int y)
    {
        render.sprite = board.tileTypes[id];
        this.id = id;

        name = "TILE_"+ id +" ("+ x + ", " + y +")";
    }
}
