using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Landing : MonoBehaviour
{
    bool canLand = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
            canLand = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
            canLand = false;
    }

    private void Update()
    {
        if (canLand && Input.GetKeyDown(KeyCode.F))
            SceneManager.LoadScene("PlanetScene");
    }
}
