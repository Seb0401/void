using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Casillas_Contador : MonoBehaviour
{
    public TextMeshProUGUI cuadroTexto;
    public GameObject objetoActivar;
    public int limiteInferior = 0;
    public int limiteSuperior = 100;

    // Método para aumentar el valor en el cuadro de texto
    public void AumentarValor()
    {
        int valorActual = int.Parse(cuadroTexto.text);
        valorActual++;

        // Verificar si el valor no excede el límite superior
        if (valorActual <= limiteSuperior)
        {
            cuadroTexto.text = valorActual.ToString();

            // Verificar si el valor llegó al límite superior y activar el GameObject
            if (valorActual == limiteSuperior)
            {
                ActivarGameObject();
            }
        }
    }

    // Método para disminuir el valor en el cuadro de texto
    public void DisminuirValor()
    {
        int valorActual = int.Parse(cuadroTexto.text);
        valorActual--;

        // Verificar si el valor no es menor que el límite inferior
        if (valorActual >= limiteInferior)
        {
            cuadroTexto.text = valorActual.ToString();
        }
    }

    // Método para activar el GameObject
    private void ActivarGameObject()
    {
        if (objetoActivar != null)
        {
            objetoActivar.SetActive(true);
        }
    }
}

