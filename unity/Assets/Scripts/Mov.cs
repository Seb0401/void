using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mov : MonoBehaviour
{
    public float vel;
    public Rigidbody2D rb;
    public Vector2 dir;
    public Controlador controlador;

    void Start()
    {
        vel = 4;
    }

    // Update is called once per frame
    void Update()
    {
        dir.x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dir.x * vel, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Meta")
        {
            controlador.CargarEscena();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MetaFinal")
        {
            controlador.FinEscena();
        }
    }
}
