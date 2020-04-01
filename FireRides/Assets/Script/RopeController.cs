using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [Header("Ball")]
    public GameObject ball;
    private float ballSpeed;
    Rigidbody rigidBodyOfBall;
    Vector3 origin;
    private Transform ballTransform;
    [SerializeField] float rangeOfAim = 100f;
    public LineRenderer lineRenderer;

    private SpringJoint rope;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rigidBodyOfBall = ball.GetComponent<Rigidbody>();
        ballSpeed = ball.GetComponent<Ball>().ballSpeed;
        origin = ball.transform.position;
        ballTransform = ball.transform.parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit aim = FindAim();
            if (aim.transform != null)
            {
                Sling(aim);
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {
            BallForce();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject.DestroyImmediate(rope);
            rigidBodyOfBall.useGravity = true;
            origin = ball.transform.position;
            rigidBodyOfBall.mass = 1f;
        }

    }

    void LateUpdate()
    {
        if (rope != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetPosition(0, ball.transform.position);
            lineRenderer.SetPosition(1, rope.connectedAnchor);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    

    private RaycastHit FindAim()
    {
        RaycastHit aim;
        Physics.Raycast(origin, ballTransform.forward + ballTransform.up, out aim, rangeOfAim);
        Debug.Log(ballTransform.forward + ballTransform.up);
        Debug.Log("origin: "+origin);
        return aim;
    }

    private void Sling(RaycastHit aim)
    {
        
        
        Vector3 targetPos = new Vector3(aim.transform.position.x,
                                        aim.transform.position.y - aim.transform.localScale.y/2,
                                        aim.transform.position.z);
        Vector3 direction = targetPos - origin;

        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit);
        if (hit.collider != null)
        {
            rigidBodyOfBall.useGravity = false;
            rigidBodyOfBall.mass = 0.1f;
            NewRope(hit);
        }
    }

    private void BallForce()
    {
        rigidBodyOfBall.AddForce(Vector3.forward * ballSpeed * Time.deltaTime);
    }

    private void NewRope(RaycastHit hit)
    {
        SpringJoint newRope = ball.AddComponent<SpringJoint>();
        newRope.autoConfigureConnectedAnchor = false;
        newRope.spring = 1.5f;//mesafe başına çekim gücü
        //newRope.tolerance = 0.1f;
        newRope.damper = 40f;//salınımı kesmeye yarar
        newRope.enableCollision = true;
        newRope.connectedAnchor = hit.point;
        GameObject.DestroyImmediate(rope);
        rope = newRope;
    }
}
