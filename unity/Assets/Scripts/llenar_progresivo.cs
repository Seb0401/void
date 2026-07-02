using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class llenar_progresivo : MonoBehaviour
{
    public RectTransform barra;
    public int numeroDePartes = 5;
    public float velocidadLlenado = 1f;
    public Color colorInicial = Color.white;
    private int parteActual = 0;

    void Start()
    {
        // Dividir la barra en partes
        DividirBarra();
    }

    void DividirBarra()
    {
        float anchoParte = barra.rect.width / numeroDePartes;

        // Crear sprites para cada parte
        for (int i = 0; i < numeroDePartes; i++)
        {
            GameObject parte = new GameObject("Parte_" + i.ToString());
            Image parteImage = parte.AddComponent<Image>();
            RectTransform rectTransform = parte.GetComponent<RectTransform>();

            rectTransform.SetParent(barra.transform);
            rectTransform.anchorMin = new Vector2(i * anchoParte / barra.rect.width, 0f);
            rectTransform.anchorMax = new Vector2((i + 1) * anchoParte / barra.rect.width, 1f);

            // Establecer el color inicial
            parteImage.color = colorInicial;
        }
    }

    public void LlenarBarra()
    {
        if (parteActual < numeroDePartes)
        {
            // Cambiar el color de la parte actual
            Transform parteTransform = barra.transform.GetChild(parteActual);
            Image parteImage = parteTransform.GetComponent<Image>();
            parteImage.color = Color.green;

            parteActual++;

            // Si la barra está completamente llena, puedes realizar alguna acción aquí
            if (parteActual == numeroDePartes)
            {
                Debug.Log("La barra está completamente llena.");
            }
        }
    }
}
