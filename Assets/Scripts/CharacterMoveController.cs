using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterMoveController : MonoBehaviour
{
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("Jump")]
    public float jumpAccel;
    private bool isJumping;
    private bool isOnGround;
    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;
    private Rigidbody2D rb2d;
    private Animator anim;
    private CharacterSoundController sound;
    [Header("Scoring")]
    public ScoreController score;
    public float scoreRatio;
    private float lastPosX;

    private float lastPositionX; // For speed measure
    
    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;
    public Camera gameCamera;

    [Header("Speed")]
    public Text speed;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
        lastPositionX = rb2d.transform.position.x;
    }
    void Update(){
        // Get user input, in this case left click
        if(Input.GetMouseButtonDown(0)){
            if(isOnGround){
                isJumping = true;
                sound.PlayJump();
            }
        }
        // Set animation onground
        anim.SetBool("isOnGround",isOnGround);

        // calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPosX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed/scoreRatio);
        if(scoreIncrement>0){
            score.IncreaseScore(scoreIncrement);
            lastPosX += distancePassed;
        }

        // Check if the chracater is falling
        // game over
        if (transform.position.y < fallPositionY)
        {
            GameOver();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 vel = rb2d.velocity;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,groundRaycastDistance,groundLayerMask);
        if(hit){
            if(!isOnGround && rb2d.velocity.y <= 0){
                isOnGround = true;
            }
        }
        else{
            isOnGround = false;
        }
        if(isJumping){
            vel.y += jumpAccel;
            isJumping = false;
        }
        
        vel.x = Mathf.Clamp(vel.x + moveAccel * Time.fixedDeltaTime,0.0f,maxSpeed);
        rb2d.velocity = vel;
        speed.text = $"Speed: {Mathf.RoundToInt(Mathf.Abs(lastPositionX - rb2d.transform.position.x)/Time.fixedDeltaTime)} units/second";
        lastPositionX = rb2d.transform.position.x;
    }

    private void GameOver(){
        // Stop Scoring
        score.FinishScoring();
        // Stop Camera
        // gameCamera.p
        // Display GameOver
        gameOverScreen.SetActive(true);
        // Stop this object
        this.enabled = false;
    }
    private void OnDrawGizmos(){
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }

    public void IncreaseMaxSpeed(){
        maxSpeed++;
    }
}
