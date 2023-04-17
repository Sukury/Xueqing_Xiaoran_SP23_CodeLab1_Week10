using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleConnectionGame : MonoBehaviour
{
    public int gridSize = 15; // 棋盘格子数量
    public float gridSpacing = 0.5f; // 棋盘格子间距
    
    private int[,] grid; // 0 be an empty space, 1 be blue, and 2 be red

    public TextMeshProUGUI display;
    
    private bool whiteTurn = false;

    private List<GameObject> spawnedPieces = new List<GameObject>();

    public GameObject whitePrefab, blackPrefab, gridPrefab;
    
    private GameObject[,] chessPieces; // 格子

    private int xGrid, yGrid;
    
    private void Start()
    {
        CreateChessBoard();
        
        grid = new int [gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = 0;
            }
        }
    }
    
    // 创建棋盘
    void CreateChessBoard()
    {
        // 创建棋盘数组
        chessPieces = new GameObject[gridSize, gridSize];

        // 创建棋盘格子
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject grid = Instantiate(gridPrefab);
                grid.transform.position = new Vector3(
                        gridSpacing * x - gridSize * gridSpacing/2, 
                        gridSpacing * y - gridSize * gridSpacing/2,
                        gridSpacing);
                grid.transform.localScale = new Vector3(gridSpacing - 0.05f, gridSpacing - 0.05f);
                chessPieces[x, y] = grid;
            }
        }
    }

    private void Update()
    {
        // If you press space, it reloads the scene.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        if (BlackWin() || WhiteWin()) return;
        
        // 如果鼠标左键被按下
        if (Input.GetMouseButtonDown(0))
        {
            float clickOffset1 = gridSpacing / 4;
            float clickOffset2 = 3 * gridSpacing / 4;
            Vector3 mousePos = Input.mousePosition; // 获取鼠标在屏幕上的位置
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            float xPos = worldPos.x + gridSpacing * gridSize / 2;
            float yPos = worldPos.y + gridSpacing * gridSize / 2;

            if (xPos < gridSpacing * gridSize &&
                xPos > 0 &&
                yPos < gridSpacing * gridSize &&
                xPos > 0)
            {
                xGrid = Mathf.RoundToInt(xPos / gridSpacing + 0.2f);
                Debug.Log("xGrid:" + xGrid);
                yGrid = Mathf.RoundToInt(yPos / gridSpacing + 0.2f);
                Debug.Log("yGrid:" + yGrid);
            }
                
            if (IsEmpty(xGrid, yGrid))
            {
                if (whiteTurn)
                    grid[xGrid, yGrid] = 2;
                else
                    grid[xGrid, yGrid] = 1;
                
                whiteTurn = !whiteTurn;
            }
            UpdateDisplay();
        }
    }

    // If a space is 1, it's "blue"
    public bool ContainsWhite(int x, int y)
    {
        return grid[x, y] == 1;
    }

    // If a space is 2, it's "red"
    public bool ContainsBlack(int x, int y)
    {
        return grid[x, y] == 2;
    }

    // If a space is 0, it's empty.
    public bool IsEmpty(int x, int y)
    {
        return grid[x, y] == 0;
    }
    
    private void UpdateDisplay()
    {
        // To update the display, first destroy all the pieces that were spawned.
        foreach (var piece in spawnedPieces)
        {
            Destroy(piece);
        }
        
        spawnedPieces.Clear();

        // Then, for everything in the grid, if it's not 0, spawn the correct piece.
        for (var x = 0; x < gridSize; x++)
        {
            for (var y = 0; y < gridSize; y++)
            {
                if (ContainsWhite(x, y))
                {
                    var whitePiece = Instantiate(whitePrefab);
                    whitePiece.transform.position = 
                        new Vector3(
                            gridSpacing * x - gridSize * gridSpacing/2 - gridSpacing/2, 
                            gridSpacing*y - gridSize*gridSpacing/2 - gridSpacing/2
                            );
                    whitePiece.transform.localScale = 
                        new Vector3(gridSpacing - 0.1f, gridSpacing - 0.1f);
                    spawnedPieces.Add(whitePiece);
                }
                if (ContainsBlack(x, y))
                {
                    var blackPiece = Instantiate(blackPrefab);
                    blackPiece.transform.position = 
                        new Vector3(
                        gridSpacing * x - gridSize * gridSpacing/2 - gridSpacing/2, 
                        gridSpacing*y - gridSize*gridSpacing/2 - gridSpacing/2
                    );
                    blackPiece.transform.localScale = 
                        new Vector3(gridSpacing - 0.1f, gridSpacing - 0.1f);
                    spawnedPieces.Add(blackPiece);
                }
            }
        }
        
        // check to see if red or blue won.
        if (WhiteWin())
        {
            display.text = "WHITE WINS!";
            display.color = Color.white;
        }
        else if (BlackWin())
        {
            display.text = "BLACK WINS!";
            display.color = Color.black;
        }
        else
        {
            display.text = "";
        }
    }
    
    public bool WhiteWin()
    {
        return FourInARow() == 1;
    }

    public bool BlackWin()
    {
        return FourInARow() == 2;
    }

    // This function checks for four in a row, and returns the number that is four in a row.
    // 1 for blue
    // 2 for red
    public int FourInARow()
    {
        
        for (var x = 0; x < gridSize; x++)
        {
            for (var y = 0; y < gridSize; y++)
            {
                if (y <= gridSize - 5)
                    if (grid[x,y] != 0 &&
                        grid[x, y] == grid[x, y + 1] &&
                        grid[x, y] == grid[x, y + 2] &&
                        grid[x, y] == grid[x, y + 3]&&
                        grid[x, y] == grid[x, y + 4])
                        return grid[x, y];
             
                if (x <= gridSize - 5)               
                    if (grid[x,y] != 0 && 
                        grid[x, y] == grid[x + 1, y] && 
                        grid[x, y] == grid[x + 2, y] && 
                        grid[x, y] == grid[x + 3, y] &&
                        grid[x, y] == grid[x + 4, y])
                        return grid[x, y];

                if (x <= gridSize - 5 && y <= gridSize - 5)
                    if (grid[x,y] != 0 &&
                        grid[x, y] == grid[x + 1, y + 1] && 
                        grid[x, y] == grid[x + 2, y + 2] && 
                        grid[x, y] == grid[x + 3, y + 3] &&
                        grid[x, y] == grid[x + 4, y + 4])
                        return grid[x, y];

                if (x >= 5 && y <= gridSize - 5)
                    if (grid[x,y] != 0 && 
                        grid[x, y] == grid[x - 1, y + 1] && 
                        grid[x, y] == grid[x - 2, y + 2] && 
                        grid[x, y] == grid[x - 3, y + 3] &&
                        grid[x, y] == grid[x - 4, y + 4])
                        return grid[x, y];
            }
        }

        return 0;
    }
    
}
