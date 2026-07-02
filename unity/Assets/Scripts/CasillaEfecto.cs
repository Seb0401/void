using System.Collections.Generic;
using UnityEngine;

// Aplica efectos de tablero tipo "oca" (avanzar/retroceder casillas) sobre un
// Casillas_Contador. Se suscribe a Casillas_Contador.onValorCambiado y, si la
// casilla nueva tiene un efecto configurado, mueve el contador automaticamente
// esa cantidad de pasos (positivo = avanzar, negativo = retroceder).
public class CasillaEfecto : MonoBehaviour
{
    [System.Serializable]
    public class Efecto
    {
        public int casilla;
        public int desplazamiento;
    }

    public Casillas_Contador contador;
    public List<Efecto> efectos = new List<Efecto>();

    private bool aplicandoEfecto;

    private void OnEnable()
    {
        if (contador != null)
        {
            contador.onValorCambiado.AddListener(RevisarEfecto);
        }
    }

    private void OnDisable()
    {
        if (contador != null)
        {
            contador.onValorCambiado.RemoveListener(RevisarEfecto);
        }
    }

    private void RevisarEfecto(int casillaActual)
    {
        // Evita que el desplazamiento generado por el propio efecto
        // dispare RevisarEfecto de nuevo en cadena.
        if (aplicandoEfecto)
        {
            return;
        }

        foreach (Efecto efecto in efectos)
        {
            if (efecto.casilla == casillaActual && efecto.desplazamiento != 0)
            {
                aplicandoEfecto = true;
                int pasos = Mathf.Abs(efecto.desplazamiento);
                for (int i = 0; i < pasos; i++)
                {
                    if (efecto.desplazamiento > 0)
                    {
                        contador.AumentarValor();
                    }
                    else
                    {
                        contador.DisminuirValor();
                    }
                }
                aplicandoEfecto = false;
                break;
            }
        }
    }
}
