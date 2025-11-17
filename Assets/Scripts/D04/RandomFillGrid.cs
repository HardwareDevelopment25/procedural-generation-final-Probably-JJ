using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class RandomFillGrid : MonoBehaviour
{
    [SerializeField] private int gridSize = 64;
    [SerializeField] private int seed = 0;
    [SerializeField] private float percentageOfOnes = 0.4f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private int smoothIts = 1;
    [SerializeField] private GameObject stone, grass, spikes, cellPrefab;
    public Sprite[] marchinSprites;

    private int[,] intGrid;
    private GameObject caveParent;
    private GameObject marchinParent;
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
        CaveMaker(intGrid, texture);
        AddCaveGrassAndSpikes(intGrid, texture);

        GenerateMarchinSquares(intGrid);
        MakeCave3D(intGrid, stone, grass, spikes);
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
                if (grid[x, y] == 0)
                {
                    tex.SetPixel(x, y, Color.lightBlue); //Air
                }
                else if (grid[x,y] == 1)
                {
                    tex.SetPixel(x, y, Color.darkGray); //Stone
                }
                else if (grid[x, y] == 2)
                {
                    tex.SetPixel(x, y, Color.green); //Grass
                }
                else if (grid[x, y] == 3)
                {
                    tex.SetPixel(x, y, Color.grey); //Spikes
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

    bool MapInRange(int x, int y)=> x>=0 && x < gridSize && y>=0 && y < gridSize;     //bad function due to limited scope but still usable in some cases

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

    //making the noise map generate into terrain
    private void CaveMaker(int[,] grid, Texture2D tex)
    {
        for (int i = 0; i < smoothIts; i++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    int neighbours = GetNeighbours(x, y, grid);

                    if (neighbours > 4)
                    {
                        grid[x, y] = 1;
                    }
                    else if (neighbours < 4)
                    {
                        grid[x, y] = 0;
                    }

                }
            }
        }
        DisplayGrid(grid, tex);
    }

    //making terrain look more full by replacing top layers with grass and bottom layers with spikes
    private void AddCaveGrassAndSpikes(int[,] grid, Texture2D tex)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (y > 0 && y < gridSize - 1)
                {
                    if (grid[x, y] == 1 && grid[x, y - 1] == 0)
                    {
                        //checks if current for solid then checks space above for air if so makes grass 
                        grid[x, y] = 2; //2 represents grass
                    }

                    else if (grid[x, y] == 1 && grid[x, y + 1] == 0)
                    {
                        //checks if current for solid then checks space below for air if so makes spikes
                        grid[x, y] = 3; //3 represents spikes
                    }
                }
            }
        }
        DisplayGrid(grid, tex);
    }

    //making terrain generate on planes using prefabs so they can have textures
    private void MakeCave3D(int[,] grid, GameObject stone, GameObject grass, GameObject spikes)
    {
        caveParent = new GameObject();
        caveParent.name = "CaveParentObject";
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                switch (grid[x, y])
                {
                    case 0: //air
                        break;

                    case 1: //stone
                        GameObject.Instantiate(stone, new Vector3(x, 0, y), Quaternion.identity, caveParent.transform);
                        break;

                    case 2: //grass
                        GameObject.Instantiate(grass, new Vector3(x, 0, y), Quaternion.identity, caveParent.transform);
                        break;

                    case 3: //spikes
                        GameObject.Instantiate(spikes, new Vector3(x, 0, y), Quaternion.identity, caveParent.transform);
                        break;

                    default:
                        break;

                }
            }
        }
    }


    //generating terrain with diagonals using the same map
    //essentially all squares divided up into 4 and then checked to see which corners have connecting terrarain then filled in appropriately

    int GetConfigIndex(int[,] grid, int currentX, int currentY)
    {
        int configIndex = 0;

        if (grid[currentX, currentY] > 0) configIndex |= 1;
        if (grid[currentX+1, currentY] > 0) configIndex |= 2;
        if (grid[currentX+1, currentY+1] > 0) configIndex |= 4;
        if (grid[currentX, currentY+1] > 0) configIndex |= 8;

        return configIndex;
    }

    void GenerateMarchinSquares(int[,] grid)
    {
        marchinParent = new GameObject();
        marchinParent.name = "MarchinParentObject";

        //start at 1 and end before the end of the grid to prevent checking invalid space and returning error
        for (int x = 1; x < gridSize - 1; x++)
        {
            for (int y = 1; y < gridSize - 1; y++)
            {
                PlaceCell(x, y, GetConfigIndex(grid, x, y));
            }
        }
    }

    void PlaceCell(int x, int y, int configIndex)
    {
        Vector3 pos = new Vector3(x, 0, y);
        GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, marchinParent.transform);
        cell.transform.rotation = Quaternion.Euler(90, 0, 0);
        SpriteRenderer spriteRenderer = cell.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = marchinSprites[configIndex];
    }


    /*
    IEnumerator animator()
    {
        while (true)
        {
            //Wolframs(intGrid);
            CaveMaker(intGrid);

            yield return new WaitForSeconds(speed);
        }
    }
    */


}
