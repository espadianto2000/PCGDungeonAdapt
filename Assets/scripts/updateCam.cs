using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Unity.Services.Core;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
//using Unity.Services.Analytics;

public class algoritmoInSala
{
    public int mxObstaculos;
    public int obstaculos;
    public int enemigos;
    public int trampas;
    public bool premio;
    public float tiempo;
    public algoritmoInSala(int mxObstaculos, int obstaculos, int enemigos, int trampas, bool premio, float tiempo)
    {
        this.mxObstaculos = mxObstaculos;
        this.obstaculos = obstaculos;
        this.enemigos = enemigos;
        this.trampas = trampas;
        this.premio = premio;
        this.tiempo = tiempo;
    }
}

public class salaFAdapt
{
    public string usuario;
    public string inicioRun;
    public int nivel;
    public float dificultad;
    public DanoRecibido danoRecibido = new DanoRecibido();
    public ValoracionesEnemigos valoracionEnemigos = new ValoracionesEnemigos();
    public float tiempoSala;
    public int danoRecibidoTotal;
    public bool salaJefe;
    public salaFAdapt(string us, string ini, int niv, float dif, int en1, int en2, int en3, int en4, float[] val, float t, int danoJ, bool jef)
    {
        this.usuario = us;
        this.inicioRun = ini;
        this.nivel = niv;
        this.dificultad = dif;
        this.danoRecibido.enem1 = en1;
        this.danoRecibido.enem2 = en2;
        this.danoRecibido.enem3 = en3;
        this.danoRecibido.enem4 = en4;
        this.valoracionEnemigos.enem1 = val[0];
        this.valoracionEnemigos.enem2 = val[1];
        this.valoracionEnemigos.enem3 = val[2];
        this.valoracionEnemigos.enem4 = val[3];
        this.tiempoSala = t;
        this.danoRecibidoTotal = danoJ;
        this.salaJefe = jef;
    }
}
public class DanoRecibido
{
    public int enem1;
    public int enem2;
    public int enem3;
    public int enem4;
}
public class updateCam : MonoBehaviour
{
    public GameObject cam;
    public salas salas;
    public bool entrada = false;
    public GameObject player;
    public bool moverjugador = false;
    public Vector3 destino;
    public GameObject puertas;
    public List<GameObject> pr;
    public bool finalizado = false;
    public float velocidadTemp = -500f;
    public gameManager gm;
    public List<GameObject> enemigosInstanciados;
    public int contadorEnemigos=-1000;
    public bool spawnEnemigos = true;
    public GameObject premio = null;
    public GameObject PanelInfo;
    public GameObject map;
    public GameObject salaIn;
    public GameObject salaOut;
    public GameObject unknown;
    public GameObject boss;
    public bool contenidoGenerado = false;
    public bool spawnPortal=false;
    public List<GameObject> trampas;
    private dificultadAdaptable dl;
    public int danoRecibidoEnSala = 0;
    public int danoEnem1 = 0;
    public int danoEnem2 = 0;
    public int danoEnem3 = 0;
    public int danoEnem4 = 0;
    public bool[] tipoEnems = new bool[4];
    public float tiempoSala = -1;
    private AudioManager am;
    public string nombreBoss;
    public salaFAdapt sf;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < tipoEnems.Length; i++)
        {
            tipoEnems[i] = false;
        }
        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        gm = GameObject.Find("GameManager").GetComponent<gameManager>();
        cam = GameObject.Find("Camara");
        salas = GameObject.FindGameObjectWithTag("salas").GetComponent<salas>();
        player = GameObject.Find("player");
        PanelInfo = gm.panelInfo;
        map = GameObject.Find("mapa");
        //Debug.Log(player);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (entrada)
        {
            gm.InputEnable = false;
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, transform.position+new Vector3(0,50,0), 10 * Time.deltaTime);
            if(Vector3.Distance(cam.transform.position, transform.position + new Vector3(0, 50, 0))<0.01f)
            {
                gm.InputEnable = true;
                if (spawnEnemigos && !finalizado)
                {
                    if (boss != null)
                    {
                        boss.SetActive(true);
                        contadorEnemigos = 1;
                        spawnPortal = true;
                        nombreBoss = boss.name;
                    }
                    else
                    {
                        
                        foreach (GameObject en in enemigosInstanciados)
                        {
                            en.SetActive(true);
                        }
                        contadorEnemigos = enemigosInstanciados.Count;
                        foreach(GameObject tramp in trampas)
                        {
                            tramp.SetActive(true);
                        }
                    }
                }
                entrada = false;
            }
        }
        if (moverjugador)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, destino, 5 * Time.deltaTime);
            if(Vector3.Distance(player.transform.position, destino) < 0.01f)
            {
                //Debug.Log("moviendo hacia: " + destino);
                if (premio != null)
                {
                    PanelInfo.GetComponent<seguirMouse>().actualizar(premio);
                }
                player.GetComponentInChildren<Animator>().SetBool("corriendo", false);
                moverjugador = false;
                //player.GetComponent<charController>().cuerpo.transform.localPosition = new Vector3(-0.08f, -0.5f, -0.15f);
                player.GetComponent<charController>().animador.Play("Idle_Battle");
                if (!finalizado)
                {                    
                    GameObject puerta1 = Instantiate(puertas, transform.position + new Vector3(5.5f, 0.95f, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
                    GameObject puerta3 = Instantiate(puertas, transform.position + new Vector3(-5.5f, 0.95f, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
                    GameObject puerta2 = Instantiate(puertas, transform.position + new Vector3(0, 0.95f, 5.5f), new Quaternion(0, 0, 0, 0));
                    GameObject puerta4 = Instantiate(puertas, transform.position + new Vector3(0, 0.95f, -5.5f), new Quaternion(0, 0, 0, 0));
                    pr.Add(puerta1);
                    pr.Add(puerta2);
                    pr.Add(puerta3);
                    pr.Add(puerta4);
                    tiempoSala = 0;
                }
            }
        }
        if(contadorEnemigos == 0 && !finalizado)
        {
            
            if (gm.identificado)
            {
                //---------------PruebaFirebaseNest--------------
                string id = gm.identificadorMaq.Replace('.', ',');
                id = id.Replace('/', ',');
                string[] ident = id.Split('|');
                sf = new salaFAdapt(ident[0], ident[1], dl.nivel, dl.nivelDificultad, danoEnem1, danoEnem2, danoEnem3, danoEnem4, dl.GetComponent<evaluadorDeDesempeño>().valoraciones, tiempoSala, danoRecibidoEnSala, spawnPortal);
                string jsonString = JsonConvert.SerializeObject(sf);
                Debug.Log(jsonString);
                StartCoroutine(salaFinalizada(jsonString));
                //-----------------------------------------------
                /*Debug.Log("salaFinalizada: " + Analytics.IsCustomEventEnabled("salaFinalizada"));
                AnalyticsResult anRes = Analytics.CustomEvent("salaFinalizada-"+ gm.identificadorMaq +"-"+ dl.nivel+"dif: "+dl.nivelDificultad+"("+danoEnem1+","+danoEnem2+","+danoEnem3+","+danoEnem4+")", new Dictionary<string, object>
                {
                    { "tiempo", tiempoSala },
                    { "danoRecibido", danoRecibidoEnSala },
                    { "salaJefe", spawnPortal },
                    {"valoracionesEnemigos","("+dl.GetComponent<evaluadorDeDesempe�o>().valoraciones[0]+","+dl.GetComponent<evaluadorDeDesempe�o>().valoraciones[1]+","+dl.GetComponent<evaluadorDeDesempe�o>().valoraciones[2]+","+dl.GetComponent<evaluadorDeDesempe�o>().valoraciones[3]+")" }
                });
                Debug.Log("analyticsResult salaFinalizada: " + anRes);
                Analytics.FlushEvents();*/
                tiempoSala = -1;
            }
            salas.salasSuperadas++;
            FinalizarSala();
            if (spawnPortal)
            {
                if (premio != null)
                {
                    Destroy(premio.gameObject);
                }
                GetComponent<generarDistribucion>().instanciarPremio();
                PanelInfo.GetComponent<seguirMouse>().actualizar(premio);
                premio.SetActive(true);
                Instantiate(gm.portal, new Vector3(transform.position.x,0.5f,transform.position.z), Quaternion.Euler(new Vector3(-90,0,0)));
                mandarEvaluadorBoss();
            }
            else
            {
                mandarEvaluadorEnemigos();
            }
            Debug.Log("desempeño enem1: " + dl.GetComponent<evaluadorDeDesempeño>().valoraciones[0]);
            Debug.Log("desempeño enem2: " + dl.GetComponent<evaluadorDeDesempeño>().valoraciones[1]);
            Debug.Log("desempeño enem3: " + dl.GetComponent<evaluadorDeDesempeño>().valoraciones[2]);
            Debug.Log("desempeño enem4: " + dl.GetComponent<evaluadorDeDesempeño>().valoraciones[3]);
            List<int>[] temp = dl.GetComponent<evaluadorDeDesempeño>().listaDeArrays[dl.nivel];
            dl.actualizarModelosEnemigos();
        }
    }
    IEnumerator salaFinalizada(string js)
    {
        UnityWebRequest uwr = new UnityWebRequest("https://pcg-nest.herokuapp.com/salaAdapt", "POST");
        byte[] xmlToSend = Encoding.UTF8.GetBytes(js);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(xmlToSend);
        string cadenadeXML = Encoding.UTF8.GetString(xmlToSend);
        //Debug.Log(cadenadeXML);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            string servicioResult2 = uwr.downloadHandler.text;
            Debug.Log("error webrequest: " + servicioResult2);
            Debug.Log("statusCode: " + uwr.responseCode);
            uwr.Dispose();
            yield break;
        }
        else
        {
            Debug.Log("se envio la data");
            Debug.Log("statusCode: " + uwr.responseCode);
            uwr.Dispose();
            yield break;
        }
    }
    public void mandarEvaluadorEnemigos()
    {
        if (tipoEnems[0])
        {
            dl.GetComponent<evaluadorDeDesempeño>().ocurrenciaEnemigo1(dl.nivel, danoEnem1);
        }
        if (tipoEnems[1])
        {
            dl.GetComponent<evaluadorDeDesempeño>().ocurrenciaEnemigo2(dl.nivel, danoEnem2);
        }
        if (tipoEnems[2])
        {
            dl.GetComponent<evaluadorDeDesempeño>().ocurrenciaEnemigo3(dl.nivel, danoEnem3);
        }
        if (tipoEnems[3])
        {
            dl.GetComponent<evaluadorDeDesempeño>().ocurrenciaEnemigo4(dl.nivel, danoEnem4);
        }
    }
    public void mandarEvaluadorBoss()
    {
        if (nombreBoss.Contains("Red"))
        {
            dl.GetComponent<evaluadorDeDesempeño>().ocurrenciaBoss1(dl.nivel, danoRecibidoEnSala);
        }
        if (nombreBoss.Contains("among us"))
        {
            dl.GetComponent<evaluadorDeDesempeño>().ocurrenciaBoss2(dl.nivel, danoRecibidoEnSala);
        }
        if (nombreBoss.Contains("GruntPol"))
        {
            dl.GetComponent<evaluadorDeDesempeño>().ocurrenciaBoss3(dl.nivel, danoRecibidoEnSala);
        }
    }
    private void FixedUpdate()
    {
        tiempoSala += tiempoSala >= 0 ? Time.fixedDeltaTime : 0f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (!finalizado && boss != null)
            {
                am.activarBoss();
            }else if (spawnPortal)
            {
                if (am.estado != 3)
                {
                    am.activarWin();
                }
            }
            else if(transform.position.x != 0 && transform.position.z != 0)
            {
                if (am.estado != 1)
                {
                    am.activarGameplay();
                }
            }
            other.GetComponent<charController>().salaActual = this;
            if (!contenidoGenerado)
            {
                dl = GameObject.Find("dificultad").GetComponent<dificultadAdaptable>();
                float t1 = Time.realtimeSinceStartup;
                GetComponent<generarDistribucion>().generarElementos2(dl.numObs,dl.numEnemigos,dl.numTrampas);
                GetComponent<generarDistribucion>().instanciarElementos(dl);
                contenidoGenerado = true;
                float t2 = Time.realtimeSinceStartup;
                //
                algoritmoInSala ais = new algoritmoInSala(dl.numObs,GetComponent<generarDistribucion>().contObs,dl.numEnemigos,dl.numTrampas, premio,(t2-t1));
                string jsonString = JsonConvert.SerializeObject(ais);
                StartCoroutine(tileMap(jsonString));
                //
                //Debug.Log("tiempo de algoritmo inSala: " + (t2 - t1));
            }
            salaOut.SetActive(false);
            salaIn.SetActive(true);
            //Debug.Log(transform.position.x + "," + transform.position.z);
            foreach(GameObject cuadro in GameObject.FindGameObjectsWithTag("Unknown"))
            {
                //Debug.Log(cuadro.transform.position.x+","+cuadro.transform.position.z);
                if((Mathf.Abs(cuadro.transform.position.x-transform.position.x) == 11 && Mathf.Abs(cuadro.transform.position.z - transform.position.z) == 0) || (Mathf.Abs(cuadro.transform.position.z-transform.position.z) == 11 && Mathf.Abs(cuadro.transform.position.x - transform.position.x) == 0))
                {
                    cuadro.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            entrada = true;
            player.GetComponent<charController>().animador.Play("RunForwardBattle");
            //Debug.Log("se ha entrado a la sala en: " + transform.position);
            if(Mathf.Abs(other.transform.position.x - transform.position.x) < 0.1f && Mathf.Abs(other.transform.position.z - transform.position.z) < 0.1f)
            {
                //Debug.Log("sala Inicial");
                spawnEnemigos = false;
                finalizado = true;
            }
            else
            {
                //Debug.Log("se debe mover");
                float posXJ = player.transform.position.x;
                float posZJ = player.transform.position.z;
                float destX = transform.position.x;
                float destZ = transform.position.z;
                if ((posXJ - destX) > 3)
                {
                    destX += 4.5f;
                    unknown.transform.GetChild(3).gameObject.SetActive(true);
                }
                else if((posXJ - destX) < -3)
                {
                    destX -= 4.5f;
                    unknown.transform.GetChild(1).gameObject.SetActive(true);
                }
                else { destX = posXJ; }
                if((posZJ - destZ) > 3)
                {
                    destZ += 4.5f;
                    unknown.transform.GetChild(2).gameObject.SetActive(true);
                }
                else if((posZJ - destZ) < -3)
                {
                    destZ -= 4.5f;
                    unknown.transform.GetChild(4).gameObject.SetActive(true);
                }
                else { destZ = posZJ; }
                destino = new Vector3(destX, 0.6f, destZ);
                //Debug.Log("moviendo hacia: " + destino);

                velocidadTemp = other.GetComponent<statsJugador>().velocidad;
                moverjugador = true;
            }
            map.transform.position = transform.position + new Vector3(0, 60, 0);
        }
    }

    IEnumerator tileMap(string js)
    {
        UnityWebRequest uwr = new UnityWebRequest("https://pcg-nest.herokuapp.com/tileMapAdapt", "POST");
        byte[] xmlToSend = Encoding.UTF8.GetBytes(js);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(xmlToSend);
        string cadenadeXML = Encoding.UTF8.GetString(xmlToSend);
        //Debug.Log(cadenadeXML);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            string servicioResult2 = uwr.downloadHandler.text;
            Debug.Log("error webrequest: " + servicioResult2);
            Debug.Log("statusCode: " + uwr.responseCode);
            uwr.Dispose();
            yield break;
        }
        else
        {
            Debug.Log("se envio la data");
            Debug.Log("statusCode: " + uwr.responseCode);
            uwr.Dispose();
            yield break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("player"))
        {
            salaIn.SetActive(false);
            salaOut.SetActive(true);
        }
    }
    public void FinalizarSala()
    {
        finalizado = true;
        if (premio != null)
        {
            gm.numeroPremiosNivel++;
            premio.SetActive(true);
        }
        foreach (GameObject p in pr)
        {
            Destroy(p);
        }
    }
}
