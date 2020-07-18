using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise2
{
    public static float LacunatiryPerlin(float xPos, float yPos, Vector2[] offset, float lacunarity = 2, float gain = 0.3f, int numLayers = 5)
    {
        float noiseSum = 0;
        float amplitude = 1f;
        for (int i = 0; i < numLayers; ++i)
        {
            // Change in frequency and amplitude
            noiseSum += Mathf.PerlinNoise(xPos + offset[i].x, yPos + offset[i].y) * amplitude;
            xPos *= lacunarity;
            yPos *= lacunarity;
            amplitude *= gain;
        }

        return noiseSum;
    }

    public static float[] GenerateNoiseMap(int mapSize, int seed, float scale, float lacunarity, float gain, int numLayers)
    {
        float[] newMap = new float[mapSize * mapSize];

        System.Random prng = new System.Random(seed); // Pseudo random
        Vector2[] LayerOffsets = new Vector2[numLayers];
        for (int i = 0; i < numLayers; i++)
        {
            // Values too high or low break the random value
            float offsetX = prng.Next(-100000, 100000); 
            float offsetY = prng.Next(-100000, 100000);
            LayerOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Stops scale being 0 to avoid errors
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;
                float perlinValue = LacunatiryPerlin(sampleX, sampleY, LayerOffsets, lacunarity, gain, numLayers);
                newMap[y * mapSize + x] = perlinValue;
            }
        }
        return newMap;
    }
}