using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    System.Random m_Random;
    public int size = 10, seed = 0;
    public GameObject wall, floor;
    private bool[,] maze;

    private void Start()
    {
        m_Random = new System.Random(seed);
        maze = new bool[size,size]; //makes a grid of false bools

        GenerateMaze();
        DrawMaze();
    }

    private void GenerateMaze()
    {
        //stack to store unvisited locations
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        //start maze at top left (0,0)
        Vector2Int current = new Vector2Int(0, 0);

        //mark starting cell as part of maze
        maze[current.x, current.y] = true;

        //add start pos to stack
        stack.Push(current);

        while(stack.Count > 0)//while something in stack, run loop
        {
            current = stack.Pop();//takes top pos to be checked

            //checking neighbours
            List<Vector2Int> neighbours = new List<Vector2Int>(); //makes list of up to 4 neighbours

            if(current.x > 1 && !maze[current.x - 2, current.y]) //checking if left is available
               neighbours.Add(new Vector2Int(current.x - 2, current.y));
            
            if (current.x < size - 2 && !maze[current.x + 2, current.y]) //checking if right is available
                neighbours.Add(new Vector2Int(current.x + 2, current.y));

            if (current.y > 0 && !maze[current.x, current.y - 2 ]) //checking if up is available
                neighbours.Add(new Vector2Int(current.x, current.y - 2));

            if (current.y < size - 2 && !maze[current.x, current.y + 2]) //checking if down is available
                neighbours.Add(new Vector2Int(current.x, current.y + 2));

            //choose a neighbour then add to stack
            if(neighbours.Count > 0)//checks if anything added to list
            {
                stack.Push(current); //puts back what we popped off the stack

                //choose a neighbour
                Vector2Int chosenOne = neighbours[ m_Random.Next(0, neighbours.Count) ];
                
                if(chosenOne.x == current.x)
                {
                    maze[chosenOne.x, chosenOne.y + 1] = true; //marks the vertical point inbetween current and chosen as true
                }
                else
                {
                    maze[chosenOne.x + 1, chosenOne.y] = true; // marks the horizontal point inbetween current and chosen as true
                }

                maze[chosenOne.x, chosenOne.y] = true;

                stack.Push(chosenOne);
            }
        }
    }

    private void DrawMaze() //loops through all maze x and y checking for true and placing floor otherwise if false places wall
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (maze[x, y])
                {
                    GameObject.Instantiate(floor, new Vector3(x, 0, y), Quaternion.identity);
                }
                else
                {
                    GameObject.Instantiate(wall, new Vector3(x, 0, y), Quaternion.identity);
                }
            }
        }
    }
}
