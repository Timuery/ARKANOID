using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb2d;

    private Controller controller;
    // Start is called before the first frame update
    void StartSpeed()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.up * speed;
        
    }
    void Start()
    {
        Debug.Log("ball is Spawned");
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }
    void MouseClick()
    {
        if (Input.GetMouseButtonDown(0) && transform.parent !=null)
        {
            StartSpeed();
            controller.gameIsStarted = true;
            transform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MouseClick();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ARKANOID")
        {
            Vector3 paddle = collision.transform.position;
            float paddleWidth = collision.collider.bounds.size.x;

            Vector3 contactPoint = collision.contacts[0].point;

            float different = contactPoint.x - paddle.x;

            float offset = different / (paddleWidth / 2);

            Vector2 direct = new Vector2(offset, 1).normalized;

            rb2d.velocity = direct * speed;

            controller.ZeroCombo();
        }
        if (collision.transform.tag == "block")
        {
            Destroy(collision.gameObject);
            controller.AddCombo();
            controller.AddScore();
            controller.CountBlocks();
        }
        if (collision.transform.name == "RestartZone")
        {
            controller.hpcontroller(-1);
            controller.FindPaddleAndBall();
        }

    }
}
