using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FixedWorldSize : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] int width; //Width of the chunk being loaded
    [SerializeField] int height; //Height of the chunk being loaded

    [Header("Generation Modifiers")]
    [SerializeField] float smoothness; //Terrain smoothness
    [SerializeField] float seed; //Generating seed

    [Header("Cave Gen")]
    [Range(0, 1)]
    [SerializeField] float modifier; // The size of the caves
    [SerializeField] int caveHeight; // How far up you can find caves on the map

    [Header("Tiles")]
    [SerializeField] TileBase[] groundTiles;
    [SerializeField] TileBase[] stoneTiles;
    [SerializeField] TileBase[] caveTiles;
    [SerializeField] TileBase[] oreTiles;
    [Header("Tilemaps")]
    [SerializeField] Tilemap groundTilemap; //The tilemap that all the groundTiles resides in
    [SerializeField] Tilemap caveTilemap; //The tilemap that all the caveTiles resides in
    [SerializeField] Tilemap oreTilemap; //The tilemap that all the caveTiles resides in
    int[,] map; //The map used for the generating

    Transform player;
    int playerSpawnY; //The players spawn position on the y-axis to prevent spawning inside of terrain

    int chunkWidth = 16; //Since of the chunks being rendered

    int renderedWidth; //How much of the map has been rendered to the right 0 == width/2
    int negativeRenderedWidth; // How much of the map has been rendered to the left 0 == width/2
    

    public int WorldWidth; //public int for the camera to know when the borders been met might move the camera code into this script instead;


    private void Awake()
    {
        WorldWidth = width;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        negativeRenderedWidth = width / 2;
        renderedWidth = width / 2;
        teleportationWidth = width - chunkWidth;

        map = GenerateArray(width, height, true);
        map = GenerateTerrain(map);

        GetPlayerPoints(map, width, height);

        player.position = groundTilemap.GetCellCenterWorld(new Vector3Int(width / 2, playerSpawnY, 0));
    }

    int leftSpawnY;
    int rightSpawnY;
    void GetPlayerPoints(int[,] map, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            int x = 1;

            if (map[x, y] != 0)
            {
                leftSpawnY = y + 1; //Determines the y-level of which the player should spawn to prevent spawning inside of terrain.
            }

            x = width - 1;

            if (map[x, y] != 0)
            {
                rightSpawnY = y + 1; //Determines the y-level of which the player should spawn to prevent spawning inside of terrain.
            }

            x = width / 2;
            if(map[x,y] != 0)
            {
                playerSpawnY = y + 1; //Determines the y-level of which the player should spawn to prevent spawning inside of terrain.
            }


        }
    }

    private void LateUpdate()
    {

        if (player.position.x > width)
        {
           //If the player exits through the right border it moves him back to the begínning of the map
           player.position = groundTilemap.GetCellCenterWorld(new Vector3Int(1, leftSpawnY, 0));
        }
        if (player.position.x < 0)
        {
            //If the player exits through the left border it moves him to the end of the map
            player.position = groundTilemap.GetCellCenterWorld(new Vector3Int(width -1, rightSpawnY, 0));
        }
        switch (ChunkExists())
        {
            case 1:
                {
                    Debug.Log("E");
                    //Render chunks to the right of the map and adds a chunk to the rendered width counter
                    StartCoroutine(RenderMap(map, renderedWidth));
                    renderedWidth += chunkWidth;
                    if(renderedWidth == width)
                        rightBorderChunkSpawned = true;
                    
                    break;

                }
            case 2:
                {
                    //Render chunks to the left of the map and adds a chunk to the negative rendered width counter
                    negativeRenderedWidth -= 16;
                    StartCoroutine(RenderMap(map, negativeRenderedWidth));
                    if(negativeRenderedWidth == 0)
                    {
                        leftBorderChunkSpawned = true;
                    }
                    break;
                }
            case 3:
                {
                    Debug.Log("e");
                    //Renders chunks to the left of the map but only if the player has teleported from the left side border to the right side border
                    StartCoroutine(RenderMap(map, width -chunkWidth));
                    rightBorderChunkSpawned = true;
                    break;
                }
            case 4:
                {
                    Debug.Log("w");
                    //Renders chunks to the left of the map but only if the player has teleported from the left side border to the right side border
                    StartCoroutine(RenderMap(map, 0));
                    leftBorderChunkSpawned = true;
                    break;
                }
            case 5:
                {
                    Debug.Log("hej");
                    teleportationWidth -= chunkWidth;
                    StartCoroutine(RenderMap(map, teleportationWidth));
                    break;
                }
            case 0:

                break;
        }
    }
    bool rightBorderChunkSpawned = false;
    bool leftBorderChunkSpawned = false;
    int teleportationWidth;
    int ChunkExists()
    {
        if (player.position.x > renderedWidth - chunkWidth && renderedWidth + chunkWidth <= width && player.position.x < renderedWidth + 1 && teleportationWidth != renderedWidth)
            return 1;
        else if (player.position.x < negativeRenderedWidth + chunkWidth && negativeRenderedWidth - chunkWidth >= 0)
            return 2;
        else if (player.position.x < chunkWidth && !rightBorderChunkSpawned)
            return 3;
        else if (player.position.x > width - chunkWidth && !leftBorderChunkSpawned)
            return 4;
        else if (player.position.x - chunkWidth > renderedWidth && player.position.x - chunkWidth < teleportationWidth)
            return 5;
        else return 0;
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


    public IEnumerator RenderMap(int[,] map, int xOffset)
    {
        int mapX;
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < height; y++)
            {

                mapX = x + xOffset;
                int xRenderPosition = x + xOffset;


                switch (map[mapX, y])
                {
                    case 1:
                        groundTilemap.SetTile(new Vector3Int(xRenderPosition, y, 0), groundTiles[Random.Range(0, groundTiles.Length)]);
                        break;

                    case 2:
                        groundTilemap.SetTile(new Vector3Int(xRenderPosition, y, 0), stoneTiles[Random.Range(0, stoneTiles.Length)]);
                        break;

                    case 3:
                        caveTilemap.SetTile(new Vector3Int(xRenderPosition, y, 0), caveTiles[Random.Range(0, caveTiles.Length)]);
                        break;
                    case 4:
                        oreTilemap.SetTile(new Vector3Int(xRenderPosition, y, 0), oreTiles[0]);
                        break;
                    case 5:
                        oreTilemap.SetTile(new Vector3Int(xRenderPosition, y, 0), oreTiles[1]);
                        break;
                    case 6:
                        oreTilemap.SetTile(new Vector3Int(xRenderPosition, y, 0), oreTiles[2]);
                        break;
                }

            }
            yield return null;
        }

    }

}
