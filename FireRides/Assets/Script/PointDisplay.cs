using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointDisplay : MonoBehaviour
{
    /*
     *textSpeed: It defines the point text speed and takes a value between 0.1f and 1f
     * enterCircle: If player enters the collider of circle it will be true
      */
    Text txtPoint;
    [SerializeField] [Range(0.1f, 1f)] float textSpeed = 1f;
    Coroutine moveCor;
    Vector3 textPos;
    bool enterCircle = false;
    // Start is called before the first frame update
    void Start()
    {
        txtPoint = GetComponent<Text>();
        textPos = transform.position;
    }
    private void Update()
    {
        if(enterCircle)
        {
            if (transform.position.y > textPos.y + 50f)
            {
                StopCoroutine(moveCor);
                enterCircle = false;
                Invoke("Delete", 1f);
            }
            else
            {
                moveCor = StartCoroutine(Move());
            }
        }
    }

    private void Delete()
    {
        txtPoint.text = "";
        transform.position = textPos;
    }

    public void TakePosition()
    {
        textPos = transform.position;
    }

    public void ShowPoint(int point)
    {
        txtPoint.text = "+" + point;
        moveCor = StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        enterCircle = true;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        yield return new WaitForSeconds(0.5f / textSpeed); 
    }
}
