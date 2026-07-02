using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ControladorVida : MonoBehaviour
{
    public int hpMax;
    public int HpActual;
    public TextMeshProUGUI textoVida;
    public GameObject objetoPersonaje;
    public Image imagenBarraHpActual;
    public GameObject contenedorHp;
    public GameObject[] hpUnidades;
    public GameObject[] hpUnidadesActivas;
    public bool invulnerable;
    public float tiempoInvulnerable;
    public SpriteRenderer[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        HpActual = hpMax;
        //ActualizaBarraHp();
        ActualizarContenedorHP();
    }

    public void AumentarVida(int cantidad)
    {
        HpActual += cantidad;
        if (HpActual > hpMax)
        {
            HpActual = hpMax;
        }
        ActualizarTextoVida();
        //ActualizaBarraHp();
        ActualizarContenedorHP();
    }

    public void DisminuirVida(int cantidad)
    {
        if (invulnerable ==false)
        {
            HpActual -= cantidad;
            if (HpActual <= 0)
            {
                HpActual = 0;
                objetoPersonaje.SetActive(false);
            }
            ActualizarTextoVida();
            //ActualizarBarraHP();
            ActualizarContenedorHP();
            StartCoroutine(CorrutinaInvulnerable());
        }
    }


    public void ActualizarTextoVida()
    {
        textoVida.text = HpActual + "/" + hpMax;
    }

    public void ActualizarBarraHp()
    {
        imagenBarraHpActual.fillAmount = (float)HpActual / hpMax;
    }

    public void ActualizarContenedorHP()
    {
        for (int i = 0; i < hpUnidades.Length; i++)
        {
            hpUnidades[i].SetActive(false);
        }

        for (int i = 0;  i < hpMax; i++)
        {
            hpUnidades[i].SetActive(true);
        }
        
        for (int i = 0; i < hpMax; i++)
        {
            hpUnidadesActivas[i].SetActive(false);
        }

        for (int i = 0;i < HpActual; i++)
        {
            hpUnidadesActivas[i].SetActive(true);
        }
    }

    

    public IEnumerator CorrutinaInvulnerable()
    {
        foreach (SpriteRenderer spriteRenderer in sprites)
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 0.5f;
                spriteRenderer.color = color;
            }
        }

        invulnerable = true;
        yield return new WaitForSeconds(tiempoInvulnerable);
        invulnerable = false;

        foreach (SpriteRenderer spriteRenderer in sprites)
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
        }        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
