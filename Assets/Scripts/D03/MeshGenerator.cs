using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrain(float[,] heightMap, float heightMult)
    {
        int height = heightMap.GetLength(0);
        int width = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.verts[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, y] * heightMult, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    //making the tris
                    //draws a square
                    meshData.AddTri(vertexIndex, vertexIndex + width + 1, vertexIndex + width); // a = start | b = below a and right | c = below a (drawing a tri)
                    meshData.AddTri(vertexIndex + width + 1, vertexIndex, vertexIndex + 1); //draws the other half of the square with a tri
                }
                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] verts;
    public int[] tris;

    public int triIndex = 0;
    public Vector2[] uvs;

    public MeshData(int meshWidth, int meshHeight)
    {
        verts = new Vector3[meshWidth * meshHeight];
        tris = new int[(meshWidth - 1) * (meshHeight - 1) * 6]; // -1 prevents going out of bounds * 6 (cause 6 verts)
        uvs = new Vector2[meshWidth * meshHeight];
    }

    public void AddTri(int a, int b, int c)
    {
        tris[triIndex] = a;
        tris[triIndex + 1] = b;
        tris[triIndex + 2] = c;
        triIndex += 3;
    }

    public Mesh CreateMesh() //making the mesh from tris here
    {
        Mesh mesh = new Mesh();

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
