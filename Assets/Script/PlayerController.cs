﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direccion;
    private Animator anim;
    private CinemachineVirtualCamera cm;
    private Vector2 direccionMovimiento;
    private Vector2 direccionDaño;
    private GrayCamera gc;
    private SpriteRenderer sp;
    [Header("Estadisticas")]
    public float velocidadDeMovimiento = 10;
    public float fuerzaSalto = 7;
    public float velocidadDash = 20;
    public int vidas = 3;
    public float tiepoInmortalidad;
    [Header("Booleanos")]
    public bool puedeMover = true;
    public bool enSuelo = true;
    public bool puedeDash;
    public bool haciendoDash;
    public bool tocadoPiso;
    public bool HaciendoShake; /*Temblor de la camara*/
    public bool estaAtacando;
    private bool bloqueado;
    public bool esImortal;
    public bool aplicarFuerza;
    [Header("Colision")]
    public float radioColision;
    public Vector2 abajo;
    public LayerMask layerPiso;

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamara").GetComponent<CinemachineVirtualCamera>();
        gc = Camera.main.GetComponent<GrayCamera>();
        sp = GetComponent<SpriteRenderer>();
    }
    public void SetBloqueoTrue()
    {
        bloqueado = true;
    }
    public void Morir()
    {
        if(vidas > 0)
        return;

        this.enabled = false;
    }

    public void RecibirDaño()
    {
        StartCoroutine(ImpactoDaño(Vector2.zero));
    }
    public void RecibirDaño(Vector2 direccionDaño)
    {
        StartCoroutine(ImpactoDaño(direccionDaño));
    }
    private IEnumerator ImpactoDaño(Vector2 direccionDaño)
    {
        if(!esImortal)
        {
            StartCoroutine(Inmortalidad());
            vidas--;
            gc.enabled = true;
            float velocidadAuxiliar = velocidadDeMovimiento;
            this.direccionDaño = direccionDaño;
            aplicarFuerza = true;
            Time.timeScale = 0.4f;
            StartCoroutine(AgitarCamara());
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1;
            gc.enabled = false;
            for(int i = GameManager.instance.vidasUI.transform.childCount -1; i>=0; i--)
            {
                if(GameManager.instance.vidasUI.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    GameManager.instance.vidasUI.transform.GetChild(i).gameObject.SetActive(false);
                    break;
                }
            }
            velocidadDeMovimiento = velocidadAuxiliar;
            Morir();
        }
    }
    private void FixedUpdate()
    {
        if(aplicarFuerza)
        {
            velocidadDeMovimiento = 0;
            rb.velocity = Vector2.zero;
            rb.AddForce(-direccionDaño*10,ForceMode2D.Impulse);
            aplicarFuerza = false;
        }
    }
    public void DarInmortalidad()
    {
        StartCoroutine(Inmortalidad());
    }
    private IEnumerator Inmortalidad()
    {
        esImortal = true;
        float tiempoTranscurrido = 0;
        while(tiempoTranscurrido < tiepoInmortalidad)
        {
            sp.color = new Color(1,1,1,.5f);
            yield return new WaitForSeconds(tiepoInmortalidad/20);
            sp.color = new Color(1,1,1,1);
            yield return new WaitForSeconds(tiepoInmortalidad/20);
            tiempoTranscurrido += tiepoInmortalidad/10;
        }
        esImortal =  false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();
        Agarres();
    }
    private void Atacar(Vector2 direccion)
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(!estaAtacando && !haciendoDash)
            {
                estaAtacando = true;
                anim.SetFloat("ataqueX", direccion.x);
                anim.SetFloat("ataqueY",direccion.y);
                anim.SetBool("atacar",true);
            }
        }
    }
    public void FinalizarAtaque()
    {
        anim.SetBool("atacar",false);
        bloqueado = false;
        estaAtacando = false;
    }
    private Vector2 DireccionAtaque(Vector2 direccionMovimiento, Vector2 direccion)
    {
        if(rb.velocity.x == 0 && direccion.y != 0 )
        {
            return new Vector2(0,direccion.y);
        } return new Vector2(direccionMovimiento.x,direccion.y);
    }
    private IEnumerator AgitarCamara()/*Agitacion de la camara cuando el pj haga el Dash*/
    {
        haciendoDash = true;
        CinemachineBasicMultiChannelPerlin c = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        c.m_AmplitudeGain = 5;/*modificamos el valor de la agitacion de la camara*/
        yield return new WaitForSeconds(0.3f);
        c.m_AmplitudeGain = 0;
        haciendoDash = false;
    }
    private IEnumerator AgitarCamara(float tiempo)/*Agitacion de la camara cuando ataquen al pj*/
    {
        haciendoDash = true;
        CinemachineBasicMultiChannelPerlin c = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        c.m_AmplitudeGain = 5;/*modificamos el valor de la agitacion de la camara*/
        yield return new WaitForSeconds(tiempo);
        c.m_AmplitudeGain = 0;
        haciendoDash = false;
    }
    private void Dash(float x, float y){
        anim.SetBool("dash",true);
        StartCoroutine(AgitarCamara());
        puedeDash=true;
        rb.velocity = Vector2.zero;
        rb.velocity += new Vector2(x,y).normalized*velocidadDash;
        StartCoroutine(PrepararDash());
    }
    private IEnumerator PrepararDash(){
        StartCoroutine(DashSuelo());
        rb.gravityScale = 0;
        haciendoDash = true;
        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = 1;
        haciendoDash = false;
        FinalizarDash();
    }
    private IEnumerator DashSuelo(){
        yield return new WaitForSeconds(0.15f);
        if(enSuelo)
            puedeDash = false;
    }
    public void FinalizarDash(){
        anim.SetBool("dash",false);
    }
    private void TocarPiso(){
        puedeDash = false;
        haciendoDash = false;
        anim.SetBool("saltar",false);
    }
    private void Movimiento(){
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        direccion = new Vector2(x,y);
        Vector2 direccionRaw = new Vector2(xRaw,yRaw); 
        Caminar();
        Atacar(DireccionAtaque(direccionMovimiento, direccionRaw));
        MejorarSalto();
        if(Input.GetKeyDown(KeyCode.Space)){
            if(enSuelo){
                anim.SetBool("saltar",true);
                Salto();
            }
        }
        if(Input.GetKeyDown(KeyCode.Mouse1) && !haciendoDash && !puedeDash){
            if(xRaw != 0 || yRaw != 0 )
            Dash(xRaw,yRaw);
        }
        if(enSuelo && !tocadoPiso){
            TocarPiso();
            tocadoPiso = true;
        }
        if(!enSuelo && tocadoPiso){
            tocadoPiso = false;
        }
            float velocidad;
            if(rb.velocity.y > 0)
                velocidad = 1;
            else
                velocidad = -1;
        if(!enSuelo){
            
            anim.SetFloat("velocidadVertical",velocidad);
        }
        else{
            if(velocidad == -1)
            FinalizarSalto();
        }
    }
    public void FinalizarSalto(){
        anim.SetBool("saltar",false);
    }
    private void MejorarSalto(){
        if(rb.velocity.y < 0){
            rb.velocity += Vector2.up*Physics2D.gravity.y*(2.5f-1)*Time.deltaTime;
        }else if(rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)){
            rb.velocity += Vector2.up*Physics2D.gravity.y*(2.0f-1)*Time.deltaTime;
        }
    }
    private void Agarres(){
        enSuelo = Physics2D.OverlapCircle((Vector2)transform.position + abajo, radioColision,
                    layerPiso);
    }
    private void Salto(){
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up*fuerzaSalto;
    }

    private void Caminar(){
        if(puedeMover && !haciendoDash && !estaAtacando)
        {
            rb.velocity = new Vector2(direccion.x*velocidadDeMovimiento, rb.velocity.y);
            if(direccion != Vector2.zero)
            {
                if(!enSuelo){
                    anim.SetBool("saltar",true);
                }else{
                    anim.SetBool("caminar",true);
                }
                if(direccion.x < 0 && transform.localScale.x > 0)
                {
                    direccionMovimiento = DireccionAtaque(Vector2.left,direccion);
                    transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                }else if(direccion.x > 0 && transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3 (Mathf.Abs(-transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    direccionMovimiento = DireccionAtaque(Vector2.right,direccion);
                 }
            
             }
             else{
                if(direccion.y > 0 && direccion.x == 0 )
                {
                    direccionMovimiento = DireccionAtaque(direccion,Vector2.up);
                }
                anim.SetBool("caminar",false);
             }
        }else{
            if(bloqueado)
            {
                FinalizarAtaque();
            }
        }
        
       
    }
}