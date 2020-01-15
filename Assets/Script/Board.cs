using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    // height and width value will be come from Unity
    public int width;
    public int height;

    private BackgroundTile[,] allTiles;

    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];

        SetUp();
    }

    void Update()
    {
        // Get Action when press the Back Button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Scene 0 is the start page. 
            SceneManager.LoadScene(0); 
        }
    }

    // Setup the board
    private void SetUp()
    {
        // Double loop for create the board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 temPosition = new Vector2(i, j);
                GameObject backendTile = Instantiate(tilePrefab, temPosition, Quaternion.identity) as GameObject;
                backendTile.transform.parent = this.transform;
                backendTile.name = "( " + i + ", " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);
                int maxLoaded = 0;

                while (MatchAt(i, j, dots[dotToUse]) && maxLoaded < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxLoaded++;
                }
                maxLoaded = 0;

                GameObject dot = Instantiate(dots[dotToUse], temPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";

                allDots[i, j] = dot;
            }
        }
    }

    // Find The matched 
    private bool MatchAt(int column, int row, GameObject picese)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == picese.tag && allDots[column - 2, row].tag == picese.tag)
            {

                return true;
            }

            if (allDots[column, row - 1].tag == picese.tag && allDots[column, row - 2].tag == picese.tag)
            {

                return true;
            }
        }
        else if (row <= 1 || column <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == picese.tag && allDots[column, row - 2].tag == picese.tag)
                {

                    return true;
                }
            }

            if (column > 1)
            {
                if (allDots[column - 1, row].tag == picese.tag && allDots[column - 2, row].tag == picese.tag)
                {

                    return true;
                }
            }
        }

        return false;
    }

    // Destroy Method
    private void DestryMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    // Check the dot's for destroy
    public void DestryMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestryMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    // Decrease the Board
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                } else if (nullCount > 0) 
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    // Refill the board after destroy the dots
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 temPosition = new Vector2(i, j);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], temPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                }
            }
        }
    }

    // Find the matched Dot before create the board and when refill the board
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Fill up the board after destroy the matched dot
    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        // wait sometime...
        yield return new WaitForSeconds(.5f);

        // let's check the board it's matched or not 
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestryMatches();
        }
    }

}
