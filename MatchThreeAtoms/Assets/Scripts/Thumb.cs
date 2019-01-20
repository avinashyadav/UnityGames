using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thumb : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "ball")
        {
            var ball = other.gameObject.GetComponent<Ball>();
            ball.touched = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "ball")
        {
            var ball = other.gameObject.GetComponent<Ball>();
            ball.touched = false;
        }
    }
}
