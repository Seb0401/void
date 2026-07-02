using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbrirCerrar : MonoBehaviour
{
    public GameObject pantalla;
    public GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BtnAbrir()
    {
        pantalla.SetActive(true);
        canvas.SetActive(true);
    }

    public void BtnCerrar()
    {
        pantalla.SetActive(false);
        canvas.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
