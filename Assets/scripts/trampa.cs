using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trampa : MonoBehaviour
{
    float timer = 3f;
    dificultadAdaptable dl;

    void Start()
    {
        dl = GameObject.Find("dificultad").GetComponent<dificultadAdaptable>();
    }

    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 4f;
            GetComponent<Animator>().SetTrigger("activarTrampa");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            other.GetComponent<statsJugador>().recibirDano(Mathf.RoundToInt(dl.nivelDificultad - 1));
        }
    }
}
