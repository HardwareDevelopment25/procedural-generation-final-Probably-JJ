using NUnit.Framework.Constraints;
using UnityEngine;

public class FIXMinerPathGen : MonoBehaviour
{
    [SerializeField] private Vector3 miner;
    [SerializeField] private Vector3 pathPlacer;
    [SerializeField] private GameObject floor;
    [SerializeField] private int moves = 10;
    [SerializeField]private int size = 32;

    private bool[,] grid;

    private int randomSelection = 0;
    private bool hasMoved = false;
    private Vector3 minerPos;

    [SerializeField] private int seed;
    System.Random rand;

    private void Awake()
    {
        rand = new System.Random(seed);
        grid = new bool[size, size];
    }

    private void Start()
    {
        minerPos = miner;
        Generate();
    }

    private void Update()
    {
       
    }

    private void Generate()
    {
        for (int i = 0; i < moves; i++)
        {
            randomSelection = rand.Next(0, 4);

            if (randomSelection == 0) // move right
            {
                minerPos.x++;
                hasMoved = true;
            }
            else if (randomSelection == 1) // move left
            {
                minerPos.x--;
                hasMoved = true;
            }
            else if (randomSelection == 2) // move up
            {
                minerPos.z += 1;
                hasMoved = true;
            }
            else if (randomSelection == 3) // move down
            {
                minerPos.z -= 1;
                hasMoved = true;
            }

            // is minerpos.x < 0 or if > grid. width , dont do it and same for y

            grid[(int)minerPos.x, (int)minerPos.z] = true;

            miner = minerPos;


        }

        for(int x  = 0; x < size; x++)
            for(int y  = 0; y < size; y++)
            {
                if (grid[x, y])
                {
                    pathPlacer = new Vector3(miner.x, 0, miner.z);
                    GameObject.Instantiate(floor, new Vector3(x , 0 , y), Quaternion.identity);
                }
                else
                {
                    pathPlacer = new Vector3(miner.x, 0, miner.z);
                    GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = new Vector3(x, 0, y);


                }

           
            }

    }

}
