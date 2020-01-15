using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previewsColumn;
    public int previewsRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private Board board;

    private GameObject otherDot;

    private Vector2 temPosition;
    private Vector2 firstTouchPosion;
    private Vector2 finalTouchPosition;

    public float swipeAngle = 0;
    public float swipeRessist = 1f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        previewsColumn = column;
        previewsRow = row;
    }

    // Update is called once per frame
    void Update()
    {
        // Find The matched Dot
        FindingMatched();

        // If Dot tag matched here we change the color
        if (isMatched)
        {
            SpriteRenderer mySprit = GetComponent<SpriteRenderer>();
            mySprit.color = new Color(0f, 0f, 0f, .2f);
        }

        targetX = column;
        targetY = row;

        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move Towards the target
            temPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, temPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        } else
        {
            // Directly set the position
            temPosition = new Vector2(targetX, transform.position.y);
            transform.position = temPosition;
            
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move Towards the target
            temPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, temPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            // Directly set the position
            temPosition = new Vector2(transform.position.x, targetY);
            transform.position = temPosition;
            
        }
    }

    // Use IEnumerator for pause the method to check the matched item
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            // if dot do not matched return to the previews position
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previewsRow;
                column = previewsColumn;
            }
            else
            {
                // Destry the matched 
                board.DestryMatches();

            }
            otherDot = null;
        }
    }

    // Get Swipe start Value
    private void OnMouseDown()
    {
        firstTouchPosion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Get Swipe End Value
    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    // Calculate the Angle
    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.x - firstTouchPosion.x) > swipeRessist || Mathf.Abs(finalTouchPosition.y - firstTouchPosion.y) > swipeRessist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosion.y, finalTouchPosition.x - firstTouchPosion.x) * 180 / Mathf.PI;
            Debug.Log(swipeAngle);
            MovePieces();
        }
    }

   // Move The pieces 
    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            //Right Swipe finded. let's move the pieces
            otherDot = board.allDots[column + 1, row];
            previewsRow = row;
            previewsColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;

        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Up Swipe finded. let's move the pieces
            otherDot = board.allDots[column, row + 1];
            previewsRow = row;
            previewsColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;

        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe finded. let's move the pieces
            otherDot = board.allDots[column - 1, row];
            previewsRow = row;
            previewsColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe finded. let's move the pieces
            otherDot = board.allDots[column, row - 1];
            previewsRow = row;
            previewsColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }

        // Start Coroutine to pause the method for .5f
        StartCoroutine(CheckMoveCo());
    }

   
    // Find the matched which call on Update method
    void FindingMatched()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];

            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];

            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

}
