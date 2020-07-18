using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator2
{
    public static Texture2D colourMapTexture(Color[] colourMap, int mapSize)
    {
        Texture2D texture = new Texture2D(mapSize, mapSize);
        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D noiseMapTexture(float[] noiseMap, int levelOfDetail)
    {
        Texture2D texture = new Texture2D(levelOfDetail, levelOfDetail);

        Color[] colourMap = new Color[levelOfDetail * levelOfDetail];
        for (int y = 0; y < levelOfDetail; y++)
        {
            for (int x = 0; x < levelOfDetail; x++)
            {
                int vertexIndex = y * levelOfDetail + x;
                // Sets colour based on percentage between 0 and 1, same as noise map
                colourMap[vertexIndex] = Color.Lerp(Color.black, Color.white, noiseMap[y*levelOfDetail+x]);
            }
        }
        return colourMapTexture(colourMap, levelOfDetail);
    }
}