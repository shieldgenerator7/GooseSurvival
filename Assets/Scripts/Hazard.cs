using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If it hits something that does not move,
        Rigidbody2D rigidbody2d = collision.gameObject.GetComponent<Rigidbody2D>();
        if (!rigidbody2d)
        {
            //Make it unmovable
            Destroy(GetComponent<Rigidbody2D>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc)
        {
            pc.maxMoveSpeed *= 2f;
            Destroy(gameObject);
        }
    }
}
