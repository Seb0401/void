using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AtaquePredeterminado : MonoBehaviour
{
    public Button boton;
    public GameObject objetoX;
    public GameObject objetoY;

    void Start()
    {
        // Asignar la función CopiarImagenObjeto al evento de clic del botón
        boton.onClick.AddListener(CopiarImagenObjeto);
    }

    void CopiarImagenObjeto()
    {
        // Verificar si los objetos X e Y y el botón están asignados
        if (objetoX != null && objetoY != null && boton != null)
        {
            // Obtener los componentes Image de los objetos X e Y
            Image imagenObjetoX = objetoX.GetComponent<Image>();
            Image imagenObjetoY = objetoY.GetComponent<Image>();

            // Verificar si ambos objetos tienen componentes Image
            if (imagenObjetoX != null && imagenObjetoY != null)
            {
                // Copiar la sprite y el color de Y a X
                imagenObjetoX.sprite = imagenObjetoY.sprite;
                imagenObjetoX.color = imagenObjetoY.color;
            }
            else
            {
                Debug.LogWarning("Ambos objetos deben tener componentes Image.");
            }
        }
        else
        {
            Debug.LogWarning("Objeto X, objeto Y o botón no asignados en el Inspector de Unity.");
        }
    }
}