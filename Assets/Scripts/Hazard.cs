using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float duration = 10;

    private float startTime = 0;
    private bool active = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startTime > 0)
        {
            if (Time.time > startTime + duration)
            {
                if (active)
                {
                    active = false;
                    despawn();
                }
                else
                {
                    if (Time.time > startTime + duration + 2)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If it hits something that does not move,
        Rigidbody2D rigidbody2d = collision.gameObject.GetComponent<Rigidbody2D>();
        if (!rigidbody2d)
        {
            //Make it unmovable
            Destroy(GetComponent<Rigidbody2D>());
            startTime = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }
        PlayerFear fear = collision.gameObject.GetComponent<PlayerFear>();
        if (fear)
        {
            if (!fear.Invincible)
            {
                fear.Fear += 1;
                fear.stun();
            }
            despawn();
        }
    }

    private void despawn()
    {
        new List<Collider2D>(GetComponents<Collider2D>())
            .ForEach(coll => Destroy(coll));
        gameObject.AddComponent<Rigidbody2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 0.5f;
        sr.color = color;
        //Destroy shadow
        Shadow shadow = GetComponentInChildren<Shadow>();
        if (shadow)
        {
            Destroy(shadow.gameObject);
        }
    }
}
