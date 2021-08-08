using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralLand : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] int width; //Width of the chunk being loaded
    [SerializeField] int height; //Height of the chunk being loaded

    [Header("Generation Modifiers")]
    [SerializeField] float smoothness; //Terrain smoothness
    [SerializeField] float seed; //Generating seed

    [Header("Cave Gen")]
    [Range(0,1)]
    [SerializeField] float modifier; // The size of the caves
    [SerializeField] int caveHeight; // How far up you can find caves on the map

    [Header("Tiles")]
    [SerializeField] TileBase groundTile; //Will later be changed to an array of different types of grounds
    [SerializeField] TileBase stoneTile; //Will later be changed to an array of different types of grounds
    [SerializeField] TileBase caveTile; //Will later be changed to an array of different types of cavetiles
    [SerializeField] TileBase[] oreTiles; //Will later be changed to an array of different types of cavetiles
    [Header("Tilemaps")]
    [SerializeField] Tilemap groundTilemap; //The tilemap that all the groundTiles resides in
    [SerializeField] Tilemap caveTilemap; //The tilemap that all the caveTiles resides in
    [SerializeField] Tilemap oreTilemap; //The tilemap that all the caveTiles resides in
    int[,] map; //The map used for the generating

    [Header("Spawning")]
    [SerializeField] GameObject playerSpawn;

    private void Start()
    {
        //seed = Random.Range(0f, 1000f);
        map = GenerateArray(width, height, true);
        map = GenerateTerrain(map);
        map = GenerateOres(map);
        RenderMap(map);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Generate();
        }
    }
    void Generate()
    {
        groundTilemap.ClearAllTiles();
        caveTilemap.ClearAllTiles();
        oreTilemap.ClearAllTiles();
        map = GenerateArray(width, height, true);
        map = GenerateTerrain(map);
        map = GenerateOres(map);
        RenderMap(map);
    }

    public int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (y <= height - 10)
                {
                    map[x, y] = (empty) ? 0 : 2;
                } 
                else
                    map[x, y] = (empty) ? 0 : 1;
            }
        }
        return map;
    }



    public int[,] GenerateTerrain(int[,] map)
    {
        int perlinHeight;
        for (int x = 0; x < width; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height / 2);
            perlinHeight += height / 2;


            for (int y = 0; y < perlinHeight; y++)
            {
                if (y > caveHeight)
                {
                    map[x, y] = 1;
                }
                else
                {
                    int caveValue = Mathf.RoundToInt(Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));
                    map[x, y] = (caveValue == 1) ? 3 : 2;
                }
            }
        }
        return map;
    }

    public int[,] GenerateOres(int [,] map)
    {
        for (int x = 0; x < width; x++)
        {


            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 2)
                {
                    int percentage = Random.Range(0, 100);
                    if (percentage <= 5)
                    {
                        map[x, y] = 4;
                    }
                    else if (percentage <= 15)
                    {
                        map[x, y] = 5;
                    }
                    else if (percentage <= 35)
                    {
                        map[x, y] = 6;
                    }
                }
            }
        }
        return map;
    }

    public void RenderMap(int[,] map)
    {
        bool playerSpawned = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (!playerSpawned && x == width / 2 && map[x, y] == 0) //Checks if it should spawn player
                {
                    playerSpawn.transform.position = groundTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    playerSpawned = true;
                }
                switch (map[x,y])
                {
                    case 1:
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                        break;

                    case 2:
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), stoneTile);
                        break;

                    case 3:
                        caveTilemap.SetTile(new Vector3Int(x, y, 0), caveTile);
                        break;
                    case 4:
                        oreTilemap.SetTile(new Vector3Int(x, y, 0), oreTiles[0]);
                        break;
                    case 5:
                        oreTilemap.SetTile(new Vector3Int(x, y, 0), oreTiles[1]);
                        break;
                    case 6:
                        oreTilemap.SetTile(new Vector3Int(x, y, 0), oreTiles[2]);
                        break;
                }            
                    
            }
        }
    }
}
