using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweatDrop : MonoBehaviour
{
    public float duration = 1;
    private float startTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + duration)
        {
            Destroy(gameObject);
        }
    }
}
