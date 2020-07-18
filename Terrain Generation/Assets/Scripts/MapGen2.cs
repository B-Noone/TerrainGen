using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class MapGen2 : MonoBehaviour
{
    // Different methods of drawing the map
    public enum DrawMode { NoiseMap, ColourMap, BlankMap };
    [Header ("Draw Settings")] // Headers used to split up options in editor
    public DrawMode drawMode;
    public Material material;

    [Header ("Map Settings")]
    public bool drawMesh = true; // Decides if the map has depth
    public bool autoUpdate; // Update the map as settings change
    public int seed;
    [Range(2, 500)]
    public int levelOfDetail = 255; // Number of vertices per line
    [Range(50, 500)]
    public float mapSize = 10; // Space between verticies
    [Range(0,10000)]
    public float heightMultiplier = 10;

    [Header("Noise Settings")]
    [Range(0,1000)]
    public float noiseScale;
    [Range(0, 7)]
    public float lacunarity = 2;
    [Range(0, 1)]
    public float gain = 0.5f;
    [Range(0,10)]
    public int numOfLayers = 5;

    [Header("Shader Settings")]
    public bool customShader = false; // Allows the user to enable shader
    public Shader newShader;
    private Shader defaultShader;

    [Header("Erosion Settings")]
    public bool erosion = false;
    public int numOfIterations = 10000;

    Erosion2 erosionSimulator;

    Mesh mesh = null;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Renderer textureRender;
    MeshCollider meshCollider;

    // Faster than a 2d array and easier to use with certain algorithms
    float[] noiseMap; 
    Color[] colourMap;
    public TerrainType2[] regions;

    public Stopwatch stopwatch;

    // Terrain would unload during play
    void Start()
    {
        GenerateMap();
    }
    
    // Gets data for noise map
    public void GenerateMap()
    {
        stopwatch = new Stopwatch();
        erosionSimulator = FindObjectOfType<Erosion2>();
        noiseMap = Noise2.GenerateNoiseMap(levelOfDetail, seed, noiseScale, lacunarity, gain, numOfLayers);
        stopwatch.Start();
        if (erosion)
        {
            HydraulicErosion();
        }
        GenerateMesh();
        stopwatch.Stop();
        print("Run time: " + stopwatch.Elapsed);

    }

    // Runs erosion simulation
    public void HydraulicErosion()
    {
        erosionSimulator.Erode(noiseMap, levelOfDetail, numOfIterations);
    }

    public void GenerateMesh()
    {
        Vector3[] verts = new Vector3[levelOfDetail * levelOfDetail]; // Stores verticies
        int[] triangles = new int[(levelOfDetail - 1) * (levelOfDetail - 1) * 6];
        Vector2[] uvs = new Vector2[levelOfDetail* levelOfDetail]; // Stores texture offsets
        int vertex = 0;

        for (int y = 0; y < levelOfDetail; y++)
        {
            for (int x = 0; x < levelOfDetail; x++)
            {
                // Equation for converting 1d array into readable data
                int vertexIndex = y * levelOfDetail + x; 

                Vector2 percent = new Vector2(x / (levelOfDetail - 1f), y / (levelOfDetail - 1f));
                Vector3 pos = new Vector3(percent.x * 2 - 1, 0, percent.y * 2 - 1) * mapSize;
                if (drawMesh) // If the map has depth or not
                {
                    pos += Vector3.up * noiseMap[vertexIndex] * heightMultiplier;
                }
                verts[vertexIndex] = pos;
                uvs[vertexIndex] = new Vector2(x / (float)levelOfDetail, y / (float)levelOfDetail);

                // Construct triangles 2 at a time as will only be made in squares
                if (x != levelOfDetail - 1 && y != levelOfDetail - 1)
                {

                    triangles[vertex + 0] = vertexIndex + levelOfDetail;
                    triangles[vertex + 1] = vertexIndex + levelOfDetail + 1;
                    triangles[vertex + 2] = vertexIndex;

                    triangles[vertex + 3] = vertexIndex + levelOfDetail + 1;
                    triangles[vertex + 4] = vertexIndex + 1;
                    triangles[vertex + 5] = vertexIndex;
                    vertex += 6;
                }
            }
        }

        CreateMesh(verts, triangles, uvs);
    }

    // Sets mesh values
    public void CreateMesh(Vector3[] verts, int[] triangles, Vector2[] uvs)
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        else
        {
            mesh.Clear();
        }

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        CreateMeshProperties();
        SetMeshCollider(mesh);
        DrawMap();
    }

    public void SetMeshCollider(Mesh newMesh)
    {
        meshCollider.sharedMesh = null; // Unity can have issues if not set to null first
        meshCollider.sharedMesh = newMesh;
    }

    // Makes sure mesh and mesh components exist
    public void CreateMeshProperties()
    {
        string meshParentName = "Mesh Parent";
        Transform meshParent = transform.Find(meshParentName);
        if (meshParent == null)
        {
            meshParent = new GameObject(meshParentName).transform;
            meshParent.transform.parent = transform;
            meshParent.transform.localPosition = Vector3.zero;
            meshParent.transform.localRotation = Quaternion.identity;
        }

        // Ensure mesh renderer and filter components are assigned
        if (!meshParent.gameObject.GetComponent<MeshFilter>())
        {
            meshParent.gameObject.AddComponent<MeshFilter>();
        }
        if (!meshParent.GetComponent<MeshRenderer>())
        {
            meshParent.gameObject.AddComponent<MeshRenderer>();
        }
        if (!meshParent.GetComponent<Renderer>())
        {
            meshParent.gameObject.AddComponent<Renderer>();
        }
        if (!meshParent.gameObject.GetComponent<MeshCollider>())
        {
            meshParent.gameObject.AddComponent<MeshCollider>();
        }

        meshRenderer = meshParent.GetComponent<MeshRenderer>();
        meshFilter = meshParent.GetComponent<MeshFilter>();
        textureRender = meshParent.GetComponent<Renderer>();
        meshCollider = meshParent.GetComponent<MeshCollider>();
    }

    public void DrawMap()
    {
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
        if (drawMode == DrawMode.ColourMap)
        {
            colourMap = new Color[levelOfDetail * levelOfDetail];
            for (int y = 0; y < levelOfDetail; y++)
            {
                for (int x = 0; x < levelOfDetail; x++)
                {
                    float currentHeight = noiseMap[y*levelOfDetail+x];
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colourMap[y *levelOfDetail+x] = regions[i].colour;
                            break;
                        }
                    }
                }
            }
            DrawTexture(TextureGenerator2.colourMapTexture(colourMap, levelOfDetail));
        }
        else if(drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(TextureGenerator2.noiseMapTexture(noiseMap, levelOfDetail));
        }
        else if (drawMode == DrawMode.BlankMap)
        {
            Color[] fakeMap = new Color[levelOfDetail * levelOfDetail];
            Color defaultColour = new Color(1, 1, 1);
            // Sets map colour to be white
            for (int i = 0; i < fakeMap.Length; i++)
            {
                fakeMap[i] = defaultColour;
            }
            DrawTexture(TextureGenerator2.colourMapTexture(fakeMap, levelOfDetail));
        }
        if (customShader)
        {
            DrawShader(newShader);
        }
        else
        {
            DrawShader(Shader.Find("Standard"));
        }
    }

    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawShader(Shader newShader)
    {
        meshRenderer.sharedMaterial.shader = newShader;
    }
}

[System.Serializable]
public struct TerrainType2
{
    public string terrainName;
    public float height;
    public Color colour;
}