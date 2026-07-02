using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Conteo2 : MonoBehaviour
{
    public int NroMax;
    public int NroActual;
    public TextMeshProUGUI textoVida;
    public Slider barra;


    // Start is called before the first frame update
    void Start()
    {
        textoVida.text = NroActual + "/" + NroMax;
        barra.maxValue = NroMax;
        barra.value = NroActual;

    }

    public void Aumentar(int cantidad)
    {
        if (NroActual == NroMax)
        {
            ActualizarTexto();
        }
        else
        {
            NroActual += cantidad;
            barra.value += cantidad;
            ActualizarTexto();
        }
    }

    public void Disminuir(int cantidad)
    {
        if (NroActual == 0)
        {
            ActualizarTexto();
        }
        else
        {
            NroActual -= cantidad;
            barra.value -= cantidad;
            ActualizarTexto();
        }
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

