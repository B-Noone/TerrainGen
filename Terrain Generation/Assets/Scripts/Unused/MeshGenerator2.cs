using UnityEngine;

public class MeshGenerator2 : MonoBehaviour
{

    [Header("Mesh Settings")]
    [Range(2, 255)]
    public int mapSize = 241*241;
    public float scale = 20;
    public float elevationScale = 10;
    public Material material;

    [Header("Erosion Settings")]
    public int numErosionIterations = 50000;

    [Header("Animation Settings")]
    public bool animateErosion;
    public int iterationsPerFrame = 100;
    public bool showNumIterations;
    public int numAnimatedErosionIterations { get; private set; }

    float[] map;
    Mesh mesh;
    Erosion2 erosion;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    void Start()
    {
        erosion = FindObjectOfType<Erosion2>();
        Application.runInBackground = true;
    }

    public void StartMeshGeneration(float[] newMap)
    {
        map = newMap;
        GenerateMesh();
    }

    public void Erode(float[] newMap)
    {
        map = newMap;
        erosion = FindObjectOfType<Erosion2>();
        erosion.Erode(map, mapSize, numErosionIterations);
        GenerateMesh();
    }

    void Update()
    {
        if (animateErosion)
        {
            for (int i = 0; i < iterationsPerFrame; i++)
            {
                erosion.Erode(map, mapSize);
            }
            numAnimatedErosionIterations += iterationsPerFrame;
            GenerateMesh();
        }
    }

    void GenerateMesh()
    {
        Vector3[] verts = new Vector3[mapSize * mapSize];
        int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 6];
        int t = 0;

        for (int y = 0; y < mapSize-1; y++)
        {
            for (int x = 0; x < mapSize-1; x++)
            {
                int i = y * mapSize + x;

                Vector2 percent = new Vector2(x / (mapSize - 1f), y / (mapSize - 1f));
                Vector3 pos = new Vector3(percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
                //Debug.Log(i);
                pos += Vector3.up * map[i] * elevationScale;
                verts[i] = pos;

                // Construct triangles
                if (x != mapSize - 1 && y != mapSize - 1)
                {

                    triangles[t + 0] = i + mapSize;
                    triangles[t + 1] = i + mapSize + 1;
                    triangles[t + 2] = i;

                    triangles[t + 3] = i + mapSize + 1;
                    triangles[t + 4] = i + 1;
                    triangles[t + 5] = i;
                    t += 6;
                }
            }
        }

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
        mesh.RecalculateNormals();

        AssignMeshComponents();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
        material.SetFloat("_MaxHeight", elevationScale);
    }

    void AssignMeshComponents()
    {
        // Find/creator mesh holder object in children
        string meshHolderName = "Mesh Holder";
        Transform meshHolder = transform.Find(meshHolderName);
        if (meshHolder == null)
        {
            meshHolder = new GameObject(meshHolderName).transform;
            meshHolder.transform.parent = transform;
            meshHolder.transform.localPosition = Vector3.zero;
            meshHolder.transform.localRotation = Quaternion.identity;
        }

        // Ensure mesh renderer and filter components are assigned
        if (!meshHolder.gameObject.GetComponent<MeshFilter>())
        {
            meshHolder.gameObject.AddComponent<MeshFilter>();
        }
        if (!meshHolder.GetComponent<MeshRenderer>())
        {
            meshHolder.gameObject.AddComponent<MeshRenderer>();
        }

        meshRenderer = meshHolder.GetComponent<MeshRenderer>();
        meshFilter = meshHolder.GetComponent<MeshFilter>();
    }
}