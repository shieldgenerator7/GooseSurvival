﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{

    public float duration;

    private  float startTime;

    public GameObject holeSprite;
    public SweatSpawner poofSpawner;



    // Start is called before the first frame update
    void Start()
    {
        holeSprite.SetActive(false);
        poofSpawner.Play = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime > 0 && Time.time > startTime + duration)
        {
            poofSpawner.Play = false;
            holeSprite.SetActive(true);
            startTime = 0;
        }
    }

    public void playEndScene(Vector2 pos)
    {
        transform.position = new Vector2(pos.x, transform.position.y);
        poofSpawner.Play = true;
        startTime = Time.time;
    }
    public void resetGame()
    {
        FindObjectOfType<PlayerFear>().Fear = 0;
        FindObjectOfType<PlayerFear>().gameObject.SetActive(true);

        holeSprite.SetActive(false);
        poofSpawner.Play = false;

        FindObjectOfType<Timer>().resetTimer();
        FindObjectOfType<Timer>().Play = true;

    }
}
