using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Motion : MonoBehaviour
{
    TextMeshProUGUI txtPoint;
    [SerializeField] [Range(0, 1)] float textSpeed = 1f;
    Coroutine moveCor;
    Vector3 textPos;
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
