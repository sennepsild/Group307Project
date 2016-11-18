using UnityEngine;
using System.Collections;

public static class TextureGenerator {

	public static Texture2D TextureFromColorArray(int width,int height, Color[] colors)
    {
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixels(colors);
        tex.Apply();
        return tex;

    }

    public static Texture2D HeightMapFromFloatArray(float[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        

        Color[] colors = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colors[y * width + x] = Color.Lerp(Color.black, Color.white, map[x, y]);

            }
        }
        
        return TextureFromColorArray(width,height,colors);
    }


}
