using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Casillas_Contador : MonoBehaviour
{
    public TextMeshProUGUI cuadroTexto;
    public GameObject objetoActivar;
    public int limiteInferior = 0;
    public int limiteSuperior = 100;

    // Se dispara con el nuevo valor cada vez que la casilla cambia.
    // CasillaEfecto se suscribe a esto para aplicar avances/retrocesos automaticos.
    public UnityEvent<int> onValorCambiado;

    // M�todo para aumentar el valor en el cuadro de texto
    public void AumentarValor()
    {
        int valorActual = int.Parse(cuadroTexto.text);
        valorActual++;

        // Verificar si el valor no excede el l�mite superior
        if (valorActual <= limiteSuperior)
        {
            cuadroTexto.text = valorActual.ToString();
            onValorCambiado.Invoke(valorActual);

            // Verificar si el valor lleg� al l�mite superior y activar el GameObject
            if (valorActual == limiteSuperior)
            {
                ActivarGameObject();
            }
        }
    }

    // M�todo para disminuir el valor en el cuadro de texto
    public void DisminuirValor()
    {
        int valorActual = int.Parse(cuadroTexto.text);
        valorActual--;

        // Verificar si el valor no es menor que el l�mite inferior
        if (valorActual >= limiteInferior)
        {
            cuadroTexto.text = valorActual.ToString();
            onValorCambiado.Invoke(valorActual);
        }
    }

    // M�todo para activar el GameObject
    private void ActivarGameObject()
    {
        if (objetoActivar != null)
        {
            objetoActivar.SetActive(true);
        }
    }
}

