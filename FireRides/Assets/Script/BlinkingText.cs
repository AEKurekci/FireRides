using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    Text txtBlinkingText;
    // Start is called before the first frame update
    void Start()
    {
        txtBlinkingText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.parent.GetComponent<Canvas>().enabled == true)
        {
            txtBlinkingText.color = new Color(255f, 255f, 255f, Mathf.PingPong(Time.time * 0.8f, 0.75F));
        }
    }
}
