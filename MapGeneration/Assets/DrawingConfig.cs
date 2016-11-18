using UnityEngine;
using System.Collections;

public class DrawingConfig : MonoBehaviour {

    enum ObjectType { mountain,tree,water, house, rocks };

    public static float[,] mountainsHeight;
    public static float[,] waterHeight;
    public static float[,] treePlacement;
    public static float[,] housePlacement;
    public static float[,] rockPlacement;



    public int width;
    public int height;
    public Renderer rendere;
    public Texture2D originalTex;
    Texture2D texture;
    Color[] pixels;
    Color[] pixels2;
    Color[] fillpix;

    //blobs
    ObjectType currentobjectype;
    int[,] mountains;
    int numberOfMountains;
    int[,] trees;
    int numberOfTrees;
    int[,] water;
    int numberOfWater;
    int[,] houses;
    int numberOfHouses;

    int[,] rocks;
    int numberOfRocks;

    bool[,] filled;

    public void Reset()
    {

        texture = new Texture2D(originalTex.width, originalTex.height,TextureFormat.RGB24,false);
        texture.SetPixels(originalTex.GetPixels());
        texture.Apply();
      //  TextureScale.Point(texture, width, height);
        Debug.Log(texture.width + " " + texture.height);
        rendere.transform.localScale = new Vector3(texture.width, 1, texture.height);
        rendere.sharedMaterial.mainTexture = texture;
    }


    public void NormalizeRGB()
    {
        Texture2D normalizedRGB = new Texture2D(texture.width,texture.height);
        pixels = texture.GetPixels();
        pixels2 = texture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
        {

            if (pixels[i].r < 0.1 && pixels[i].g < 0.1 && pixels[i].b < 0.1)
            {
                pixels2[i] = Color.black;
            }
            else {
                pixels2[i].r = pixels[i].r / (pixels[i].r + pixels[i].g + pixels[i].b);
                pixels2[i].g = pixels[i].g / (pixels[i].r + pixels[i].g + pixels[i].b);
                pixels2[i].b = pixels[i].b / (pixels[i].r + pixels[i].g + pixels[i].b);
            }

            
        }

        normalizedRGB.SetPixels(pixels2);
        normalizedRGB.Apply();
        rendere.sharedMaterial.mainTexture = normalizedRGB;
    }

    public void isolateRocks()
    {

        currentobjectype = ObjectType.rocks;
        Texture2D rockmap = new Texture2D(texture.width, texture.height);
        pixels = getCurrentPixs();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] == Color.black)
                pixels2[i] = Color.white;
            else
                pixels2[i] = Color.black;
        }
        rockmap.SetPixels(pixels2);
        rockmap.Apply();
        rockmap = BinaryMedianFilter(rockmap);



        rendere.sharedMaterial.mainTexture = rockmap;


        housePlacement = new float[rockmap.width, rockmap.height];

        GrassFire(rockmap);
        housePlacement = blobMiddle(rocks, numberOfRocks, rockPlacement);


    }

    public void isolateHouses()
    {

        currentobjectype = ObjectType.house;
        Texture2D housemap = new Texture2D(texture.width, texture.height);
        pixels = getCurrentPixs();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i] == Color.black)
                pixels2[i] = Color.white;
            else
                pixels2[i] = Color.black;
        }
        housemap.SetPixels(pixels2);
        housemap.Apply();
        housemap = BinaryMedianFilter(housemap);



        rendere.sharedMaterial.mainTexture = housemap;

        
        housePlacement = new float[housemap.width, housemap.height];

        GrassFire(housemap);
        housePlacement = blobMiddle(houses, numberOfHouses, housePlacement);

    
    }


    public void IsolateMountains()
    {
        currentobjectype = ObjectType.mountain;
        Texture2D MountainMap = new Texture2D(texture.width, texture.height);
        pixels = getCurrentPixs();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r > pixels[i].g && pixels[i].r > pixels[i].b && pixels[i].r > 0.37)
                pixels2[i] = Color.white;
            else
                pixels2[i] = Color.black;
        }
        MountainMap.SetPixels(pixels2);
        MountainMap.Apply();
        MountainMap = BinaryMedianFilter(MountainMap);
        MountainMap = meanfilter(MountainMap);

        pixels = MountainMap.GetPixels();
        mountainsHeight = new float[MountainMap.width, MountainMap.height];
        for (int y = 0; y < MountainMap.height; y++)
        {
            for (int x = 0; x < MountainMap.width; x++)
            {
                if (pixels[y * MountainMap.width + x] != Color.black)
                    mountainsHeight[x, y] = pixels[y * MountainMap.width + x].r;
            }
        }


        rendere.sharedMaterial.mainTexture = MountainMap;
       // GrassFire(MountainMap);
    }

    public void IsolateWater()
    {
        currentobjectype = ObjectType.water;
        Texture2D Watermap = new Texture2D(texture.width, texture.height,TextureFormat.ARGB32,false);
        pixels = getCurrentPixs();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].b > pixels[i].g && pixels[i].b > pixels[i].r && pixels[i].b > 0.4)
                pixels2[i] = Color.white;
            else
                pixels2[i] = Color.black;
        }
        Watermap.SetPixels(pixels2);
        Watermap.Apply();
        Watermap = BinaryMedianFilter(Watermap);
        Watermap = meanfilter(Watermap);

        //TextureScale.Point(Watermap, Watermap.width/2, Watermap.height/2);
        //TextureScale.Point(Watermap, Watermap.width / 2, Watermap.height / 2);
        // rendere.transform.localScale = new Vector3(Watermap.width, 1, Watermap.height);

        //Watermap = work(Watermap);



        //  GrassFire(Watermap);
        //  Watermap= blobMiddle(water,Watermap,numberOfWater);

        pixels = Watermap.GetPixels();
        waterHeight = new float[Watermap.width, Watermap.height];
        for (int y = 0; y < Watermap.height; y++)
        {
            for (int x = 0; x < Watermap.width; x++)
            {
                if (pixels[y * Watermap.width + x] != Color.black)
                    waterHeight[x, y] = pixels[y * Watermap.width + x].r;
            }
        }


        rendere.sharedMaterial.mainTexture = Watermap;

    }


    public void findTrees()
    {
        currentobjectype = ObjectType.tree;
        Texture2D Treemap = new Texture2D(texture.width, texture.height);
        pixels = getCurrentPixs();

        for (int i = 0; i < pixels.Length; i++)
        {
            if(pixels[i].g > pixels[i].r && pixels[i].g > pixels[i].b && pixels[i].g >0.4)
              pixels2[i] = Color.white;
            else
              pixels2[i] = Color.black;
        }
        Treemap.SetPixels(pixels2);
        Treemap.Apply();
        Treemap = BinaryMedianFilter(Treemap);

        

        rendere.sharedMaterial.mainTexture = Treemap;

        treePlacement = new float[Treemap.width, Treemap.height];

        GrassFire(Treemap);
        treePlacement = blobMiddle(trees, numberOfTrees, treePlacement);

    }

    Texture2D work(Texture2D tex)
    {

      


        fillpix = tex.GetPixels();
     
                    replaceblack(10, 10, tex.width,tex.height);
         

        tex.SetPixels(fillpix);
        tex.Apply();
        return tex;
    }

   void replaceblack(int x, int y, int width, int height)
    {
        fillpix[y * width + x].r = 1;

        if (x + 1 < width && x - 1 > 0) {
            if (fillpix[y * width + (x + 1)].r != 1)
                replaceblack(x + 1, y, width, height);

            if (fillpix[y * width + (x - 1)].r != 1)
                replaceblack(x - 1, y, width, height);
        }

        if (y + 1 < height && y - 1 > 0)
        {

            if (fillpix[(y + 1) * width + x].r != 1)
                replaceblack(x, y + 1, width, height);

            if (fillpix[(y - 1) * width + (x - 1)].r != 1)
                replaceblack(x, y - 1, width, height);
        }
    }

    Texture2D meanfilter(Texture2D tex)
    {
        int kernelsize = 11;
        pixels = tex.GetPixels();
        pixels2 = tex.GetPixels();


        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                float value = 0;

                for (int ky = 0; ky < kernelsize; ky++)
                {
                    for (int kx = 0; kx < kernelsize; kx++)
                    {

                        if (x - ((kernelsize - 1) / 2) + kx <= 0 || x - ((kernelsize - 1) / 2) + kx >= tex.width - 1 || y - ((kernelsize - 1) / 2) + ky <= 0 || y - ((kernelsize - 1) / 2) + ky >= tex.height - 1)
                        {

                            value++;

                        }

                        else
                        value +=  pixels[(y - ((kernelsize - 1) / 2) + ky) * tex.width + (x - ((kernelsize - 1) / 2) + kx)].r;
                    }
                }

                value = value / (kernelsize * kernelsize);
                pixels2[y * tex.width + x].r = value;
                pixels2[y * tex.width + x].g = value;
                pixels2[y * tex.width + x].b = value;
            }
        }

        tex.SetPixels(pixels2);
        tex.Apply();
        return tex;
    }
      

    




    void GrassFire(Texture2D tex)
    {
        trees = new int[tex.width, tex.height];
        mountains = new int[tex.width, tex.height];
        houses = new int[tex.width, tex.height];
        water = new int[tex.width, tex.height];
        pixels = tex.GetPixels();


        int  label = 1;
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                if (currentobjectype == ObjectType.tree)
                {
                    
                    if (pixels[y * tex.width + x].r > 0 && trees[x, y] == 0)
                    {
                        
                        grassFireTrees(x, y, label, tex.width);
                        label++;
                    }
                }
                else if (currentobjectype == ObjectType.mountain)
                {
                   
                    if (pixels[y * tex.width + x].r > 0 && mountains[x, y] == 0)
                    {

                        grassFireMountains(x, y, label, tex.width);
                        label++;
                    }
                }
                else if (currentobjectype == ObjectType.water)
                {
                    
                    if (pixels[y * tex.width + x].r > 0 && water[x, y] == 0)
                    {

                        grassFireWater(x, y, label, tex.width);
                        label++;
                    }
                }
                else if (currentobjectype == ObjectType.house)
                {

                    if (pixels[y * tex.width + x].r > 0 && houses[x, y] == 0)
                    {

                        grassFireHouses(x, y, label, tex.width);
                        label++;
                    }
                }
            }
        }

        if (currentobjectype == ObjectType.tree)
        {
            numberOfTrees = label-1;
            Debug.Log((label - 1) + " trees found");
        }
        if (currentobjectype == ObjectType.mountain)
        {
            numberOfMountains = label-1;
            Debug.Log((label - 1) + " mountains found");
        }
        if (currentobjectype == ObjectType.water)
        {
            numberOfWater = label-1;
            Debug.Log((label-1) + " waters found");
        }
        if (currentobjectype == ObjectType.house)
        {
            numberOfHouses = label - 1;
            Debug.Log((label - 1) + " houses found");
        }
    }

    float[,] blobMiddle(int[,] blobs, int number, float[,] map)
    {
        Debug.Log(number);

        int[] xpos = new int[number + 1];
        int[] ypos = new int[number + 1];

        int[] xmax = new int[number + 1];
        int[] xmin = new int[number + 1];
        int[] ymax = new int[number + 1];
        int[] ymin = new int[number + 1];

        for (int i = 0; i < ymax.Length; i++)
        {
            xmax[i] = int.MinValue;
            xmin[i] = int.MaxValue;
            ymax[i] = int.MinValue;
            ymin[i] = int.MaxValue;
        }


        for (int y = 0; y < blobs.GetLength(1); y++)
        {
            for (int x = 0; x < blobs.GetLength(0); x++)
            {
                if (blobs[x, y] != 0)
                {


                    if (xmax[blobs[x, y]] < x)
                        xmax[blobs[x, y]] = x;
                    if (xmin[blobs[x, y]] > x)
                        xmin[blobs[x, y]] = x;

                    if (ymax[blobs[x, y]] < y)
                        ymax[blobs[x, y]] = y;
                    if (ymin[blobs[x, y]] > y)
                        ymin[blobs[x, y]] = y;

                }

            }
        }
        Debug.Log("the map bounds is " + map.GetLength(0) + "," + map.GetLength(1));
        for (int i = 0; i < number+1; i++)
        {
            if (i ==0)
                continue;

            

            ypos[i] = ymin[i];
            xpos[i] = xmin[i];

            if (xpos[i] >128)
                xpos[i] = 128-1;
            if (xpos[i] < 0)
                xpos[i] = 0;

            if (ypos[i] > 128)
                ypos[i] = 128-1;
            if (ypos[i] < 0)
                ypos[i] = 0;


            Debug.Log("blob " + i + " middle is " + xpos[i] + "," + ypos[i]);
            map[xpos[i], ypos[i]] = 1;

           

           

         
        }



        return map;
    }


    void grassFireWater(int x, int y, int label, int width)
    {

        water[x, y] = label;
        if (x + 1 < water.GetLength(0) && x - 1 > 0 && y + 1 < water.GetLength(1) && y - 1 > 0)
        {

            if (water[x - 1, y] != label && pixels[y * width + (x - 1)].r > 0)
                grassFireWater(x - 1, y, label, width);

            if (water[x + 1, y] != label && pixels[y * width + (x + 1)].r > 0)
                grassFireWater(x + 1, y, label, width);

            if (water[x, y - 1] != label && pixels[(y - 1) * width + x].r > 0)
                grassFireWater(x, y - 1, label, width);

            if (water[x, y + 1] != label && pixels[(y + 1) * width + x].r > 0)
                grassFireWater(x, y + 1, label, width);
        }
    }


    void grassFireMountains(int x, int y, int label, int width)
    {

        mountains[x, y] = label;
        if (x + 1 < mountains.GetLength(0) && x - 1 > 0 && y + 1 < mountains.GetLength(1) && y - 1 > 0)
        {

            if (mountains[x - 1, y] != label && pixels[y * width + (x - 1)].r > 0)
                grassFireMountains(x - 1, y, label, width);

            if (mountains[x + 1, y] != label && pixels[y * width + (x + 1)].r > 0)
                grassFireMountains(x + 1, y, label, width);

            if (mountains[x, y - 1] != label && pixels[(y - 1) * width + x].r > 0)
                grassFireMountains(x, y - 1, label, width);

            if (mountains[x, y + 1] != label && pixels[(y + 1) * width + x].r > 0)
                grassFireMountains(x, y + 1, label, width);
        }
    }

    void grassFireTrees(int x, int y, int label, int width)
    {

        trees[x, y] = label;
        if (x + 1 < trees.GetLength(0) && x - 1 > 0 && y + 1 < trees.GetLength(1) && y - 1 > 0)
        {

            if (trees[x - 1, y] != label && pixels[y * width + (x - 1)].r > 0)
                grassFireTrees(x - 1, y, label, width);

            if (trees[x + 1, y] != label && pixels[y * width + (x + 1)].r > 0)
                grassFireTrees(x + 1, y, label, width);

            if (trees[x, y - 1] != label && pixels[(y - 1) * width + x].r > 0)
                grassFireTrees(x, y - 1, label, width);

            if (trees[x, y + 1] != label && pixels[(y + 1) * width + x].r > 0)
                grassFireTrees(x, y + 1, label, width);
        }
    }

    void grassFireHouses(int x, int y, int label, int width)
    {

        houses[x, y] = label;
        if (x + 1 < trees.GetLength(0) && x - 1 > 0 && y + 1 < trees.GetLength(1) && y - 1 > 0)
        {

            if (houses[x - 1, y] != label && pixels[y * width + (x - 1)].r > 0)
                grassFireHouses(x - 1, y, label, width);

            if (houses[x + 1, y] != label && pixels[y * width + (x + 1)].r > 0)
                grassFireHouses(x + 1, y, label, width);

            if (houses[x, y - 1] != label && pixels[(y - 1) * width + x].r > 0)
                grassFireHouses(x, y - 1, label, width);

            if (houses[x, y + 1] != label && pixels[(y + 1) * width + x].r > 0)
                grassFireHouses(x, y + 1, label, width);
        }
    }



    Texture2D BinaryMedianFilter(Texture2D tex)
    {
        Color[] pixs = tex.GetPixels();
        Color[] pixs2 = tex.GetPixels();
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                int sum =0;

                for (int ky = 0; ky < 3; ky++)
                {
                    for (int kx = 0; kx < 3; kx++)
                    {
                        if(x-1+kx <0|| x - 1 + kx > tex.width-1 || y - 1 + ky < 0 || y - 1 + ky > tex.height - 1)
                        {
                            
                            
                        }
                        else if (pixs[(y-1+ky) * tex.width + (x- 1+kx)].r > 0)
                            sum++;
                    }
                }
                if (sum > 4)
                    pixs2[y * tex.width + x] = Color.white;
                else
                    pixs2[y * tex.width + x] = Color.black;

            }
        }
        tex.SetPixels(pixs2);
        tex.Apply();

        return tex;
    }


     Color[] getCurrentPixs()
    {
        Texture2D tex =(Texture2D) rendere.sharedMaterial.mainTexture;
        
        return tex.GetPixels();
    }

}
