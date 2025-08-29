using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] float timePassed;
    [SerializeField] GameObject arrow;
    [SerializeField] Image clockFill;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);
    }


    void Update()
    {
        if(gameStarted)
        {
            timePassed += Time.deltaTime;
        }
        
        if(timePassed >= 90)
        {
            gameStarted = false;
            Debug.Log("Tiempo!!");
        }
        else
        {
            clockFill.fillAmount = timePassed / 90f;
            arrow.transform.eulerAngles = new Vector3(arrow.transform.rotation.x, arrow.transform.rotation.y, -(timePassed * 4));
        }
    }
}
