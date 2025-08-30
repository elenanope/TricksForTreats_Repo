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
     bool loadNewGame;//0 nada, 1 fade, 2 load
    [SerializeField] float timePassed;
    public int lastBiscuits;
    [SerializeField] int totalBiscuits;
    int actualScene;
    [SerializeField] GameObject arrow;
    [SerializeField] Image clockFill;
    [SerializeField] GameObject fadePanel;
    [SerializeField] GameObject cinematic = null;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

        fadePanel.SetActive(false);
        actualScene = 0;
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
        }
        else
        {
            if (timePassed > 10)
            {
                loadNewGame = true;
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
            loadNewGame = true;
            //anim corta, fade + pantalla del principio
        }
        else
        {
            if(clockFill != null) clockFill.fillAmount = timePassed / 90f;
            if (arrow != null) arrow.transform.eulerAngles = new Vector3(arrow.transform.rotation.x, arrow.transform.rotation.y, -(timePassed * 4));
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
        if(safeMode)
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
    public void LoadNextLevel()
    {
        timePassed = 0;
        totalBiscuits += lastBiscuits;
        lastBiscuits = 0;
        actualScene++;
        Debug.Log("Ahora tienes estas galletas: " + totalBiscuits);
        SceneManager.LoadScene(actualScene);
        AudioManager.Instance.restart = true;
        //cargar siguiente escena
    }
    public void ChangeGameMode(bool easyMode)
    {
        safeMode = easyMode;
    }
}
