using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
   public static MeshProperties GenerateTerrainMesh(float[,] noiseMap, float heightMultiplier, float levelArea, float levelOfDetail)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float topLeftX = (width - 1) / -2f; //allows centering of mesh
        float topLeftZ = (height - 1) / 2f; //allows centering of mesh

        if(levelOfDetail == 0)
        {
            levelOfDetail = 0.5f;
        }
        float increment = levelOfDetail * 2;

        int verticiesPerLine = (int)((width - 1) / increment) + 1;

        MeshProperties meshProp = new MeshProperties(verticiesPerLine, verticiesPerLine);
        int vertexIndex = 0;

        for (int y=0; y < height; y += (int)increment) //Loop through map
        {
            for(int x=0; x < width; x += (int)increment)
            {
                if (noiseMap[x, y] < levelArea)
                {
                    noiseMap[x, y] = levelArea;
                }
                meshProp.vertices[vertexIndex] = new Vector3(topLeftX + x, noiseMap[x, y] * heightMultiplier, topLeftZ - y);
                meshProp.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                if(x < width-1 && y < height - 1) //Skip vertices on edge and bottom of map
                {
                    meshProp.AddTriangle(vertexIndex, vertexIndex + verticiesPerLine + 1, vertexIndex + verticiesPerLine); //triangle 1 (Has to draw them in clockwise for Unity)
                    meshProp.AddTriangle(vertexIndex + verticiesPerLine + 1, vertexIndex, vertexIndex + 1); //triangle 2 to make a square
                }
                vertexIndex++;
            }
        }
        return meshProp; //Sending properties rather than the mesh for threading
    }
}

public class MeshProperties
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshProperties(int meshWidth, int MeshHeight)
    {
        vertices = new Vector3[meshWidth * MeshHeight];
        uvs = new Vector2[meshWidth * MeshHeight];
        triangles = new int[(meshWidth - 1) * (MeshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh newMesh = new Mesh();
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.uv = uvs;
        newMesh.RecalculateNormals();
        return newMesh;
    }
}