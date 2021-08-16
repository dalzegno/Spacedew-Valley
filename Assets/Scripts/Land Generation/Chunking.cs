using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunking : MonoBehaviour
{

    float smoothness = 16; //Terrain smoothness
    float seed = 1;

    float modifier = 0.2f; // The size of the caves



    int worldHeight;
    int worldWidth;
    int negativeWorldHeight = 0;
    int negativeWorldWidth = 0;

    int chunkWidth = 16;
    int chunkHeight = 16;

    [SerializeField] Tilemap groundTilemap; //The tilemap that all the groundTiles resides in
    [SerializeField] TileBase groundTile; //Will later be changed to an array of different types of grounds

    Transform player;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void Start()
    {
        int[,] map = GenerateChunkArray(false, 48, 48);
        //map = GenerateTerrain(map, 48, 48);
        RenderChunk(map, worldWidth, worldHeight, 48, 48, chunkWidth, chunkHeight, false);
        worldHeight = 48;
        worldWidth = 48;
    }

    private void LateUpdate()
    {
        switch (ChunkExists())
        {
            case 0:
                break;
            case 1:
                {
                    Debug.Log("loading 1");
                    int[,] map = GenerateChunkArray(false, chunkWidth, chunkHeight);
                    //map = GenerateTerrain(map, chunkWidth, chunkHeight);
                    worldWidth += chunkWidth;
                    RenderChunk(map, 1, 1, chunkWidth, chunkHeight, worldWidth, worldHeight, false);
                    break;
                }
            case 2:
                {
                    Debug.Log("loading 2");

                    int[,] map = GenerateChunkArray(false, chunkWidth, chunkHeight);
                    //map = GenerateTerrain(map, chunkWidth, chunkHeight);
                    int offsetY = worldHeight;
                    worldHeight += chunkHeight;
                    RenderChunk(map, 0, offsetY, chunkWidth, chunkHeight, worldWidth, worldHeight, false);
                    break;
                }
            case 3:
                {
                    Debug.Log("loading 3");

                    int[,] map = GenerateChunkArray(false, chunkWidth, chunkHeight);
                    //map = GenerateTerrain(map, chunkWidth, chunkHeight);
                    int offsetX = negativeWorldWidth;
                    negativeWorldWidth += chunkWidth;
                    RenderChunk(map, offsetX, 0, chunkWidth, chunkHeight, negativeWorldWidth, negativeWorldHeight, true);
                    break;
                }
            case 4:
                {
                    Debug.Log("loading 4");

                    int[,] map = GenerateChunkArray(false, chunkWidth, chunkHeight);
                    //map = GenerateTerrain(map, chunkWidth, chunkHeight);
                    int offsetY = negativeWorldHeight;
                    negativeWorldHeight += chunkHeight;
                    RenderChunk(map, 0, offsetY, chunkWidth, chunkHeight, negativeWorldWidth, negativeWorldHeight, true);
                    break;
                }
        }
    }

    int[,] GenerateChunkArray(bool empty, int width, int height)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
               
                    map[x, y] = (empty) ? 0 : 1;
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
                    int caveValue = Mathf.RoundToInt(Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));
                    map[x, y] = (caveValue == 1) ? 2 : 1;
                
            }
        }
        return map;
    }

    void RenderChunk(int[,] map, int offsetX, int offsetY, int width, int height, int worldWidth, int worldHeight, bool negative)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int renderX = x + (offsetX * worldWidth);
                int renderY = y + (offsetY * worldHeight);
                if (negative)
                {
                    renderX *= -1;
                    renderY *= -1;
                }
                switch (map[x, y])
                {
                    case 1:
                        groundTilemap.SetTile(new Vector3Int(renderX, renderY, 0), groundTile);
                        break;
                }
            }
        }
    }

    int ChunkExists()
    {
        if(player.position.x > worldWidth - chunkWidth)
            return 1;
        //else if (player.position.y > worldHeight - chunkHeight)
        //    return 2;
        //else if (player.position.x > negativeWorldWidth + chunkWidth)
        //    return 3;
        //else if (player.position.y > negativeWorldHeight + chunkHeight)
        //    return 4;
        else
            return 0;
    }

}


