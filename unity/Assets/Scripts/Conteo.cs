using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Conteo : MonoBehaviour
{
    public int NroMax;
    public int NroActual;
    public TextMeshProUGUI textoVida;
    public Slider barra;
    public GameObject Button;
    public ConteoSinBarra conteoSinBarra;


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
            conteoSinBarra.NroActual -=cantidad;
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
