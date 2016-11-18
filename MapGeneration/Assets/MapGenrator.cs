using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenrator : MonoBehaviour {

    public enum Mode { Heightmap,Colormap ,Mesh}
    public Mode mode;

    public Transform meshTransform;
    MeshData meshData;

    public GameObject tree;
    public GameObject house;
    List<GameObject> trees = new List<GameObject>();
    List<GameObject> houses = new List<GameObject>();

    public bool addMountains;
    public bool addWater;


    private bool addtrees;
    private bool addhouses;

    public bool Addtrees
    {
        get { return addtrees; }
        set
        {
            if (value == false)
                destroyTrees();
            addtrees = value;

        }
    }
    public bool Addhouses
    {
        get { return addhouses; }
        set
        {
            if (value == false)
                destroyHouses();
            addhouses = value;

        }
    }

    public int mapWidth, mapHeight;
    public float noiseScale;

    public int iterations;
    public float iterationItensity;
    public float iterationSize;

    public Color[] colors;
    public float[] height;

    public float heightMultiplier;
    //lets the user create a curve that can be multiplied to the heigh making mountains taller and grass flatter
    public AnimationCurve heightCurve;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;




    // Use this for initialization
    void Start () {
        DrawingConfig conf =FindObjectOfType<DrawingConfig>();
        conf.Reset();
        conf.NormalizeRGB();
        conf.IsolateMountains();
        conf.NormalizeRGB();
        conf.IsolateWater();
        conf.NormalizeRGB();
        conf.isolateHouses();
        conf.NormalizeRGB();
        conf.findTrees();
        mode = Mode.Mesh;
        Addtrees = false;
        Addhouses = false;
        Addtrees = true;
        Addhouses = true;
        addMountains = true;
        addWater = true;
        GenerateMap();

	}
	

    public void GenerateMap()
    {
        //Debug.Log(Mathf.InverseLerp(0.1, 0.2, currentheight));

        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,seed, noiseScale, iterations,iterationItensity, iterationSize,offset);
        if (addMountains && DrawingConfig.mountainsHeight !=null)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                for (int x = 0; x < noiseMap.GetLength(0); x++)
                {
                    noiseMap[x,y] += (DrawingConfig.mountainsHeight[x, y])/3;
                    noiseMap[x, y] = Mathf.InverseLerp(0, 1, noiseMap[x, y]);
                }
            }
          
            
        }
        if (addWater && DrawingConfig.waterHeight != null)
        {
            for (int y = 0; y < noiseMap.GetLength(1); y++)
            {
                for (int x = 0; x < noiseMap.GetLength(0); x++)
                {
                    noiseMap[x, y] -= (DrawingConfig.waterHeight[x, y]) / 2;
                    noiseMap[x, y] = Mathf.InverseLerp(0, 1, noiseMap[x, y]);
                }
            }


        }


        Color[] colorMap = new Color[mapHeight * mapWidth];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentheight = noiseMap[x, y];

                for (int i = 1; i < colors.Length-1; i++)
                {

                    if (currentheight <= height[i]) {
                        
                        colorMap[y * mapWidth + x] = Color.Lerp(colors[i], colors[i+1], Mathf.InverseLerp(height[i-1],height[i],currentheight));
                        break;
                            }
                }

                  

            }
        }
        

        ShowCurrentMap display = FindObjectOfType<ShowCurrentMap>();

        if (mode == Mode.Heightmap)
            display.DrawTexture(TextureGenerator.HeightMapFromFloatArray(noiseMap));
        else if (mode == Mode.Colormap)
            display.DrawTexture(TextureGenerator.TextureFromColorArray(mapWidth, mapHeight, colorMap));
        else if (mode == Mode.Mesh)
        {
            meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightCurve);

            if (addhouses)
                AddHouses(noiseMap);



            display.DrawMesh(meshData.CreateMesh(), TextureGenerator.TextureFromColorArray(mapWidth, mapHeight, colorMap));
            if (addtrees)
                AddTrees(noiseMap);


        }
    }
    


    void destroyTrees()
    {
        foreach (GameObject t in trees)
        {
            DestroyImmediate(t);
        }

    }
    void destroyHouses()
    {
        Debug.Log("destroy houses");
        foreach (GameObject h in houses)
        {
            DestroyImmediate(h);
        }

    }


    public void AddTrees(float[,] noiseMap)
    {
        destroyTrees();

       


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
              
                if (DrawingConfig.treePlacement[x, y] > 0)
                {
                    Vector3 temp = meshData.vertices[y * mapWidth + x];

                    temp.y = (temp.y * 10) -50 ;
                    temp.x = meshTransform.position.x  + temp.x * 10;
                    temp.z = meshTransform.position.y  + temp.z * 10;
                  

        //trees.Add((GameObject)Instantiate(tree, new Vector3(meshTransform.position.x - ((mapWidth / 2) * 10) + x * 10, heightCurve.Evaluate(noiseMap[x, y]) * heightMultiplier, meshTransform.position.y - ((mapHeight / 2) * 10) + y * 10), Quaternion.identity));
        trees.Add((GameObject)Instantiate(tree, temp, Quaternion.identity));

                }

            }
        }
    }
    public void AddHouses(float[,] noiseMap)
    {
        destroyHouses();




        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                if (DrawingConfig.housePlacement[x, y] > 0)
                {
                    Vector3 temp = meshData.vertices[y * mapWidth + x];

                   

                    for (int hx = 0; hx < 10; hx++)
                    {
                        for (int hy = 0; hy < 10; hy++)
                        {

                            if (x + hx-5 > mapWidth - 1 || y + hy > mapHeight - 1|| y + hy < 0 || x + hx - 5 < 0)
                                continue;

                            meshData.vertices[(y+hy) * mapWidth + (x + hx-5)].y = temp.y;
                            

                        }
                    }


                    temp.y = (temp.y * 10) - 50;
                    temp.x = meshTransform.position.x + temp.x * 10;
                    temp.z = meshTransform.position.y + temp.z * 10;

                   
                    houses.Add((GameObject)Instantiate(house, temp, Quaternion.identity));

                }

            }
        }
    }
}
