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

    [Header("Map Details")]
    public int treeAmount = 10;
    public int rockAmount = 10;
    public int shipAmount = 10;
    public int unitformity = 5;



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
    //regions made in inspector so this needs to be populated in there too
    public float treeRegionLower;
    public float treeRegionUpper;
    public float rockRegionLower;
    public float rockRegionUpper;
    public float shipRegionLower;
    public float shipRegionUpper;

    //Rendering Components
    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    Material _material;
    MeshCollider _collider;

    [SerializeField] private ObjectDistribution _objectDistribution;
    [SerializeField] private ButtonEventLogic _buttonLogic;
    private GameObject spawnedObjects;



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
        _buttonLogic = GameObject.FindGameObjectWithTag("UI").GetComponent<ButtonEventLogic>();

        GenerateMaps();
    }

    //the use of globals justified by needing to be able to recall this function later on when changes to the parametres are made
    public void GenerateMaps()
    {
        Destroy(spawnedObjects);
        spawnedObjects = new GameObject();
        spawnedObjects.name = "SpawnedObjects";




        //create maps according to input map size
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


        //each stored at a lower number that is here multiplied by map case (so there is an appropriate amount of each on their respective map sizes)
        treeAmount *= _buttonLogic.GetMapCase();
        rockAmount *= _buttonLogic.GetMapCase();
        shipAmount *= _buttonLogic.GetMapCase();

        //Add Trees, Rocks and Ships
        _objectDistribution.GenerateTrees(spawnedObjects, size, seed, treeAmount, unitformity, combinedMap, treeRegionUpper, treeRegionLower);
        _objectDistribution.GenerateRocks(spawnedObjects, size, seed, rockAmount, unitformity, combinedMap, rockRegionUpper, rockRegionLower);
        _objectDistribution.GenerateShips(spawnedObjects, size, seed, shipAmount, unitformity, combinedMap, shipRegionUpper, shipRegionLower);

        //reset to their stored value
        treeAmount /= _buttonLogic.GetMapCase();
        rockAmount /= _buttonLogic.GetMapCase();
        shipAmount /= _buttonLogic.GetMapCase();
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
