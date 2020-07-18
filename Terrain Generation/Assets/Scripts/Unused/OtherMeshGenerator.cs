using UnityEngine;

public class OtherMeshGenerator : MonoBehaviour
{
    public static OtherMeshProperties GenerateMesh(int mapSize, float[] map, float levelOfDetail, float heightMultiplier)
    {
        if (levelOfDetail == 0)
        {
            levelOfDetail = 0.5f;
        }
        float increment = levelOfDetail * 2;
        int verticiesPerLine = (int)((mapSize - 1) / increment) + 1;
        OtherMeshProperties oMeshProp = new OtherMeshProperties(verticiesPerLine, verticiesPerLine);
        int vertexIndex = 0;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                //vertexIndex = y * mapSize + x;
                int i = y * mapSize + x;
                Vector2 percent = new Vector2(x / (mapSize - 1f), y / (mapSize - 1f));
                Vector3 pos = new Vector3(percent.x * 2 - 1, 0, percent.y * 2 - 1);
                pos += Vector3.up * map[i] * heightMultiplier;
                //verts[i] = pos;
                //oMeshProp.vertices[vertexIndex] = pos;
                oMeshProp.vertices[i] = pos;
                oMeshProp.uvs[vertexIndex] = percent;

                // Construct triangles
                if (x < mapSize - 1 && y < mapSize - 1)
                {
                    oMeshProp.AddTriangle(vertexIndex + mapSize, vertexIndex + mapSize + 1, vertexIndex);
                    oMeshProp.AddTriangle(vertexIndex + mapSize + 1, vertexIndex + 1, vertexIndex);
                }
                vertexIndex++;
            }
        }
        return oMeshProp;
    }
}

public class OtherMeshProperties
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public OtherMeshProperties(int meshWidth, int MeshHeight)
    {
        vertices = new Vector3[meshWidth * MeshHeight];
        uvs = new Vector2[meshWidth * MeshHeight];
        triangles = new int[(meshWidth-1) * (MeshHeight-1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (triangleIndex < triangles.Length)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    public Mesh CreateMesh()
    {
        Mesh newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.uv = uvs;
        newMesh.RecalculateNormals();
        return newMesh;
    }
}