using Unity.VisualScripting;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{
    [Header("Map Attributes")]
    public int size = 256;
    public int scale = 20;
    public int seed = 1;
    public int octaves = 4;
    public float lacunarity = 3.3f;
    public float persistence = 0.39f;

    [Header("Mesh Attributes")]
    public AnimationCurve _animationCurve;
    [Range(1, 6)]
    public int levelOfDetail = 1;
    public int heightMult = 5;

    [Header("Debug Texure View")]
    public Texture2D noiseTex; //debugging purposes
    public Texture2D fallTex;
    public Texture2D colourTex;
    public Texture2D combinedTex;

    [System.Serializable] //Allows different materials to be assigned based on height, must be made in inspector
    public struct TerrainType
    {
        public string name;
        public float height;
        public Material _material;
        public Color color;

    }

    public TerrainType[] regions;

    //Rendering Components
    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    Material _material;
    MeshCollider _collider;

    [SerializeField] private ObjectDistribution _objectDistribution;




    //================================================================================================= Extra Spacing to make navigating code easier

    private void Start()
    {
        //ensure the game object has the required components to display
        if(this.gameObject.GetComponent<MeshFilter>() == null)
        {
            _meshFilter = this.AddComponent<MeshFilter>();
        }
        if (this.gameObject.GetComponent<MeshRenderer>() == null)
        {
            _meshRenderer = this.AddComponent<MeshRenderer>();
        }
        if (this.gameObject.GetComponent<MeshCollider>() == null)
        {
            _collider = this.AddComponent<MeshCollider>();
        }
        //set the shader
        _material = new Material(Shader.Find("Unlit/Texture"));

        _meshRenderer.material = _material;

        _objectDistribution = GetComponent<ObjectDistribution>();

        GenerateMaps();
    }

    //the use of globals justified by needing to be able to recall this function later on when changes to the parametres are made
    public void GenerateMaps()
    {
        //create maps according to input map size | Nulling them incase the size of the map is changed

        float[,] noiseMap = new float[size, size];
        float[,] fallOffMap = new float[size, size];
        float[,] combinedMap = new float[size, size];
        Color[,] colourMap = new Color[size, size];

        //generate maps
        noiseMap = NoiseMapGenerator.GenerateNoiseMap(size, size, scale, lacunarity, octaves, persistence, seed, Vector2.zero);
        fallOffMap = NoiseMapGenerator.GenerateFallOffMap(size, _animationCurve);

        //convert maps to Textures so they can be debugged
        noiseTex = MapToTexture(noiseMap);
        fallTex = MapToTexture(fallOffMap);

        //iterate through each part of the combined map, subtracting the value of fallOffMap from noiseMap
        //creates the combined map

        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                //if statement to prevent values below 0
                //not needed but ensures the map doesnt have giant pits around the edges
                if (noiseMap[x, y] - fallOffMap[x, y] > 0) 
                {
                    combinedMap[x, y] = noiseMap[x, y] - fallOffMap[x, y];
                }
                else
                {
                    combinedMap[x, y] = 0;
                }
            }
        combinedTex = MapToTexture(combinedMap);

        //generate the Mesh
        MeshData md = MeshGenerator.GenerateTerrain(combinedMap, heightMult, _animationCurve, levelOfDetail);
        //set the mesh
        _meshFilter.mesh = md.CreateMesh();

        //generate ColourMap
        colourTex = ConvertToColour(combinedMap, colourMap);
        //set the colours
        _material.mainTexture = colourTex;

        //generate a lower detail version of the same map for collision

        int detailOfCollider = levelOfDetail + 2;

        _collider.sharedMesh = MeshGenerator.GenerateTerrain(combinedMap, heightMult, _animationCurve, detailOfCollider).CreateMesh();



        //Add Trees, Rocks, Shrubs and Ships
        _objectDistribution.GenerateTrees(size, seed, 50, 4); //ambiguous numbers, change with UI integrations


        //add more for things like rocks

    }

    private Texture2D MapToTexture(float[,] map)
    {
        Texture2D tex = new Texture2D(map.GetLength(0), map.GetLength(1));
        float currentHeight = new float();

        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
            {
                currentHeight = map[x, y];
                tex.SetPixel(x, y, new Color(currentHeight, currentHeight, currentHeight));
            }


        //apply the texture
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

        return tex;
    }

    //append later to use materials
    private Texture2D ConvertToColour(float[,] _noiseMap, Color[,] _colourMap)
    {
        Texture2D tex = new Texture2D(_noiseMap.GetLength(0), _noiseMap.GetLength(1));
        float currentHeight = new float();

        //generating the colour map
        for (int x = 0; x < tex.width; x++)
            for (int y = 0; y < tex.height; y++)
            {
                currentHeight = _noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++) //compair the current height against each regions height, setting the colour when it matches
                {
                    if (currentHeight <= regions[i].height)
                    {
                        _colourMap[x, y] = regions[i].color;
                        break;
                    }
                }
            }

        //apply colour map to texture
        for (int x = 0; x < _colourMap.GetLength(0); x++)
            for (int y = 0; y < _colourMap.GetLength(1); y++)
            {
                tex.SetPixel(x, y, _colourMap[x, y]);
            }


        //apply the texture
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

        return tex;
    }
}
