using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Transform camera;
    Transform player;

    FixedWorldSize GeneratorScript;

    float maxDist;
    float minDist;
    private void Awake()
    {
        GeneratorScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<FixedWorldSize>();
        
        camera = this.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }
    private void Start()
    {
        maxDist = GeneratorScript.WorldWidth - 11.5f;
        minDist = 11.5f;
        
    }
    private void Update()
    {
        if (player.position.x > maxDist || player.position.x < minDist)
        {
            if(player.position.x > maxDist)
            {
                camera.position = new Vector3(maxDist, player.position.y, -1);
            }
            if(player.position.x < minDist)
            {
                camera.position = new Vector3(minDist, player.position.y, -1);
            }
            return;
        }
        camera.position = player.position;
    }
}
