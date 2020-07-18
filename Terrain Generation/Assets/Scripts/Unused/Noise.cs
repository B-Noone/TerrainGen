using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour //maybe static
{
    float[] newMap;
    //float[] newMap2;

    public static float LacunatiryPerlin(float xPos, float yPos, Vector2[] offset, float lacunarity = 2, float gain = 0.3f, int numLayers = 5)
    {
        float noiseSum = 0;
        float amplitude = 1f;
        for (int i = 0; i < numLayers; ++i)
        {
            // change in frequency and amplitude
            noiseSum += Mathf.PerlinNoise(xPos + offset[i].x, yPos + offset[i].y) * amplitude;
            xPos *= lacunarity;
            yPos *= lacunarity;
            amplitude *= gain;
            //lacunarity *= 2;
        }

        return noiseSum;
    }

    public static float LacunatiryPerlin2(int mapScale, float xPos, float yPos, float initialScale, Vector2[] offset, int x, int y, float lacunarity = 2, float gain = 0.3f, int numLayers = 5)
    {

        //float[] map = new float[mapScale * mapScale];
        float noiseSum = 0;
        float amplitude = 1f;
        float scale = initialScale;

        for (int i = 0; i < numLayers; i++)
        {
            // change in frequency and amplitude
            Vector2 tempNoise = offset[i] + new Vector2(x / (float)mapScale, y / (float)mapScale) * scale;
            noiseSum += Mathf.PerlinNoise(tempNoise.x, tempNoise.y) * amplitude;
            //noiseSum += Mathf.PerlinNoise(xPos + offset[i].x, yPos + offset[i].y) * amplitude;
            //xPos *= lacunarity;
            //yPos *= lacunarity;
            amplitude *= gain;
            scale *= lacunarity;
            //lacunarity *= 2;
        }
        return noiseSum;
        //map[y * mapScale + x] = noiseSum;
        //minValue = Mathf.Min(noiseSum, minValue);
        //maxValue = Mathf.Max(noiseSum, maxValue);
        //return map;
        //return noiseSum;
    }

    public float[,] GenerateNoiseMap(int width, int height, int seed, float scale, float lacunarity, float gain, int numLayers)
    {
        float[,] noiseMap = new float[width, height];
        float[] tempMap = new float[width * height];
        newMap = new float[width * height];

        System.Random prng = new System.Random(seed); //pseudo random
        Vector2[] LayerOffsets = new Vector2[numLayers];
        for(int i = 0; i < numLayers; i++)
        {
            float offsetX = prng.Next(-100000, 100000); //Values too high or low break the random value
            float offsetY = prng.Next(-100000, 100000);
            LayerOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if(scale <= 0)
        {
            scale = 0.0001f;
        }
        int iterations = 0;
        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                //float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                float perlinValue = LacunatiryPerlin(sampleX, sampleY, LayerOffsets, lacunarity, gain, numLayers);
                float perlinValue2 = LacunatiryPerlin2(width, sampleX, sampleY, scale, LayerOffsets, x, y, lacunarity, gain, numLayers);
                //noiseMap = perlinValue2;
                noiseMap[x, y] = perlinValue2;
                //newMap[y *width + x] = perlinValue;
                newMap[y*width+x] = perlinValue2;
                //Debug.Log("NoiseMap: " + noiseMap[x, y]);
                //Debug.Log("newMap: " + newMap[y*width+x]);
                iterations++;
                //newMap2[y*width+x] = perlinValue2;
            }
        }
        //int itit = 0;
        //for(int x = 0; x < width; x++)
        //{
        //    Debug.Log("x:" + x + " y:" + 0 + " value: " + noiseMap[x, 0]);
        //    Debug.Log("iteration: " + x + " value: " + newMap[x]);
        //   // itit++;
        //}
        Debug.Log(GetMap()[58080]);
        return noiseMap;
    }

    //public void SetMap(int y, int x, int mapScale, float noiseSum)
    //{
    //    newMap[y * mapScale + x] = noiseSum;
    //}

    public float[] GetMap()
    {
        return newMap;
    }
}