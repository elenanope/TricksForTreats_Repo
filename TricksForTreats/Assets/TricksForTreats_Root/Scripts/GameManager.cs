using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("GameManager is null!");
            }
            return instance;
        }
    }

    public bool gameStarted;
    [SerializeField] bool safeMode;
    public bool loadNewGame;//0 nada, 1 fade, 2 load
    [SerializeField] float timePassed;
    public int lastBiscuits;
    public int endNumber;
    [SerializeField] int totalBiscuits;
    int actualScene;
    [SerializeField] GameObject arrow;
    [SerializeField] Image clockFill;
    [SerializeField] GameObject fadePanel;
    [SerializeField] GameObject fadePanel2;
    [SerializeField] GameObject cinematic = null;
    [SerializeField] GameObject[] endPanels = new GameObject[4]; // 0 lose water, 1 lose Time, 2 win 1, 3 win 2
    [SerializeField] Color imageToFade;
    [SerializeField] float alpha = 0;
    [SerializeField] float fadeSpeed = 2f;
    [SerializeField] int fading;//1 de negro a trans, 2 al revs

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

        fadePanel.SetActive(false);
        actualScene = 0;
        fadePanel2 = GameObject.Find("fadePanel1");
        StartCoroutine(FadeIn());
    }


    void Update()
    {
        if(gameStarted)
        {
            timePassed += Time.deltaTime;
            if (arrow == null)
            {
                arrow = GameObject.Find("clockArrow");
            }
            if (clockFill == null)
            {
                clockFill = GameObject.Find("clockFill")?.GetComponent<Image>(); //ver si esto va bien //errores al restart
            }
            if (fadePanel == null)
            {
                fadePanel = GameObject.Find("fadePanel");
                //fadePanel.SetActive (false);
            }
            else fadePanel.SetActive (false);
            if (fadePanel2 == null)
            {
                fadePanel2 = GameObject.Find("fadePanel1");
                //fadePanel.SetActive (false);
            }

            if (endPanels[0] == null)
            {
                endPanels[0] = GameObject.Find("LoseDrinkPanel");
            }
            else endPanels[0].SetActive (false);
            if (endPanels[1] == null)
            {
                endPanels[1] = GameObject.Find("LoseTimePanel");
            }
            else endPanels[1].SetActive (false);
            if(actualScene == 4)
            {
                if (endPanels[2] == null)
                {
                    endPanels[2] = GameObject.Find("WinPanel0");
                }
                else endPanels[2].SetActive(false);
                if (endPanels[3] == null)
                {
                    endPanels[3] = GameObject.Find("WinPanel1");
                }
                else endPanels[3].SetActive(false);
            }
        }

        if (loadNewGame)
        {
            StartCoroutine(Restart());
            //if(!fadePanel.activeSelf)  //errores al restart
            {
                //StartCoroutine(Restart());
            }
        }
        
        if(timePassed >= 90)
        {
            gameStarted = false;
            Debug.Log("Tiempo!!");
            GameManager.Instance.endNumber = 1;
            loadNewGame = true;
            //anim corta, fade + pantalla del principio
        }
        else
        {
            if(clockFill != null) clockFill.fillAmount = timePassed / 90f;
            if (arrow != null) arrow.transform.eulerAngles = new Vector3(arrow.transform.rotation.x, arrow.transform.rotation.y, -(timePassed * 4));
        }

        if (imageToFade != null)
        {
            if(fading==1)
            {
                if(!fadePanel.activeSelf)fadePanel.SetActive(true);
                imageToFade.a = Mathf.Clamp01(imageToFade.a + fadeSpeed * Time.deltaTime);
                fadePanel.GetComponent<Image>().color = imageToFade;
            }
            else if(fading==2)
            {
                imageToFade.a = Mathf.Clamp01(imageToFade.a - fadeSpeed * Time.deltaTime);
                fadePanel2.GetComponent<Image>().color = imageToFade;
            }
            
        }
    }
    
    IEnumerator Restart()
    {
        loadNewGame = false;
        timePassed = 0;
        AudioManager.Instance.PlaySFX(4);
        lastBiscuits = 0;
        fadePanel.SetActive(true);
        yield return new WaitForSeconds(2);
        endPanels[endNumber].SetActive(true);
        imageToFade = endPanels[endNumber].GetComponent<Image>().color;
        yield return new WaitForSeconds(1);
        fading = 1;
        yield return new WaitForSeconds(3);
        fading= 0;

        if (safeMode)
        {
            SceneManager.LoadScene(actualScene);
            fadePanel.SetActive(false);
        }
        else
        {
            totalBiscuits = 0;
            SceneManager.LoadScene(0);
            actualScene = 0;
        }
        AudioManager.Instance.restart = true;
        yield break;
    }
    IEnumerator FadeOut()
    {
        gameStarted = false;
        totalBiscuits += lastBiscuits;
        lastBiscuits = 0;
        if (actualScene < 4)
        {
            actualScene++;
            Debug.Log("Ahora tienes estas galletas: " + totalBiscuits);
            yield return new WaitForSeconds(1);

            imageToFade = fadePanel.GetComponent<Image>().color;
            fading = 1;
            yield return new WaitForSeconds(2);
            fading = 0;
            
            SceneManager.LoadScene(actualScene);
            timePassed = 0;
            gameStarted = true;
            AudioManager.Instance.restart = true;
            StartCoroutine(FadeIn());
        }
        else
        {
            if (totalBiscuits < 37) //revisar cuando niveles montados
            {
                endNumber = 2;
            }
            else endNumber = 3;
            Restart();
        }
        
        yield break;
    }
    IEnumerator FadeIn()
    {
        Debug.Log("fading");
        yield return new WaitForSeconds(1);

        //fadePanel2 = GameObject.Find("fadePanel1");
        imageToFade = fadePanel2.GetComponent<Image>().color;
        fading = 2;
        yield return new WaitForSeconds(1.6f);
        fading= 0;
        fadePanel2.SetActive(false);
        yield break;
    }
    public void LoadNextLevel()
    {
        if(actualScene<4)StartCoroutine(FadeOut());
        else StartCoroutine(Restart());
            
        //cargar siguiente escena
    }
    /*public void LoadNextLevel()
    {
        timePassed = 0;
        totalBiscuits += lastBiscuits;
        lastBiscuits = 0;
        if(actualScene <4)
        {
            actualScene++;
            Debug.Log("Ahora tienes estas galletas: " + totalBiscuits);
            SceneManager.LoadScene(actualScene);
            AudioManager.Instance.restart = true;
            StartCoroutine(FadeIn());
        }
        else
        {
            if (totalBiscuits < 37) //revisar cuando niveles montados
            {
                endNumber = 2;
            }
            else endNumber = 3;
            Restart();
        }
            
        //cargar siguiente escena
    }*/
    public void ChangeGameMode(bool easyMode)
    {
        safeMode = easyMode;
    }
}
