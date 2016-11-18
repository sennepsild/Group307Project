using UnityEngine;
using System.Collections;

public static class Noise {

	public static float[,] GenerateNoiseMap(int width,int height,int seed,float scale,int iterations, float iterationItensity, float iterarationSize, Vector2 offset)
    {
        System.Random Seed = new System.Random(seed);
        Vector2[] iterationOffset = new Vector2[iterations];
        for (int i = 0; i < iterations; i++)
        {
            //I currently dont know how the random function works exactly(other than the obvious ofc), gonna read up on that
            float offsetX = Seed.Next(-100000, 100000)+ offset.x;
            float offsetY = Seed.Next(-100000, 100000)+ offset.y;

            iterationOffset[i] = new Vector2(offsetX, offsetY);
        }
 

        float[,] noisemap = new float[width, height];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //will determine the strenght of the noise, will decrease for each iteration
                float amplitude = 1;

                //will determine the size of the noise, will increase with each iteration
                float frequency = 1;


                float noiseHeight = 0;

                for (int i = 0; i < iterations; i++)
                {
                    float xpos = (x - width/2) / scale * frequency + iterationOffset[i].x;
                    float ypos = (y - height/2) / scale * frequency + iterationOffset[i].y;

                    //only adding noise will make the image very bright, therefore letting the noise go in negative is better 
                    float noise = Mathf.PerlinNoise(xpos, ypos)*2-1;
                    noiseHeight += noise * amplitude;

                    //persistance needs to be less than 1
                    amplitude *= iterationItensity;

                    //iterarationSize needs to be more than 1
                    frequency *= iterarationSize;

                    
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noisemap[x, y] = noiseHeight;

               
            }
        }
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noisemap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noisemap[x, y]);

                // make the noise go from 0.3 to 0.7
                noisemap[x, y] = Mathf.Lerp(0.3f, 0.7f, noisemap[x, y]);
            }
        }


                return noisemap;
    }


}
