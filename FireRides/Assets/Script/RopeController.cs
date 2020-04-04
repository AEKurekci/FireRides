using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RopeController : MonoBehaviour
{
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RaycastHit aim;
                Physics.Raycast(origin, transform.forward + transform.up, out aim,Mathf.Infinity,layerMask);
                if (aim.transform != null)
                {
                    Sling(aim);
                    FindObjectOfType<whooshSound>().gameObject.GetComponent<AudioSource>().Play();
                }
            }
            else if (Input.GetKey(KeyCode.Space) && rope != null)
            {
                BallForce();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                GameObject.DestroyImmediate(rope);
                rigidBodyOfBall.useGravity = true;
                rigidBodyOfBall.drag = .5f;
                origin = ball.transform.position;
                FindObjectOfType<whooshSound>().gameObject.GetComponent<AudioSource>().Stop();
            }
        }
        

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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RestartGame();
                FindObjectOfType<GameOverCanvas>().gameObject.GetComponent<Canvas>().enabled = false;
            }
            rigidBodyOfBall.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

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

    private void Teeter()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameStarted = true;
            FindObjectOfType<GameOverCanvas>().GetComponent<Canvas>().enabled = false;
            FindObjectOfType<GameCanvas>().GetComponent<Canvas>().enabled = false;
            FindObjectOfType<InGameCanvas>().GetComponent<Canvas>().enabled = true;
        }
        else
        {
            rigidBodyOfBall.velocity = (Mathf.PingPong(Time.time * initialSwingingSpeed, initialSwingingDistance)
                                            - (initialSwingingDistance/2)) 
                                            * Vector3.Cross(ball.transform.right, ball.transform.up);
        }
        
    }

    private void Sling(RaycastHit aim)
    {
        Vector3 targetPos = new Vector3(aim.transform.position.x,
                                        aim.transform.position.y - aim.transform.localScale.y/2,
                                        aim.transform.position.z);
        Vector3 direction = targetPos - origin;
        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            rigidBodyOfBall.useGravity = false;
            rigidBodyOfBall.mass = 0.1f;
            rigidBodyOfBall.drag = 0f;
            NewRope(hit);
        }
    }
    private void NewRope(RaycastHit hit)
    {
        SpringJoint newRope = ball.AddComponent<SpringJoint>();
        newRope.autoConfigureConnectedAnchor = false;
        newRope.spring = 2.5f;//mesafe başına çekim gücü
        newRope.damper = 50f;//salınımı kesmeye yarar
        newRope.enableCollision = true;
        newRope.connectedAnchor = hit.point;
        GameObject.DestroyImmediate(rope);
        rope = newRope;
    }
    private void BallForce()
    {
        rigidBodyOfBall.AddForce(Vector3.Cross(-transform.right, (origin - rope.connectedAnchor).normalized) * ballSpeed * Time.deltaTime);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
