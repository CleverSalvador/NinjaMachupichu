using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactuable : MonoBehaviour
{
    private bool puedeInteractuar;
    private BoxCollider2D bc;
    private SpriteRenderer sp;
    private GameObject indicarInteractuable;
    private Animator anim;

    //Palanca
    public UnityEvent evento;
    
    public GameObject[] objetos;

    public bool esCofre;
    public bool esPalanca;
    public bool palancaAccionada;
    public bool esCheckPoint;


    private void Awake() 
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if(transform.GetChild(0) != null){
            indicarInteractuable = transform.GetChild(0).gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player"))
        {
            puedeInteractuar = true;
            indicarInteractuable.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.CompareTag("Player"))
        {
            puedeInteractuar = false;
            indicarInteractuable.SetActive(false);
        }
    }

    private void Cofre()
    {
        if(esCofre)
        {
            Instantiate(objetos[Random.Range(0, objetos.Length)], transform.position, Quaternion.identity);
        }
        anim.SetBool("abrir", true);
        bc.enabled = false;
    }

    private void Update()
    {
        if(puedeInteractuar && Input.GetKeyDown(KeyCode.C))
        {
            Cofre();
            Palanca();
            CheckPoint();
        }
    }

    /*Metodo en caso sea la Palanca*/
    private void Palanca()
    {
        if(esPalanca && !palancaAccionada)
        {
            anim.SetBool("activar", true);
            palancaAccionada = true;
            evento.Invoke();
            indicarInteractuable.SetActive(false);
            bc.enabled = false;
            this.enabled = false;
        }
        
    }
    private void CheckPoint()
    {
        if(esCheckPoint)
        {
            evento.Invoke();
        }
    }
}
