using UnityEngine;
using System.Collections;

public static class MeshGenerator  {

    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve curve)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        MeshData meshData = new MeshData(width, height);

        float topLeftX = (width -1) / -2f;
        float topLeftY = (height-1 ) / 2f;

        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {


                // creates a verticies and center them
                 meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, curve.Evaluate( heightMap[x, y])*heightMultiplier, topLeftY -y);
                
               // meshData.vertices[vertexIndex] = new Vector3(width- x, curve.Evaluate(heightMap[x, y]) * heightMultiplier, height- y);

                //uv goes from 0 to 1 so the percentage is used so a texture can be added to the mesh and it fits perfectly
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                //will create 2 triangels for each verticie except the border one
                if(x < (width -1) && y < (height -1))
                {
                    meshData.AddTriangle( vertexIndex + width + 1, vertexIndex + width, vertexIndex);
                   // meshData.AddTriangle( vertexIndex,vertexIndex + width , vertexIndex + width+1);

                   meshData.AddTriangle( vertexIndex, vertexIndex + 1, vertexIndex + width + 1);
                   // meshData.AddTriangle(vertexIndex,  vertexIndex + width + 1, vertexIndex + 1);

                }


                vertexIndex++;
            }
        }
        return meshData;
    }


}
