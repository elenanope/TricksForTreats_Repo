using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetitionRewardGenerator : MonoBehaviour
{
    int trickNumber = -1;
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
            dogController = GetComponent<DogController>();
            GenerateTrick();
            //if(trickNumber == 4) 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && trickPhase==1) trickPhase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(dogController != null && trickPhase != 2)
        {
            if(dogController.trickDone == trickNumber && trickNumber>=0)
            {
                Instantiate(biscuitPrefab, transform.position, Quaternion.identity);
                trickPhase = 2;
            }
        }
    }
    void GenerateTrick()
    {
        trickNumber = Random.Range(0, availablesTricks);
        trickBubbleAnim.SetBool(trickNumber, true);
        trickBubble.SetActive(true);
        trickPhase = 1;
    }
}
