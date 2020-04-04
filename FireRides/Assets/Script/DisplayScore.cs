using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour
{
    Text txtScore;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        txtScore = GetComponent<Text>();
        player = FindObjectOfType<Ball>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        txtScore.text = (player.transform.position.z / 20).ToString("F1");
    }
}
