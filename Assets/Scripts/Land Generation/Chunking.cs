using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunking : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] int width; //Width of the chunk being loaded
    [SerializeField] int height; //Height of the chunk being loaded
    [SerializeField] int chunkHeight; //Height of the chunk being loaded
    [SerializeField] int chunkWidth; //Height of the chunk being loaded

    [Header("Generation Modifiers")]
    [SerializeField] float smoothness; //Terrain smoothness
    [SerializeField] float seed; //Generating seed

    [Header("Cave Gen")]
    [Range(0, 1)]
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
    [SerializeField] GameObject player;


    [SerializeField] int worldWidth;
    private int worldHeight;

    private Transform playerTransform;
    private void Start()
    {
        playerTransform = player.transform;
        worldHeight = height;
        worldWidth = width;
        map = GenerateArray(width, height, true);
        map = GenerateTerrain(map, width, height);
        //map = GenerateOres(map);
        RenderMapInit(map);
    }

    

    private void LateUpdate()
    {
        if (!ChunkLoaded())
        {
            Debug.Log("loading");
            var offset = ChunkOffset();
            map = GenerateArray(chunkWidth, height, true);
            map = GenerateTerrain(map, chunkWidth, height);
            RenderMap(map, Mathf.RoundToInt(offset.x), Mathf.RoundToInt(offset.y));
        }
    }

    Vector2 ChunkOffset()
    {
        if (playerTransform.position.x > worldWidth - chunkWidth)
        {
            return new Vector2(1, 0);
        }
        else if (playerTransform.position.x > worldWidth - chunkWidth && playerTransform.position.y < worldHeight - chunkHeight)
        {
            return new Vector2(1, -1);
        }
        else if (playerTransform.position.y < worldHeight - chunkHeight)
        {
            return new Vector2(0, -1);
        }
        else return Vector2.zero;
    }
    bool ChunkLoaded()
    {
      if( playerTransform.position.x > worldWidth - chunkWidth || playerTransform.position.y < worldHeight - chunkHeight)
        {
            return false;
        }
        return true;
    }

    public int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y <= height - 10)
                {
                    map[x, y] = (empty) ? 1 : 2;
                }
                else
                    map[x, y] = (empty) ? 1 : 3;
            }
        }
        return map;
    }



    public int[,] GenerateTerrain(int[,] map, int width, int height)
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
                    map[x, y] = 2;
                }
                else
                {
                    int caveValue = Mathf.RoundToInt(Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));
                    map[x, y] = (caveValue == 1) ? 4 : 3;
                }
            }
        }
        return map;
    }

    public int[,] GenerateOres(int[,] map)
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
                        map[x, y] = 5;
                    }
                    else if (percentage <= 15)
                    {
                        map[x, y] = 6;
                    }
                    else if (percentage <= 35)
                    {
                        map[x, y] = 7;
                    }
                }
            }
        }
        return map;
    }


    public void RenderMap(int[,] map, int offsetX, int offsetY)
    {
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int renderX = x + (offsetX * worldWidth);
                int renderY = y + (offsetY * worldHeight);
                switch (map[x, y])
                {
                    case 2:
                        groundTilemap.SetTile(new Vector3Int(renderX, renderY, 0), groundTile);
                        break;

                    case 3:
                        groundTilemap.SetTile(new Vector3Int(renderX, renderY, 0), stoneTile);
                        break;

                    case 4:
                        caveTilemap.SetTile(new Vector3Int(renderX, renderY, 0), caveTile);
                        break;
                    case 5:
                        oreTilemap.SetTile(new Vector3Int(renderX, renderY, 0), oreTiles[0]);
                        break;
                    case 6:
                        oreTilemap.SetTile(new Vector3Int(renderX, renderY, 0), oreTiles[1]);
                        break;
                    case 7:
                        oreTilemap.SetTile(new Vector3Int(renderX, renderY, 0), oreTiles[2]);
                        break;
                }

            }
        }
            worldWidth += chunkWidth;
    }
    public void RenderMapInit(int[,] map)
    {
        bool playerSpawned = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (!playerSpawned && x == width / 2 && map[x, y] == 1) //Checks if it should spawn player
                {
                    playerSpawn.transform.position = groundTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    playerSpawned = true;
                }
                switch (map[x, y])
                {
                    case 2:
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                        break;

                    case 3:
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), stoneTile);
                        break;

                    case 4:
                        caveTilemap.SetTile(new Vector3Int(x, y, 0), caveTile);
                        break;
                    case 5:
                        oreTilemap.SetTile(new Vector3Int(x, y, 0), oreTiles[0]);
                        break;
                    case 6:
                        oreTilemap.SetTile(new Vector3Int(x, y, 0), oreTiles[1]);
                        break;
                    case 7:
                        oreTilemap.SetTile(new Vector3Int(x, y, 0), oreTiles[2]);
                        break;
                }

            }
        }
    }
}


