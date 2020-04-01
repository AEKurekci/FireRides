using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineExample : MonoBehaviour
{
    [SerializeField]Transform targetPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 forwardUp = transform.TransformDirection(Vector3.forward + Vector3.up) * 10;
            if(Physics.Linecast(transform.forward + transform.up, -targetPos.up))
            {
                Debug.DrawRay(transform.position, forwardUp, Color.blue);
                Debug.Log("draw");
            }
        }
    }
}
