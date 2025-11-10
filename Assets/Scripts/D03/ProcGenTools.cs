using UnityEngine;

public static class ProcGenTools 
{
    /// <summary>
    /// loops through all maze x and y checking for true and setting black otherwise if false places white
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="maze"></param>
    /// <returns></returns>
    public static Texture2D DrawMaze2D(Texture2D texture, bool[,] maze)
    {
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                if (maze[x, y])
                {
                    texture.SetPixel(x, y, Color.black);
                }
                else
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }



    public static Mesh MakeTriangle(float triangleSize)
    {
        Mesh triangle = new Mesh();

        Vector3[] verticies = new Vector3[]
        {
            new Vector3 (0, 0, 0),
            new Vector3 (triangleSize, 0, 0),
            new Vector3 (triangleSize/2, triangleSize, 0)
        };

        //not using triangleSize as uvs are on a scale of 0 - 1
        Vector2[] uvs = new Vector2[]
        {
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (0.5f, 1)
        };

        int[] triangles = new int[]
        {
            0, 1, 2
        };

        triangle.vertices = verticies;
        triangle.uv = uvs;
        triangle.triangles = triangles;

        return triangle;
    }

    public static Mesh MakeSquare(float squareSize)
    {
        Mesh square = new Mesh();

        Vector3[] verticies = new Vector3[]
        {
            //front verts
            new Vector3 (0, 0, 0), //0
            new Vector3 (squareSize, 0, 0), //1
            new Vector3 (squareSize, squareSize, 0), //2
            new Vector3 (0, squareSize, 0), //3
        };

        //not using squareSize as uvs are on a scale of 0 - 1
        Vector2[] uvs = new Vector2[]
        {
            //front uvs
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (1, 1),
            new Vector2 (0, 1),
        };

        int[] triangles = new int[]
        {
            //front face
            2, 1, 0,
            3, 2, 0,
        };

        square.vertices = verticies;
        square.uv = uvs;
        square.triangles = triangles;

        return square;
    }

    public static Mesh MakeCube(float cubeSize)
    {
        Mesh cube = new Mesh();

        Vector3[] verticies = new Vector3[]
        {
            //front verts
            new Vector3 (0, 0, 0), //0
            new Vector3 (cubeSize, 0, 0), //1
            new Vector3 (cubeSize, cubeSize, 0), //2
            new Vector3 (0, cubeSize, 0), //3

            //rear verts
            new Vector3 (0, 0, cubeSize), //4
            new Vector3 (cubeSize, 0, cubeSize), //5
            new Vector3 (cubeSize, cubeSize, cubeSize), //6
            new Vector3 (0, cubeSize, cubeSize), //7

        };

        //not using cubeSize as uvs are on a scale of 0 - 1
        Vector2[] uvs = new Vector2[]
        {
            //front uvs
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (1, 1),
            new Vector2 (0, 1),
            
            //rear uvs
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (1, 1),
            new Vector2 (0, 1)
        };

        int[] triangles = new int[]
        {
            //front face
            2, 1, 0,
            3, 2, 0,
            
            //back face
            //same as front but using rear verts(pos) in reverse order(rot)
            4, 5, 6,
            4, 6 ,7,

            //right face
            6, 5, 1,
            2, 6, 1,

            //left face
            3, 0, 4,
            4, 7, 3,

            //top face
            6, 2, 3,
            7, 6, 3,

            //bottom face
            1, 5, 4,
            1, 4, 0

        };

        cube.vertices = verticies;
        cube.uv = uvs;
        cube.triangles = triangles;

        return cube;
    }
}
