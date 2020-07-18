using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshProperties meshProp)
    {
        meshFilter.sharedMesh = meshProp.CreateMesh();
        //meshRenderer.sharedMaterial.mainTexture = newTexture;
        meshCollider.sharedMesh = meshProp.CreateMesh();
    }

    public void DrawMesh2(OtherMeshProperties meshProp)
    {
        meshFilter.sharedMesh = meshProp.CreateMesh();
        //meshRenderer.sharedMaterial.mainTexture = newTexture;
        meshCollider.sharedMesh = meshProp.CreateMesh();
    }

    public void DrawShader(Shader newShader)
    {
        meshRenderer.sharedMaterial.shader = newShader;
    }
}
