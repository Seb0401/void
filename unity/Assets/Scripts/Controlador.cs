using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controlador : MonoBehaviour
{
    public string nomEscena;
    public string sigEscena;
    public GameObject panelGanar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CargarEscena()
    {
        SceneManager.LoadScene(nomEscena);
    }

    public void FinEscena()
    {
        panelGanar.SetActive(true);
        Time.timeScale = 0;
    }

    public void IrSiguienteEscena ()
    {
        SceneManager.LoadScene(sigEscena);
        Time.timeScale = 1;
    }
}