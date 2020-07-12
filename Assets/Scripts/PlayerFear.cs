using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFear : MonoBehaviour
{
    public float maxFear = 10;
    public float minSpeedFactor = 1;
    public float maxSpeedFactor = 4;
    public float fearDrainPerSecond = 0.2f;
    public float invincibleDuration = 1;

    public BarUpdater barUpdater;
    public SweatSpawner sweatSpawner;

    private float _fear;
    public float Fear
    {
        get => _fear;
        set
        {
            if (value < _fear || !Invincible)
            {
                _fear = Mathf.Clamp(value, 0, maxFear);
                pc.speedFactor = (_fear / maxFear) * (maxSpeedFactor - minSpeedFactor) + minSpeedFactor;
                barUpdater.setValue(_fear, maxFear);
            }
            sweatSpawner.Play = (_fear > 0);
            if (_fear >= maxFear)
            {
                FindObjectOfType<GameOver>().playEndScene(transform.position);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private float invincibleStartTime;
    public bool Invincible
    {
        get => invincibleStartTime > 0 && Time.time <= invincibleStartTime + invincibleDuration;
        set => invincibleStartTime = (value) ? Time.time : 0;
    }

    private int shouldStun = -1;

    private PlayerController pc;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        Fear = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Fear > 0)
        {
            Fear -= fearDrainPerSecond * Time.deltaTime;
        }
        if (invincibleStartTime > 0)
        {
            if (!Invincible)
            {
                Invincible = false;
            }
            if (Time.time > invincibleStartTime + 0.2f)
            {
                animator.SetBool("stunned", false);
            }
        }
        if (shouldStun >= 0)
        {
            if (shouldStun == 0)
            {
                FindObjectOfType<Timer>().freezeTime(0.5f);
                Invincible = true;
            }
            shouldStun--;
        }

    }

    public void stun()
    {
        animator.SetBool("stunned", true);
        shouldStun = 3;
    }
}
