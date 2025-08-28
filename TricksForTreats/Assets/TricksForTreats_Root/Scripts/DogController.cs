using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    int eatenBiscuits = 0;
    public int trickDone = -1; //0 bark, 1 sit, 2 paw, 3 platz, 4 ball
    [SerializeField] int hidratation = 10;
    [SerializeField] bool carryingBall;
    [SerializeField] bool isFacingRight = true;
    [SerializeField] float movSpeed;
    [SerializeField] Animator dogAnim;
    [SerializeField] Rigidbody dogRb;
    [SerializeField] GameObject lastBall;
    Vector3 moveInput;
    
    void Start()
    {
        dogRb = GetComponent<Rigidbody>();
        dogAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(dogAnim.GetInteger("trickNumber") != trickDone)
        {
            dogAnim.SetInteger("trickNumber", trickDone);
        }
        if (moveInput.x > 0 && !isFacingRight) DogFlip();
        else if (moveInput.x < 0 && isFacingRight) DogFlip();
    }
    private void FixedUpdate()
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "ball")
        {
            dogAnim.SetBool("ball", true);
            carryingBall = true;
            trickDone = 4;
            lastBall = other.gameObject;
            lastBall.SetActive(false);
        }
        if(other.gameObject.name.Contains("biscuit"))
        {
            eatenBiscuits++;
            other.gameObject.SetActive(false);
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
    public void Walking (InputAction.CallbackContext context)
    {
        
        moveInput.x = context.ReadValue<Vector2>().x;
        moveInput.z = context.ReadValue<Vector2>().y;
        
    }
    public void Bark (InputAction.CallbackContext context)
    {
        if (context.performed && trickDone == -1)
        {
            trickDone = 0;
            if(carryingBall)
            {
                dogAnim.SetBool("ball", false);
                lastBall.SetActive(true);
                if(isFacingRight)lastBall.transform.position = new Vector3(transform.position.x +1, transform.position.y, transform.position.z -1 );
                else lastBall.transform.position = new Vector3(transform.position.x -1, transform.position.y, transform.position.z -1 );
                //lanzar pelota fuera creándola/poniendo la antigua en esa posición
                carryingBall = false;
            }
        }
    }
    public void Sit (InputAction.CallbackContext context)
    {
        if (context.performed) trickDone = 1;
    }
    public void Paw (InputAction.CallbackContext context)
    {
        if (context.performed) trickDone = 2;
    }
    public void Platz (InputAction.CallbackContext context)
    {
        if (context.performed) trickDone = 3;
    }

}
