using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D colourMapTexture(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D noiseMapTexture(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //colour map is 1D array, so getting index using 
                //(y*width)//Gets Row index
                //+x Gets colomn
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]); //Sets colour based on percentage between 0 and 1, same as noise map
            }
        }
        //texture.SetPixels(colourMap);
        //texture.Apply();
        return colourMapTexture(colourMap, width, height);
    }

    //public static Texture2D customTexture(Material oldMat)
    //{
    //    Texture2D newTexture = oldMat.GetTexture();

    //    return newTexture;

    //}
}
