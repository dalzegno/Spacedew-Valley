using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionChecking : MonoBehaviour
{
    GameObject TravelUI;
    bool canTravel = false;

    private void Awake()
    {
        TravelUI = GameObject.Find("TravelUI");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ship"))
        {
            canTravel = true;
            TravelUI.SetActive(true);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ship"))
        {
            canTravel = false;
            TravelUI.SetActive(false);

        }
    }

    private void Update()
    {
        if(canTravel && Input.GetKeyDown(KeyCode.F))
        {
            SceneManager.LoadScene("ShipScene");
        }
    }
}
