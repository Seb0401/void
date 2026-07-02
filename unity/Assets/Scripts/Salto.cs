using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salto : MonoBehaviour
{
    public Rigidbody2D rb;
    public float fuerzaSalto;
    public DetectorSalt detectorSalt;

    // Start is called before the first frame update
    void Start()
    {
        fuerzaSalto = 350;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) 
            && detectorSalt.puedoSaltar==true)
        {
            rb.AddForce(Vector2.up * fuerzaSalto);
            detectorSalt.puedoSaltar = false;
        }
    }
}
