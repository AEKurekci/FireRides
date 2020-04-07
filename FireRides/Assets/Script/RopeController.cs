using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RopeController : MonoBehaviour
{
    /*
     * ball: the player object
     * initialSwingingSpeed: It specify the speed of ball swinging before starting game
     * initialSwingingDistance: It specify the distance of ball swinging before starting game
     * ballSpeed: It specifies speed of ball in running game
     * rigidBodyOfBall: It keeps rigidbody of ball and exists for future physical activities
     * origin: It keeps position of ball when player wants to sling. Linerenderer draws a line from this position.(source value of line)
     * rope: it keeps SpringJoint element
     * layerMask: It distinguishes wall and ball object to avoid connect ball itself(8 is layer of wall)
     */
    [Header("Ball")]
    public GameObject ball;
    [SerializeField] float initialSwingingSpeed = 10f;
    [SerializeField] float initialSwingingDistance = 16f;
    private float ballSpeed;
    Rigidbody rigidBodyOfBall;
    Vector3 origin;

    [Header("Rope")]
    private SpringJoint rope;
    private bool firstRopeConnected = false;

    [Header("Game")]
    private bool gameStarted = false;
    public bool gameOver = false;
    public LineRenderer lineRenderer;
    int layerMask = 1 << 8;//wall layer

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rigidBodyOfBall = ball.GetComponent<Rigidbody>();
        ballSpeed = ball.GetComponent<Ball>().ballSpeed;
        origin = ball.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted && !gameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit aim;
                Physics.Raycast(origin, transform.forward + transform.up, out aim,Mathf.Infinity,layerMask);
                if (aim.transform != null)
                {
                    Sling(aim);
                    FindObjectOfType<RopeSound>().gameObject.GetComponent<AudioSource>().Play();
                    FindObjectOfType<whooshSound>().gameObject.GetComponent<AudioSource>().Play();
                }
            }
            else if (Input.GetMouseButton(0) && rope != null)
            {
                BallForce();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                GameObject.DestroyImmediate(rope);
                rigidBodyOfBall.useGravity = true;
                rigidBodyOfBall.drag = .5f;
                
                FindObjectOfType<whooshSound>().gameObject.GetComponent<AudioSource>().Stop();
            }
        }
        origin = ball.transform.position;

    }
    private void FixedUpdate()
    {
        if (!gameStarted)
        {
            if (!firstRopeConnected)
            {
                FirstRope();
            }
            else
            {
                Teeter();
            }
        }
    }

    void LateUpdate()
    {
        
        if (rope != null && !gameOver)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, ball.transform.position);
            lineRenderer.SetPosition(1, rope.connectedAnchor);
        }
        else
        {
            lineRenderer.enabled = false;
        }
        if (gameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RestartGame();
                FindObjectOfType<GameOverCanvas>().gameObject.GetComponent<Canvas>().enabled = false;
            }
            rigidBodyOfBall.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    /*
     * It connects first rope before starting
     */
    private void FirstRope()
    {
        rigidBodyOfBall.useGravity = false;
        rigidBodyOfBall.mass = .1f;
        RaycastHit hitForBeginning;
        Physics.Raycast(origin, ball.transform.up, out hitForBeginning);
        if(hitForBeginning.collider != null)
        {
            SpringJoint firstRope = ball.AddComponent<SpringJoint>();
            firstRope.autoConfigureConnectedAnchor = false;
            firstRope.damper = 30f;
            firstRope.enableCollision = true;
            firstRope.connectedAnchor = hitForBeginning.point;
            firstRope.spring = 1.5f;
            GameObject.DestroyImmediate(rope);
            rope = firstRope;
            firstRopeConnected = true;
        }
    }
    /*
     * It swings ball until game starts
     */
    private void Teeter()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameStarted = true;
            FindObjectOfType<GameOverCanvas>().GetComponent<Canvas>().enabled = false;
            FindObjectOfType<GameCanvas>().GetComponent<Canvas>().enabled = false;
            FindObjectOfType<InGameCanvas>().GetComponent<Canvas>().enabled = true;
            FindObjectOfType<PointDisplay>().TakePosition();
            FindObjectOfType<Motion>().TakePosition();
        }
        else
        {
            rigidBodyOfBall.velocity = (Mathf.PingPong(Time.time * initialSwingingSpeed, initialSwingingDistance)
                                            - (initialSwingingDistance/2)) 
                                            * Vector3.Cross(ball.transform.right, ball.transform.up);
        }
        
    }
    /*
     * When player touch on screen, this method is called and the rope is slinged to wall
     */
    private void Sling(RaycastHit aim)
    {
        Vector3 targetPos = new Vector3(aim.transform.position.x,
                                        aim.transform.position.y - aim.transform.localScale.y/2,
                                        aim.transform.position.z);
        Vector3 directionOfRope = targetPos - origin;
        RaycastHit hit;
        Physics.Raycast(origin, directionOfRope, out hit, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            rigidBodyOfBall.useGravity = false;
            rigidBodyOfBall.mass = 0.1f;
            rigidBodyOfBall.drag = 0f;
            NewRope(hit);
        }
    }
    /*
     * It creates a rope(SpringJoint)
     */
    private void NewRope(RaycastHit hit)
    {
        SpringJoint newRope = ball.AddComponent<SpringJoint>();
        newRope.autoConfigureConnectedAnchor = false;
        newRope.spring = 4.5f;//mesafe başına çekim gücü
        newRope.damper = 25f;//salınımı kesmeye yarar
        newRope.enableCollision = true;
        newRope.connectedAnchor = hit.point;
        GameObject.DestroyImmediate(rope);
        rope = newRope;
    }
    /*
     * During player touching on screen, this function is called to move player
     */
    private void BallForce()
    {
        rigidBodyOfBall.AddForce(Vector3.Cross(-transform.right, (origin - rope.connectedAnchor).normalized) * ballSpeed * Time.deltaTime);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
