using System.Drawing;
using UnityEngine;

public static class NoiseMapGenerator
{
    public static float[,] GenerateNoiseMap (int mapHeight, int mapWidth, float scale, float lacunarity, int octaves, float persistance, int seed, Vector2 offset)
    {
        float[,] noiseMap = new float[mapHeight, mapWidth];
        if (scale < 0.0f) scale = 0.00001f; //ensures we dont divide by 0

        float maxPossibleHeight = float.MinValue;
        float minPossibleHeight = float.MaxValue;

        System.Random rand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves]; //try keep octaves below 5
        
        //creates x amount of samples of octave offsets for randomness
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000) + offset.x;
            float offsetY = rand.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //create the perlin map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amp = 1, frequency = 1, noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (float)(x - (mapWidth / 2)) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (float)(y - (mapHeight / 2)) / scale * frequency + octaveOffsets[i].y;

                    float perlinResult = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinResult * amp;
                    amp += frequency;
                    frequency += lacunarity;
                }

                //we're after the highest peak and the lowest peak for lerp
                if(noiseHeight>maxPossibleHeight)maxPossibleHeight = noiseHeight;
                else if(noiseHeight < minPossibleHeight) minPossibleHeight = noiseHeight;

                noiseMap[x,y] = Mathf.InverseLerp(minPossibleHeight,maxPossibleHeight, noiseHeight);
            }
        }

        return noiseMap;
    }

    public static float[,] GemerateFallOffMap(int size, AnimationCurve ac)
    {
        float[,] fallOffMap = new float[size, size];

        for (int i = 0;i < size;i++)
            for (int j = 0;j < size;j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                fallOffMap[i,j] = ac.Evaluate(value); //uses unity curve in inspector
            }
        return fallOffMap;
    }

}
