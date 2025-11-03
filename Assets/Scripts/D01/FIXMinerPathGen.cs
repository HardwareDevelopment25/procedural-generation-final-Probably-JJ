using NUnit.Framework.Constraints;
using System.Collections;
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

    private void Generate()
    {
        for (int i = 0; i < moves; i++) //convert into while so can rerun/recheck 
        {
            randomSelection = rand.Next(0, 4);

            if (randomSelection == 0) // move right
            {
                minerPos.x++;
            }
            else if (randomSelection == 1) // move left
            {
                minerPos.x--;
            }
            else if (randomSelection == 2) // move up
            {
                minerPos.z += 1;
            }
            else if (randomSelection == 3) // move down
            {
                minerPos.z -= 1;
            }

            //if(minerPos.x < 0 || minerPos.x > size || minerPos.z < 0 || minerPos.z > size)

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
