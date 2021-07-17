using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using System;
using System.Linq;

public class Movement : MonoBehaviour
{     
    public float jumpForce;
    private bool isGrounded;    // to know when the player is in the air or not
    public Transform checkGround;   // check if this is Ground
    public LayerMask whatIsGround;  // set this Mask to all objects that are 'Ground'
    private int multipleJumps;  // allow the player to make multiple jumps while on air
    public int jumpValue;       // max number of jumps in the air

    // this variable will help us avoid fission, by manually checking if
    // 2 actions are being called within avoidFisionMs ms
    private DateTime previousAction;    // DateTime of the previous action that has been done
    public int avoidFissionMs;          // the offset we wait till the next action (in MillieSeconds)

    // set up the KeywordRecognizer (for the voice input)
    private Dictionary<string, Action> keywordActions = new Dictionary<string, Action>();
    private KeywordRecognizer keywordRecognizer;

    public Rigidbody2D rb;      // this is our player
    public GameObject gameover; // gameover Image (canvas)
    public GameObject gamewon;  // gamewon Image (canvas)


    // Start is called before the first frame update
    void Start()
    {
        previousAction = DateTime.Now;  // initialise the variable for the 1st time
        // voice command stuff
        keywordActions.Add("up", Up);
        keywordActions.Add("left", Left);
        keywordActions.Add("right", Right);
        keywordRecognizer = new KeywordRecognizer(keywordActions.Keys.ToArray(), ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;
        keywordRecognizer.Start();
        // rest of the game
        multipleJumps = jumpValue;
        gameover.SetActive(false);
        gamewon.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }

    // voice commands as functions to be called
    void Up()
    {
        Debug.Log("Up");
        if ((DateTime.Now - previousAction).TotalMilliseconds < avoidFissionMs) {
            Debug.Log("too soon, wait a bit");
            // do nothing *fision
            return;
        }
        if (multipleJumps > 0) {
            rb.velocity = Vector2.up * jumpForce;
            multipleJumps = multipleJumps - 1;
        } else if (multipleJumps == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * multipleJumps;
        }
        previousAction = DateTime.Now;  // update variable for the next action check
    }
    void Left()
    {
        Debug.Log("Left");
        if ((DateTime.Now - previousAction).TotalMilliseconds < avoidFissionMs) {
            Debug.Log("too soon, wait a bit");
            // do nothing *fision
            return;
        }
        transform.position = new Vector2(rb.position.x - 1, rb.position.y); // move left
        previousAction = DateTime.Now;      // update variable for the next action check
    }
    void Right()
    {
        Debug.Log("Right");
        if ((DateTime.Now - previousAction).TotalMilliseconds < avoidFissionMs) {
            Debug.Log("too soon, wait a bit");
            // do nothing *fision
            return;
        }
        transform.position = new Vector2(rb.position.x + 1, rb.position.y); // move right
        previousAction = DateTime.Now;      // update variable for the next action check
    }


    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("You just said: " + args.text);
        keywordActions[args.text].Invoke();
    }
    

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(checkGround.position, 0.5f, whatIsGround); // check if object is 'ground'

        if (rb.transform.position.y < -7 || rb.transform.position.x > 10 || rb.transform.position.x < -10)
        {
            Debug.Log("You fell");
            this.enabled = false;	    // Disable the players movement.
            rb.isKinematic = true;
            gameover.SetActive(true);   // Call the GameOver Image
        }
        if (rb.transform.position.x > 6.5354 && rb.transform.position.y > 2.27)
        {
            Debug.Log("You won");
            this.enabled = false;       // Disable the players movement.
            rb.isKinematic = true;
            gamewon.SetActive(true);    // Call the GameWon Image
        }
    }


    void Update()
    {
        if (isGrounded == true)
        {
            multipleJumps = jumpValue;  // when player touches the ground, max jumps are reseted
        }

        if (Input.GetKeyDown(KeyCode.W) && multipleJumps > 0)
        {
            if ((DateTime.Now - previousAction).TotalMilliseconds < avoidFissionMs) {
                Debug.Log("too soon, wait a bit");
                Debug.Log((DateTime.Now - previousAction).TotalMilliseconds);
                // do nothing *fision
                return;
            }
            rb.velocity = Vector2.up * jumpForce;
            multipleJumps = multipleJumps - 1;
            previousAction = DateTime.Now;  // update variable for the next action check
        }
        else if (Input.GetKeyDown(KeyCode.W) && multipleJumps == 0 && isGrounded == true)
        {
            if ((DateTime.Now - previousAction).TotalMilliseconds < avoidFissionMs) {
                Debug.Log("too soon, wait a bit");
                // do nothing *fision
                return;
            }
            rb.velocity = Vector2.up * multipleJumps;
            previousAction = DateTime.Now;  // update variable for the next action check
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if ((DateTime.Now - previousAction).TotalMilliseconds < avoidFissionMs) {
                Debug.Log("too soon, wait a bit");
                Debug.Log((DateTime.Now - previousAction).TotalMilliseconds);
                // do nothing *fision
                return;
            }
            transform.position = new Vector2(rb.position.x - 1, rb.position.y);
            previousAction = DateTime.Now;  // update variable for the next action check
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if ((DateTime.Now - previousAction).TotalMilliseconds < avoidFissionMs) {
                Debug.Log("too soon, wait a bit");
                Debug.Log((DateTime.Now - previousAction).TotalMilliseconds);
                // do nothing *fision
                return;
            }
            transform.position = new Vector2(rb.position.x + 1, rb.position.y);
            previousAction = DateTime.Now;  // update variable for the next action check
        }
    }

}
