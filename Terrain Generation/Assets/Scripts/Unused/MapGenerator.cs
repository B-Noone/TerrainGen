using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, CustomMap};
    public DrawMode drawMode;

    public float noiseScale;
    public float lacunarity = 2;
    public float gain = 0.5f;
    public int numOfLayers = 5;
    public float levelArea = .0f;
    public float heightMultiplier;
    [Range(50, 241)]
    const int mapSize = 241;
    [Range(0,6)]
    public int levelOfDetail;

    public bool drawMesh = true;

    public int seed;
    public bool customShader = false;
    public Shader newShader;
    private Shader defaultShader;
    public bool autoUpdate;

    Noise noise;
    Erosion2 erosionSimluator;
    MeshGenerator2 meshGen;
    public bool erosion = false;
    public int numberOfIterations = 50000;

    public TerrainType[] regions;


    //void Start()
    //{
    //    GenerateMap();
    //}

    public void GenerateMap()
    {
        //Attempts to make map size changeable
        //if (levelOfDetail != 0)
        //{
        //float remainder = 0;
        //mapSize = (((mapSize-1) % levelOfDetail) != 0) ? ((mapSize-1) - ((mapSize-1) % levelOfDetail))+1 : mapSize; //Makes sure map value is divisible by level of detail
        //remainder = (float)(mapSize-1) % (float)levelOfDetail;
        //print("mapSize = " + mapSize);
        //print("levelOfDetail = " + levelOfDetail);
        //print("remainder = " + remainder);
        //if(remainder != 0)
        //{

        //    mapSize = (mapSize - (int)remainder);
        //    print("New Map = " + mapSize);
        //}
        //}
        //if(customShader == false)
        //{
        //    defaultShader = Shader.Find("Unlit/Texture");
        //}
        erosionSimluator = FindObjectOfType<Erosion2>();
        noise = FindObjectOfType<Noise>();
        meshGen = FindObjectOfType<MeshGenerator2>();
        defaultShader = Shader.Find("Standard");
        float[,] noiseMap = noise.GenerateNoiseMap(mapSize, mapSize, seed, noiseScale, lacunarity, gain, numOfLayers);


        Color[] colourMap = new Color[mapSize * mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.noiseMapTexture(noiseMap));
            //display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, 0, levelArea, levelOfDetail));
        }
        else if(drawMode == DrawMode.ColourMap){
            display.DrawTexture(TextureGenerator.colourMapTexture(colourMap, mapSize, mapSize));
        }
        else if (drawMode == DrawMode.CustomMap)
        {
            Color[] fakeMap = new Color[mapSize * mapSize];
            Color defaultColour = new Color(1, 1, 1);
            for(int i = 0; i < fakeMap.Length; i++)
            {
                fakeMap[i] = defaultColour;
            }
            display.DrawTexture(TextureGenerator.colourMapTexture(fakeMap, mapSize, mapSize));
        }
        if (drawMesh)
        {
            meshGen.StartMeshGeneration(noise.GetMap());
            //display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, levelArea, levelOfDetail));
            //display.DrawMesh2(OtherMeshGenerator.GenerateMesh(mapSize, noise.GetMap(), levelOfDetail, heightMultiplier));
        }
        else
        {
            //display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, 0, levelArea, levelOfDetail));
            //display.DrawMesh2(OtherMeshGenerator.GenerateMesh(mapSize, noise.GetMap(), levelOfDetail, heightMultiplier));
        }
        if (customShader)
        {
            display.DrawShader(newShader);
        }
        else
        {
            display.DrawShader(defaultShader);
        }
        if (erosion)
        {
            erosionSimluator.Erode(noise.GetMap(), mapSize, numberOfIterations);
        }
    }

    private void OnValidate() //////////////// DO LATER//////////////// DO LATER//////////////// DO LATER//////////////// DO LATER//////////////// DO LATER
    {
        
    }

    public static int GetMapSize()
    {
        return mapSize;
    }
}

[System.Serializable]
public struct TerrainType {
    public string terrainName;
    public float height;
    public Color colour;
}