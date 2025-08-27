using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetitionRewardGenerator : MonoBehaviour
{
     int trickNumber = -2;
    [SerializeField] int availablesTricks = 4;
    [SerializeField] bool hasBall;
    [SerializeField] int trickPhase;
    [SerializeField] GameObject trickBubble;
    [SerializeField] GameObject biscuitPrefab;
     DogController dogController;
    [SerializeField] Animator trickBubbleAnim;

    private void Start()
    {
        if (hasBall) availablesTricks++;
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && trickPhase == 0)
        {
            dogController = other.gameObject.GetComponent<DogController>();
            GenerateTrick();
            //if(trickNumber == 4) 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && trickPhase==1)
        {
            trickPhase = 0; trickBubble.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dogController != null && trickPhase != 2)
        {
            Debug.Log("debug 1" + dogController.trickDone);
            if (dogController.trickDone == trickNumber && trickNumber >= 0)
            {
                Instantiate(biscuitPrefab, transform.position, Quaternion.identity);
                trickBubble.SetActive(false);
                Debug.Log("Completado");
                trickPhase = 2;
            }
        }
        else Debug.Log("Error");
    }
    void GenerateTrick()
    {
        
        if (trickNumber < 0)
        {
            trickNumber = Random.Range(0, availablesTricks);
            Debug.Log( "truco generado" + trickNumber);
            
           
        }
        trickBubble.GetComponent<SpriteRenderer>().enabled = false;
        trickBubble.SetActive(true);
        trickBubbleAnim.SetInteger("tricksNumber", -2);
        trickBubbleAnim.SetInteger("tricksNumber", trickNumber);
        trickBubble.GetComponent<SpriteRenderer>().enabled = true;
        trickPhase = 1;
    }
}
