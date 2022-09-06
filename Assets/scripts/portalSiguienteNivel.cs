using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using Unity.Services.Core;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
//using Unity.Services.Analytics;

public class NivelAdapt {
    public string usuario;
    public string inicioRun;
    public int nivel;
    public float dificultad;
    public int premiosNivel;
    public ValoracionesEnemigos valoracionesEnemigos = new ValoracionesEnemigos();
    public float tiempo;
    public int salasTotales;
    public int salasCompletadas;
    public float factorIncremento;
    public int danoRecibido;
    public NivelAdapt(string us, string ini, int niv, float dif, int prem,float[] valEn, float t, int st, int sc, float fact, int dano) 
    {
        this.usuario = us;
        this.inicioRun = ini;
        this.nivel = niv;
        this.dificultad = dif;
        this.premiosNivel = prem;
        this.valoracionesEnemigos.enem1 = valEn[0];
        this.valoracionesEnemigos.enem2 = valEn[1];
        this.valoracionesEnemigos.enem3 = valEn[2];
        this.valoracionesEnemigos.enem4 = valEn[3];
        this.tiempo = t;
        this.salasTotales = st;
        this.salasCompletadas = sc;
        this.factorIncremento = fact;
        this.danoRecibido = dano;
    }
}
public class ValoracionesEnemigos
{
    public float enem1;
    public float enem2;
    public float enem3;
    public float enem4;
}

public class portalSiguienteNivel : MonoBehaviour
{
    public gameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<gameManager>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            //---------------PruebaFirebaseNest--------------
            string id = gm.identificadorMaq.Replace('.', ',');
            id = id.Replace('/', ',');
            string[] ident = id.Split('|');
            GameObject dl = GameObject.Find("dificultad");
            dl.GetComponent<evaluadorDeDesempeño>().pasoNivel(dl.GetComponent<dificultadAdaptable>().nivel + 1);
            NivelAdapt nv = new NivelAdapt(ident[0], ident[1], dl.GetComponent<dificultadAdaptable>().nivel, dl.GetComponent<dificultadAdaptable>().nivelDificultad, gm.numeroPremiosNivel, dl.GetComponent<evaluadorDeDesempeño>().valoraciones, gm.tiempoNivel, GameObject.Find("salas(Clone)").GetComponent<salas>().contadorSalas + 1, GameObject.Find("salas(Clone)").GetComponent<salas>().salasSuperadas, dl.GetComponent<evaluadorDeDesempeño>().factorCrecimiento, other.GetComponent<charController>().danoNivel);
            string jsonString = JsonConvert.SerializeObject(nv);
            StartCoroutine(nivelCompletado(jsonString));
            //-----------------------------------------------
            /*Debug.Log("nivelFinalizado: "+Analytics.IsCustomEventEnabled("nivelFinalizado"));
            GameObject dl = GameObject.Find("dificultad");
            dl.GetComponent<evaluadorDeDesempeño>().pasoNivel(dl.GetComponent<dificultadAdaptable>().nivel + 1);*/
            /*AnalyticsResult anRes = Analytics.CustomEvent("nivelFinalizado-"+ gm.identificadorMaq+"-"+ dl.GetComponent<dificultadAdaptable>().nivel+"dif: "+dl.GetComponent<dificultadAdaptable>().nivelDificultad, new Dictionary<string, object>
                {
                    { "tiempo", gm.tiempoNivel },
                    { "danoRecibido", other.GetComponent<charController>().danoNivel },
                    { "PremiosNivel", gm.numeroPremiosNivel},
                    { "salasNivel", GameObject.Find("salas(Clone)").GetComponent<salas>().contadorSalas+1},
                    { "salasCompletadas", GameObject.Find("salas(Clone)").GetComponent<salas>().salasSuperadas},
                    {"valoracionesEnemigosSigNivel","("+dl.GetComponent<evaluadorDeDesempeño>().valoraciones[0]+","+dl.GetComponent<evaluadorDeDesempeño>().valoraciones[1]+","+dl.GetComponent<evaluadorDeDesempeño>().valoraciones[2]+","+dl.GetComponent<evaluadorDeDesempeño>().valoraciones[3]+")" },
                    {"factorDeIncremento",dl.GetComponent<evaluadorDeDesempeño>().factorCrecimiento }
                });
            Debug.Log("analyticsResult nivelFinalizado: " + anRes);
            Analytics.FlushEvents();*/
            other.GetComponent<charController>().danoNivel = 0;
            gm.numeroPremiosNivel = 0;
            gm.NextLevel();
        }
    }
    IEnumerator nivelCompletado(string js)
    {
        UnityWebRequest uwr = new UnityWebRequest("https://pcg-nest.herokuapp.com/nivelAdapt", "POST");
        byte[] xmlToSend = Encoding.UTF8.GetBytes(js);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(xmlToSend);
        string cadenadeXML = Encoding.UTF8.GetString(xmlToSend);
        Debug.Log(cadenadeXML);
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
}
