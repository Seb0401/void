using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Conteo1 : MonoBehaviour
{
    public int NroMax;
    public int NroActual;
    public TextMeshProUGUI textoVida;
    public Slider barra;
    public GameObject Button;
    public GameObject accion;


    // Start is called before the first frame update
    void Start()
    {
        textoVida.text = NroActual + "/" + NroMax;
        barra.maxValue = NroMax;
        barra.value = NroActual;

    }

    public void Aumentar(int cantidad)
    {
        if (NroActual != NroMax)
        {
            NroActual += cantidad;
            barra.value += cantidad;
            ActualizarTexto();
        }
        else 
        {
            Button.SetActive(false);
            accion.SetActive(true);
            ActualizarTexto(); 
        }
    }

    public void Accion()
    {
        NroActual = 0;
        barra.value = 0;
        Button.SetActive(true);
        accion.SetActive(false);
        ActualizarTexto();
    }

    public void ActualizarTexto()
    {
        textoVida.text = NroActual + "/" + NroMax;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
