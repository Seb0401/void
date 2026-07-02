using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextoParpadeante : MonoBehaviour
{
    public GameObject Tex;
    // Start is called before the first frame update
    void Start()
    {
        Tex.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime == 2)
        {
            Tex.SetActive(false);
        }
        if (Time.deltaTime == 4)
        {
            Tex.SetActive(true);
        }
    }
}
