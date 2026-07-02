using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector_Tema : MonoBehaviour
{
    public GameObject ElemActual;
    public GameObject canvaActual;
    public GameObject Elem1;
    public GameObject canva1;
    public GameObject Elem2;
    public GameObject canva2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void EscogerTema ()
    {
        ElemActual.SetActive(true);
        canvaActual.SetActive(true);
        canva1.SetActive(false);
        canva2.SetActive(false);
        Elem1.SetActive(false);
        Elem2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
