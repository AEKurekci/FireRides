using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]public float ballSpeed = 2f;
    [SerializeField] Transform cameraHolder;
    Vector3 tempVec;
    // Start is called before the first frame update
    void Start()
    {
        cameraHolder = Camera.main.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
    }

    private void CameraMovement()
    {

        tempVec.x = cameraHolder.transform.position.x;
        tempVec.y = cameraHolder.transform.position.y;
        tempVec.z = transform.position.z;
        cameraHolder.transform.position = tempVec;
    }
}
