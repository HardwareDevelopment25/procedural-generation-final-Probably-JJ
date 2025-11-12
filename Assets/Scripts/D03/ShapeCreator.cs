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
    public AnimationCurve ac;
    public Material waterLevelMat;

    [Range(1, 6)]
    public int levelOfDetail = 1;

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

        GameObject waterFilterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterFilterPlane.transform.localScale = new Vector3((float)mapSize/10.0f, 1, (float)mapSize/10.0f);
        waterFilterPlane.GetComponent<Renderer>().material = waterLevelMat;

        MeshFilter mf = this.AddComponent<MeshFilter>();
        MeshRenderer mr = this.AddComponent<MeshRenderer>();

        Material mat = new Material(Shader.Find("Unlit/Texture"));

        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(mapSize, mapSize, 20, 1, 5, 1, 0, Vector2.zero);
        float[,] falloffmap = NoiseMapGenerator.GemerateFallOffMap(mapSize, ac);
        float[, ] combinedMap = new float[mapSize, mapSize];
        for (int i = 0; i < noiseMap.GetLength(0); i++)
        {
            for (int j = 0; j < noiseMap.GetLength(1); j++)
            {
                combinedMap[i, j] = noiseMap[i, j] - falloffmap[i, j];
            }
        }


        MeshData md = MeshGenerator.GenerateTerrain(combinedMap, 5.0f, ac, levelOfDetail);
        mf.mesh = md.CreateMesh();
        
        mr.material = mat;

        //put texture on later
        ColourTheMap(combinedMap, mapTexture);
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
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
    }
}
