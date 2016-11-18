using UnityEngine;
using System.Collections;

public class ShowCurrentMap : MonoBehaviour {

    public Renderer renderer;
    public MeshRenderer meshrendere;
    public MeshCollider meshcollider;
    public MeshFilter meshFilter;

	
    public void DrawTexture(Texture2D tex)
    {
       

        renderer.sharedMaterial.mainTexture = tex;
        renderer.transform.localScale = new Vector3(tex.width,1, tex.height);
       
    }

    public void DrawMesh(Mesh mesh, Texture2D texture)
    {
        meshFilter.sharedMesh = mesh;
        meshrendere.sharedMaterial.mainTexture = texture;
        meshcollider.sharedMesh = mesh;

    }

}
