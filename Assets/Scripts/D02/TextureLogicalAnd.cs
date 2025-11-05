using System;
using Unity.Mathematics;
using UnityEngine;

public class TextureLogicalAnd : MonoBehaviour
{
    public int imageSize = 64; 
    private Texture2D texture;
    public int seed = 0;
    public float scale = 1.0f;
    System.Random random;

    private Color[] pix;

    void Start()
    {
        texture = new Texture2D(imageSize, imageSize);
        random = new System.Random(seed);
        pix = new Color[texture.width * texture.height];



        CreatePattern();

        GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    // Update is called once per frame
    void CreatePattern()
    {
        if (scale == 0) scale = 0.0000001f;
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
        texture.Apply();
    }
}
