using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Waypoints : MonoBehaviour
{
    private Vector3 direccion;
    private PlayerController player;
    private CinemachineVirtualCamera cm;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private int indiceActual = 0 ; //posicion del punto
    private bool aplicarFuerza;

    public int vidas = 3;
    public Vector2 posicionCabeza;
    public float velocidadDesplazamiento;
    public List<Transform> puntos = new List<Transform>();
    public GameObject deathEfect;
    [Header("Pltaforma Movil")]
    public bool esperando;
    private bool agitando;
    public float tiempoEspera;
    public float fuerzaImpacto;

    private void Awake() 
        {
             cm = GameObject.FindGameObjectWithTag("VirtualCamara").GetComponent<CinemachineVirtualCamera>();   
             sp = GetComponent<SpriteRenderer>();
             rb = GetComponent<Rigidbody2D>();
             player =  GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                
        }
    
    private void Start()
    {
        if(gameObject.CompareTag("Enemigo"))
        {
            gameObject.name = "Virus";
        }
    }

    private void FixedUpdate()
    {
        MovimientoWaypoints();
        if(gameObject.CompareTag("Enemigo"))
        {
            CambiarEscalaEnemigo();
        }
        if(gameObject.CompareTag("NPC"))
        {
            CambiarEscalaEnemigo();
        }
        if(aplicarFuerza)
        {
            rb.AddForce((transform.position - player.transform.position).normalized*fuerzaImpacto, ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemigo") || gameObject.CompareTag("EnemigoWP") )
        {
            /*Verificamos si los pies del player estan encima de la cabeza del enemigo*/
            if(player.transform.position.y - 0.7f > transform.position.y + posicionCabeza.y)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up*player.fuerzaSalto;//le damos un impulso al jugador
                Instantiate(deathEfect,transform.position,transform.rotation);
                AudioManager.instance.PlaySFX(6);
                Destroy(this.gameObject,0.2f);//se destruye el enemigo, PROBAMDO
            }else{
                player.RecibirDaño(-(player.transform.position-transform.position).normalized);//el jugador es empujado
            }
        }else if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))//comparar el tag para que se dezplace por la plataforma
        {
            if(player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = transform;//mismo movimiento
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))//comparar el tag para que se dezplace por la plataforma
        {
             player.transform.parent = null;//mismo movimiento
           
        }

    }
    /*Este metodo permite cambiar la direccion de movimiento del enemigo en base al atributo escala*/
    private void CambiarEscalaEnemigo()
    {
        if(direccion.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,transform.localScale.z);
        }
        else if(direccion.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y,transform.localScale.z);
        }
    }

    /*Metodo de los Waypoint*/
    private void MovimientoWaypoints()
    {
        direccion = (puntos[indiceActual].position-transform.position).normalized;
        if(!esperando)
        {
            transform.position = (Vector2.MoveTowards(transform.position, puntos[indiceActual].position,velocidadDesplazamiento*Time.deltaTime ));
        }
        
        if(Vector2.Distance(transform.position, puntos[indiceActual].position) <= 0.7f)/*verificamos que la distancia sea menor a 0.7 entre el enemigo y el waypoint*/
        {
            if(!esperando)
            {
                StartCoroutine(Espera());
            }
            
        }
    }
    private IEnumerator Espera()
    {
        esperando = true;
        yield return new WaitForSeconds(tiempoEspera);
        esperando = false;
        indiceActual ++;
        if(indiceActual >= puntos.Count)/*verificar que no sobresalga del numero de waypoints*/
        {
            indiceActual = 0;
        }
    }
    public void RecibirDaño()/*Daño hacia el jugador*/
    {
        if(vidas > 0)
        {
            StartCoroutine(EfectoDaño());
            StartCoroutine(AgitarCamara(0.1f));
            aplicarFuerza = true;
            vidas --;
        }
        else
        {
            StartCoroutine(UltimoAgitarCamara(0.3f));
        }
    }
    private void Morir()
    {
        if(vidas <= 0)
        {
            velocidadDesplazamiento = 0;
            rb.velocity = Vector2.zero;
            Instantiate(deathEfect, transform.position, transform.rotation);
            AudioManager.instance.PlaySFX(6);
            Destroy(this.gameObject,0.2f);
        }
    }
     private IEnumerator AgitarCamara(float tiempo)
    {
        if(!agitando)
        {
            agitando = true;
            CinemachineBasicMultiChannelPerlin c = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            c.m_AmplitudeGain = 5;/*modificamos el valor de la agitacion de la camara*/
            yield return new WaitForSeconds(tiempo);
            c.m_AmplitudeGain = 0;
            //Morir();
            agitando = false;
        }
       
    }
    private IEnumerator UltimoAgitarCamara(float tiempo)
    {
        if(!agitando)
        {
            transform.localScale = Vector3.zero;
            agitando = true;
            CinemachineBasicMultiChannelPerlin c = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            c.m_AmplitudeGain = 5;/*modificamos el valor de la agitacion de la camara*/
            yield return new WaitForSeconds(tiempo);
            c.m_AmplitudeGain = 0;
            Morir();
            agitando = false;
        }
       
    }
     private IEnumerator EfectoDaño()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }

}
