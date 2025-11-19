using System;
using Unity.Mathematics;
using UnityEngine;

public class TextureLogicalAnd : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColourMap};
    public DrawMode drawMode = DrawMode.NoiseMap;

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

    public TerrainType[] regions;

    public int imageSize = 64; 
    private Texture2D texture;
    public int seed = 0;
    public float scale = 1.0f;
    System.Random random;

    Color[,] colourMap;

    public float lacunarity = 1.0f;
    public int octaves = 1;
    public float persistance = 1.0f;
    public Vector2 offset = Vector2.zero;
    public float currentHeight;

    //private Color[] pix;

    void Start()
    {
        texture = new Texture2D(imageSize, imageSize);
        random = new System.Random(seed);
        //pix = new Color[texture.width * texture.height];
        colourMap = new Color[imageSize , imageSize];



        CreateMap();
        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            CreateMap();
            GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
    }

    // Update is called once per frame
    void CreateMap()
    {

        /*    
        //go through each pixel in texture
        for(int y  = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float xSample = (float)x / texture.width * scale;
                float ySample = (float)y / texture.height * scale;
                float perlinResult = Mathf.PerlinNoise(xSample, ySample);
                pix[(int)y * texture.width + (int)x] = new Color(perlinResult, perlinResult, perlinResult);


                //Color pixelColour = (random.Next(0,2) != 0 ? Color.white : Color.black);
            }
        }
        texture.SetPixels(pix); 
        */

        float[,] noiseMap = NoiseMapGenerator.GenerateNoiseMap(imageSize, imageSize, scale, lacunarity, octaves, persistance, random.Next(), offset);
        for (int i = 0; i < noiseMap.GetLength(0); i++)
            for (int j = 0; j < noiseMap.GetLength(1); j++) texture.SetPixel(i, j, new Color(noiseMap[i, j], noiseMap[i, j], noiseMap[i, j]));
        texture.Apply();


        ColourTheMap(noiseMap, texture);
    }


    public void ColourTheMap(float[,] noiseMap, Texture2D tex)
    {
        for(int i = 0; i < tex.width; i++)
            for(int j = 0; j < tex.height; j++)
            {
                currentHeight = noiseMap[i, j];

                for (int k = 0; k < regions.Length; k++)
                {
                    if(currentHeight <= regions[k].height)
                    {
                        colourMap[i,j] = regions[k].color;
                        break;
                    }
                }
            }

        for (int i = 0; i < colourMap.GetLength(0); i++)
            for (int j = 0; j < colourMap.GetLength(1); j++) tex.SetPixel(i, j, colourMap[i,j]);
        tex.Apply();
        drawMode = DrawMode.ColourMap;
    }
}
