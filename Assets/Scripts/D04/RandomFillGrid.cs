using System.Collections;
using UnityEngine;

public class RandomFillGrid : MonoBehaviour
{
    [SerializeField] private int gridSize = 64;
    [SerializeField] private int seed = 0;
    [SerializeField] private float percentageOfOnes = 0.4f;
    [SerializeField] private float speed = 1.0f;
    private int[,] intGrid;
    System.Random rand;
    Texture2D texture;
    GameObject noiseMap;


    void Start()
    {
        intGrid = new int[gridSize, gridSize];
        texture = new Texture2D(gridSize, gridSize);
        rand = new System.Random(seed);

        noiseMap = GameObject.CreatePrimitive(PrimitiveType.Plane);
        noiseMap.transform.localScale = new Vector3((float)gridSize / 10.0f, 1, (float)gridSize / 10.0f);


        PopulateGrid(intGrid);
        AddBoarders(intGrid);
        DisplayGrid(intGrid, texture);



        noiseMap.GetComponent<Renderer>().material.mainTexture = texture;

        //StartCoroutine("animator");
        CaveMaker(intGrid);
    }

    private void PopulateGrid(int[,] grid)
    {
        for(int x = 0; x < grid.GetLength(0);  x++)
            for(int y = 0; y < grid.GetLength(1); y++)
            {
                double randFloat = rand.NextDouble();
                if (randFloat < percentageOfOnes)
                {
                    grid[x, y] = 1;
                }
                else
                {
                    grid[x, y] = 0;
                }
            }

    }

    private void AddBoarders(int[,] grid)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (x == 0 || x == grid.GetLength(0) - 1 || y == 0 || y == grid.GetLength(1) - 1)
                {
                    grid[x, y] = 1;
                }
            }
    }

    private void DisplayGrid(int[,] grid, Texture2D tex)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x,y] == 1)
                {
                    tex.SetPixel(x, y, Color.black); //wall
                }
                else
                {
                    tex.SetPixel(x, y, Color.white); //empty
                }
            }

        tex.Apply();
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
    }

    private int GetNeighbours(int gridX, int gridY, int[,] grid)
    {
        int totalNeighbours = 0;

        for (int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if(MapInRange(gridX + x, gridY + y))
                {
                    if (grid[gridX + x, gridY + y] == 1)
                    {
                        totalNeighbours++;
                    }
                }
            }
        }


        if (grid[gridX, gridY] == 1)
        {
            totalNeighbours--;
        }
            return totalNeighbours;
    }

    bool MapInRange(int x, int y)=> x>=0 && x < gridSize && y>=0 && y < gridSize;     //bad function due to limited scope but still usable

    private void Wolframs(int[,] grid)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for(int y = 0; y < gridSize; y++)
            {
                int neighbours = GetNeighbours(x, y, grid);

                if (grid[x, y] == 1)
                {
                    if(neighbours < 2 )
                    {
                        grid[x, y] = 0; //dies
                    }
                    else if(neighbours >= 4)
                    {
                        grid[x, y] = 0; //dies
                    }
                    else if(neighbours == 2 || neighbours == 3)
                    {
                        grid[x, y] = 1;
                    }
                }
                else
                {
                    if(neighbours == 3)
                    {
                        grid[x, y] = 1;
                    }
                }


            }
        }
        DisplayGrid(intGrid, texture);
    }

    private void CaveMaker(int[,] grid)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                int neighbours = GetNeighbours(x, y, grid);

                if(neighbours > 4)
                {
                    grid[x, y] = 1;
                }
                else if(neighbours < 4)
                {
                    grid[x, y] = 0;
                }

            }
        }
        DisplayGrid(intGrid, texture);
    }






    IEnumerator animator()
    {
        while (true)
        {
            //Wolframs(intGrid);
            CaveMaker(intGrid);

            yield return new WaitForSeconds(speed);
        }
    }




}
