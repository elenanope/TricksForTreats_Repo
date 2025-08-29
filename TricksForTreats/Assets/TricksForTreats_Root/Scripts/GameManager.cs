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
     bool loadNewGame;//0 nada, 1 fade, 2 load
    [SerializeField] float timePassed;
    public int lastBiscuits;
    [SerializeField] int totalBiscuits;
    int actualScene;
    [SerializeField] GameObject arrow;
    [SerializeField] Image clockFill;
    [SerializeField] GameObject fadePanel;

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
        if(arrow == null)
        {
            arrow = GameObject.Find("clockArrow");
        }
        if(clockFill == null)
        {
            clockFill = GameObject.Find("clockFill").GetComponent<Image>();
        }
        if(fadePanel == null)
        {
            fadePanel = GameObject.Find("fadePanel");
        }
        if(loadNewGame)
        {
            if(!fadePanel.activeSelf)
            {
                StartCoroutine(Restart());
            }
        }
        
        if(gameStarted)
        {
            timePassed += Time.deltaTime;
            
        }
        else
        {
            if (timePassed > 10)
            {
                loadNewGame = true;
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
            clockFill.fillAmount = timePassed / 90f;
            arrow.transform.eulerAngles = new Vector3(arrow.transform.rotation.x, arrow.transform.rotation.y, -(timePassed * 4));
        }
    }
    
    IEnumerator Restart()
    {
        lastBiscuits = 0;
        fadePanel.SetActive(true);
        yield return new WaitForSeconds(2);
        loadNewGame = false;
        SceneManager.LoadScene(actualScene);
    }
    public void LoadNextLevel()
    {
        totalBiscuits += lastBiscuits;
        lastBiscuits = 0;
        actualScene++;
        Debug.Log("Ahora tienes estas galletas: " + totalBiscuits);
        SceneManager.LoadScene(actualScene);
        //cargar siguiente escena
    }
}
