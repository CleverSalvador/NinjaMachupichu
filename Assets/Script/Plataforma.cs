using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataforma : MonoBehaviour
{
    private bool aplicarFuerza;
    private bool detectaJugador;
    private PlayerController player;
    public bool daSalto;
    public BoxCollider2D plataformaCollider;
    public BoxCollider2D plataformaTrigger;

    private void Awake() {
        player =  GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    
    private void OnCollisionEnter2D(Collision2D colision) {
        if(colision.gameObject.CompareTag("Player"))
        {
            detectaJugador = true;
            if(daSalto)
            {
                aplicarFuerza = true;
            }
        }
    }
    private void FixedUpdate()
    {
        if(aplicarFuerza)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().AddForce(Vector2.up*25, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }
    private void OnCollisionExit2D(Collision2D colision)
    {
        if(colision.gameObject.CompareTag("Player"))
        {
            detectaJugador = false;
        }
    }
    
    // Update is called once per frame
    private void Update()
    {
        if(daSalto)
        {
            if(player.transform.position.y - 0.9f > transform.position.y)
            {
                plataformaCollider.isTrigger = false;
            }else 
            {
                plataformaCollider.isTrigger = true;
            }
        }
    }
    /*Plataforma Jump Down*/
    private void Start()
    {
        if(!daSalto)
        {
            Physics2D.IgnoreCollision(plataformaCollider,plataformaTrigger,true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(!daSalto)
            {
                Physics2D.IgnoreCollision(plataformaCollider,player.GetComponent<CapsuleCollider2D>(),true);
            }
        }
    }

     private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(!daSalto)
            {
                Physics2D.IgnoreCollision(plataformaCollider,player.GetComponent<CapsuleCollider2D>(),false);
            }
        }
    }
}
