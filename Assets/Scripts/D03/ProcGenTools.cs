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
}
