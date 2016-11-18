using UnityEngine;
using System.Collections;

public class MeshData  {

    public Vector3[] vertices;
    public int[] triangels;
    public Vector2[] uvs;

    int tringleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        uvs = new Vector2[meshWidth * meshHeight];
        vertices = new Vector3[meshWidth * meshHeight];
        triangels = new int[(meshWidth - 1) * (meshHeight - 1) * 6];

    }

    public void AddTriangle(int a, int b, int c)
    {
        triangels[tringleIndex] = a;
        triangels[tringleIndex+1] = b;
        triangels[tringleIndex+2] = c;
        tringleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh= new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangels;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

}
