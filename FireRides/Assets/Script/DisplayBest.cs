using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBest : MonoBehaviour
{
    Text txtBest;
    // Start is called before the first frame update
    void Start()
    {
        txtBest = GetComponent<Text>();
        Display();
    }

    public void Display()
    {
        txtBest.text = PlayerPrefs.GetFloat("best").ToString();
    }
}
