using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Habilidades : MonoBehaviour
{
    public int NroMax;
    public int NroActual;
    public TextMeshProUGUI RondasFaltantes;
    public UnityEngine.UI.Button boton_accion;



    void Start()
    {
        RondasFaltantes.text = " ";
    }

    public void PasarRonda(int cantidad)
    {
        if (NroActual != 1)
        {
            NroActual -= cantidad;
            boton_accion.interactable = false;
            ActualizarTexto();
        }
        else
        {
            RondasFaltantes.text = " ";
            boton_accion.interactable = true;
        }
    }

    public void Accion()
    {
        NroActual = NroMax;
        boton_accion.interactable = false;
        ActualizarTexto();
    }

    public void ActualizarTexto()
    {
        RondasFaltantes.text = NroActual + "";
    }
}