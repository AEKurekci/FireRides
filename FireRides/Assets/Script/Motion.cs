using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Motion : MonoBehaviour
{
    /*
     * txtPoint: Text element of point
     * textSpeed: Speed of text movement
     * textPos: The actual position of text element
     * moveCor: movement coroutine
     * enterCircle: If ball enters the circle, it will turn true
     */
    TextMeshProUGUI txtPoint;
    [SerializeField] [Range(0, 1)] float textSpeed = 1f;
    Vector3 textPos;
    Coroutine moveCor;
    bool enterCircle = false;
    // Start is called before the first frame update
    void Start()
    {
        txtPoint = GetComponent<TextMeshProUGUI>();
        textPos = transform.position;
    }
    private void Update()
    {
        if (enterCircle)
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

    public void ShowPoint(string message)
    {
        txtPoint.text = message;
        moveCor = StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        enterCircle = true;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        yield return new WaitForSeconds(0.5f / textSpeed);
    }
}
