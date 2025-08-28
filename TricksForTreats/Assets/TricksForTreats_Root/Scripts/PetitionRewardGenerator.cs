using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetitionRewardGenerator : MonoBehaviour
{
     int trickNumber = -2;
    [SerializeField] int availablesTricks = 4;
    [SerializeField] bool hasBall;
    [SerializeField] bool nearby;
    [SerializeField] int trickPhase;
    [SerializeField] GameObject trickBubble;
    [SerializeField] GameObject biscuitPrefab;
    [SerializeField] GameObject ballPrefab = null;
     DogController dogController;
    [SerializeField] Animator trickBubbleAnim;
    /*
    private void Start()
    {
        if (hasBall) availablesTricks++;
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        nearby = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(trickPhase == 0)
            {
                    dogController = other.gameObject.GetComponent<DogController>();
                if (!hasBall)
                {
                    GenerateTrick();
                    //if(trickNumber == 4) 
                }
                else
                {
                    if (ballPrefab != null)
                    {
                        trickNumber = 4;
                        Vector3 objectSpawner = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z - 1.5f);
                        GameObject ball = Instantiate(ballPrefab, objectSpawner, Quaternion.identity);
                        ball.GetComponent<Rigidbody>().AddForce(Random.Range(4, 9), 0, Random.Range(1, 5), ForceMode.Impulse);
                        Debug.Log("Bola lanzada");
                        trickPhase = 1;
                    }
                }
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && trickPhase==1)
        {
            if(!hasBall)
            {
                trickPhase = 0; 
                trickBubble.SetActive(false);
            }
            nearby = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dogController != null && trickPhase != 2 && nearby)
        {
            //Debug.Log("primer debug: lo que ha hecho el perro, lo que deberia" + dogController.trickDone + trickNumber);
            if ((dogController.trickDone == trickNumber && trickNumber >= 0) || (dogController.carryingBall))
            {
                if (trickNumber == 4)
                {
                    Debug.Log("Checkpoint1");
                    dogController.gameObject.GetComponent<Animator>().SetBool("ball", false);
                    dogController.carryingBall = false;
                }
                Vector3 objectSpawner = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z -1.5f);
                GameObject biscuit = Instantiate(biscuitPrefab, objectSpawner, Quaternion.identity);
                biscuit.GetComponent<Rigidbody>().AddForce(2, 0, 1, ForceMode.Impulse);
                trickBubble.SetActive(false);
                Debug.Log("Completado");
                trickPhase = 2;
            }
            
        }
        //else Debug.Log("Error");
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
