using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeleccionPersonaje : MonoBehaviour
{
    public Button boton;
    public GameObject objetoConImagen;

    void Start()
    {
        // Asignar la función CambiarImagenBoton al evento de clic del botón
        boton.onClick.AddListener(CambiarImagenBoton);
    }

    void CambiarImagenBoton()
    {
        // Verificar si el objeto con la imagen y el botón están asignados
        if (objetoConImagen != null && boton != null)
        {
            // Obtener la imagen y el color del botón
            Image imagenBoton = boton.GetComponent<Image>();
            Color colorBoton = imagenBoton.color;

            // Obtener el componente Image del objeto y asignarle la imagen y el color del botón
            Image imagenObjeto = objetoConImagen.GetComponent<Image>();
            if (imagenObjeto != null)
            {
                // Conservar tanto la imagen como el color del botón
                imagenObjeto.sprite = imagenBoton.sprite;
                imagenObjeto.color = colorBoton;
            }
            else
            {
                Debug.LogWarning("El objeto no tiene componente Image.");
            }
        }
        else
        {
            Debug.LogWarning("Objeto o botón no asignados en el Inspector de Unity.");
        }
    }
}