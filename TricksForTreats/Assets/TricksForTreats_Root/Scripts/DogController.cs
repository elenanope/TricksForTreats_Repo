using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    //public bool gameStarted;
    //int eatenBiscuits = 0;
    public int trickDone = -1; //0 bark, 1 sit, 2 paw, 3 platz, 4 ball
    [SerializeField] int hydration = 10;
    public bool carryingBall;
    [SerializeField] bool isFacingRight = true;
    [SerializeField] bool drinking = false;
    [SerializeField] bool mandatoryCooldown = false;
    [SerializeField] float movSpeed;
    [SerializeField] float throwForce;
    [SerializeField] float drinkTimeElapsed;
    [SerializeField] Animator dogAnim;
    [SerializeField] Rigidbody dogRb;
    public GameObject lastBall;
    [SerializeField] Vector3 forceDirection;
    Vector3 moveInput;

    [SerializeField] Image waterFill;
    //[SerializeField] AudioClip waterDrop;
    //[SerializeField] AudioClip bark;
    //[SerializeField] AudioClip bite;
    //[SerializeField] AudioSource audioSource;

    void Start()
    {
        dogRb = GetComponent<Rigidbody>();
        dogAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        waterFill.fillAmount = (float)hydration / 10f;
        if(hydration <= 0) GameManager.Instance.gameStarted = false; //anim corta, fade + pantalla del principio
        if (GameManager.Instance.gameStarted)
        {
            if (hydration > -1)
            {
                if (!drinking)
                {
                    if (hydration > -1)
                    {
                        drinkTimeElapsed += Time.deltaTime;
                        if (drinkTimeElapsed > 5)
                        {
                            if(hydration >0)hydration--;
                            Debug.Log("Has perdido agua! Te queda " + hydration);
                            drinkTimeElapsed = 0;
                        }
                    }
                }
                else
                {
                    if (hydration < 11)
                    {
                        drinkTimeElapsed += Time.deltaTime;
                        if (drinkTimeElapsed > 2)
                        {
                            if(hydration < 10)
                            {
                                hydration++;
                                if (AudioManager.Instance.audioState < 2)
                                {
                                    //AudioManager.Instance.clipNumber = 0;
                                    AudioManager.Instance.PlaySFX(0);
                                }
                                //audioSource.clip = waterDrop;
                                //audioSource.Play();
                            }
                                
                            Debug.Log("Has bebido agua! Tienes " + hydration);
                            drinkTimeElapsed = 0;
                        }
                    }
                }

                if (dogAnim.GetInteger("trickNumber") != trickDone)
                {
                    dogAnim.SetInteger("trickNumber", trickDone);
                }
                if (moveInput.x > 0 && !isFacingRight) DogFlip();
                else if (moveInput.x < 0 && isFacingRight) DogFlip();
            }
            else
            {
                Debug.Log("You are dehydrated");
            }
        }
        
    }
    private void FixedUpdate()
    {
        if(GameManager.Instance.gameStarted)
        {
            if (moveInput != null && moveInput.x == 0 && moveInput.z == 0)
            {
                dogAnim.SetBool("isWalking", false);
            }
            else
            {
                Move();
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Contains("ball") && lastBall== null)
        {
            dogAnim.SetBool("ball", true);
            carryingBall = true;
            trickDone = 4;
            lastBall = other.gameObject;
            lastBall.SetActive(false);
        }
        if(other.gameObject.name.Contains("biscuit"))
        {
            GameManager.Instance.lastBiscuits++;
            other.gameObject.SetActive(false);
            if (AudioManager.Instance.audioState < 2)
            {
                //AudioManager.Instance.clipNumber = 1;
                AudioManager.Instance.PlaySFX(1);
            }
            //audioSource.clip = bite;
            //audioSource.Play();
            Debug.Log("Te has comido las siguientes galletas!"+ GameManager.Instance.lastBiscuits);
            //sonido win
        }
        if(other.gameObject.name.Contains("fountain"))
        {
            drinking = true;
            //sonido agua
        }
        if(other.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.LoadNextLevel();
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("fountain"))
        {
            drinking = false;
        }
    }
    void Move()
    {
        dogRb.velocity = moveInput * movSpeed;
        dogAnim.SetBool("isWalking", true);
        if (trickDone != 4) trickDone = -1;
    }
    void DogFlip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }
    //IEnumerator ResetTrickDone()
    IEnumerator ResetCollider()
    {
        yield return new WaitForSeconds(1);
        lastBall.GetComponent<BoxCollider>().enabled = true;
        lastBall = null;
        yield break;
    }
    IEnumerator Cooldown()
    {
        mandatoryCooldown = true;
        yield return new WaitForSeconds(1);
        mandatoryCooldown = false;
        yield break;
    }
    public void Walking (InputAction.CallbackContext context)
    {
        
        moveInput.x = context.ReadValue<Vector2>().x;
        moveInput.z = context.ReadValue<Vector2>().y;
        
    }
    public void Bark (InputAction.CallbackContext context)
    {
        if (context.performed && trickDone == -1 && !mandatoryCooldown)
        {
            StartCoroutine(Cooldown());
            trickDone = 0;
            if (AudioManager.Instance.audioState < 2)
            {
                //AudioManager.Instance.clipNumber = 2;
                AudioManager.Instance.PlaySFX(2);
            }
            if (carryingBall)
            {
                dogAnim.SetBool("ball", false);
                lastBall.transform.position = transform.position;
                lastBall.SetActive(true);
                lastBall.GetComponent<BoxCollider>().enabled = false;
                if (isFacingRight) lastBall.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
                //else lastBall.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
                else lastBall.GetComponent<Rigidbody>().AddForce(-forceDirection.x, forceDirection.y, forceDirection.z, ForceMode.Impulse);
                //lanzar pelota fuera creándola/poniendo la antigua en esa posición
                carryingBall = false;
                StartCoroutine(ResetCollider());
            }
        }
    }
    public void Sit (InputAction.CallbackContext context)
    {
        if (context.performed && !mandatoryCooldown)
        {
            StartCoroutine(Cooldown());
            trickDone = 1;
        }
    }
    public void Paw (InputAction.CallbackContext context)
    {
        if (context.performed && !mandatoryCooldown)
        {
            StartCoroutine(Cooldown());
            trickDone = 2;
        }
    }
    public void Platz (InputAction.CallbackContext context)
    {
        if (context.performed && !mandatoryCooldown) 
        {
            StartCoroutine(Cooldown());
            trickDone = 3;
        }
    }
    public void StartGame ()
    {
        GameManager.Instance.gameStarted = true;
    }
    public void ExitGame ()
    {
        Application.Quit();
    }

}
