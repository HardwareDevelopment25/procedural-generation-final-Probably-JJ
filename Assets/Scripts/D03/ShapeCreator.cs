using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ShapeCreator : MonoBehaviour
{
    public float shapeSize = 1.0f;
    public int mapSize = 128;
    private Texture2D mapTexture; 
    public float currentHeight; //for colourMap function
    Color[,] colourMap; //for colourMap function

    [System.Serializable] //for colourMap function
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

    public TerrainType[] regions;



    void Start()
    {
        mapTexture = new Texture2D(mapSize, mapSize);
        colourMap = new Color[mapSize, mapSize];

        MeshFilter mf = this.AddComponent<MeshFilter>();
        MeshRenderer mr = this.AddComponent<MeshRenderer>();

        Material mat = new Material(Shader.Find("Unlit/Texture"));

        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(mapSize, mapSize, 20, 1, 5, 1, 0, Vector2.zero);


        MeshData md = MeshGenerator.GenerateTerrain(noiseMap, 10.0f);
        mf.mesh = md.CreateMesh();
        
        mr.material = mat;

        //put texture on later
        ColourTheMap(noiseMap, mapTexture);
        mat.mainTexture = mapTexture;
    }

    public void ColourTheMap(float[,] noiseMap, Texture2D tex)
    {
        for (int i = 0; i < tex.width; i++)
            for (int j = 0; j < tex.height; j++)
            {
                currentHeight = noiseMap[i, j];

                for (int k = 0; k < regions.Length; k++)
                {
                    if (currentHeight <= regions[k].height)
                    {
                        colourMap[i, j] = regions[k].color;
                        break;
                    }
                }
            }

        for (int i = 0; i < colourMap.GetLength(0); i++)
            for (int j = 0; j < colourMap.GetLength(1); j++) tex.SetPixel(i, j, colourMap[i, j]);
        tex.Apply();
    }
}
