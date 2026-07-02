using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConteoSinBarra : MonoBehaviour
{
    public int NroMax;
    public int NroActual;
    public TextMeshProUGUI textoVida;


    // Start is called before the first frame update
    void Start()
    {
        textoVida.text = NroActual + "/" + NroMax;
    }

    public void Aumentar(int cantidad)
    {
        if (NroActual != NroMax)
        {
            NroActual += cantidad;
            ActualizarTexto();
        }
        else
        {
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