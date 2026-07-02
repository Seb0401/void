using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectorEnemigoPersonaje : MonoBehaviour
{
    public animacionesEnemigo animacionesEnemigo;
    public bool personajeEntro;
    public int tiempoAtaque1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            personajeEntro = true;
            StartCoroutine(CorrutinaAtaque());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            personajeEntro = false;
            StopCoroutine(CorrutinaAtaque());
        }
    }

    public IEnumerator CorrutinaAtaque()
    {
        while (personajeEntro)
        {
            animacionesEnemigo.AnimacionAtaque();
            yield return new WaitForSeconds(tiempoAtaque1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
