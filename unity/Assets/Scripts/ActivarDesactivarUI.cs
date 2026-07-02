using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivarDesactivarUI : MonoBehaviour
{
    public GameObject personaje;
    public GameObject configuracion;
    public GameObject Otros;
    // Start is called before the first frame update
    public void Activar ()
    {
        personaje.SetActive (true);
        configuracion.SetActive (true);
        Otros.SetActive (true);
    }

    public void Desactivar ()
    {
        personaje.SetActive (false);
        configuracion.SetActive (false);
        Otros.SetActive (false);
    }
}
