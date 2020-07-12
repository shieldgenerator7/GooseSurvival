using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweatSpawner : MonoBehaviour
{
    public GameObject sweatPrefab;

    private bool _play = false;
    public bool Play
    {
        get => _play;
        set
        {
            if (!_play && value)
            {
                lastSpawnTime = 0;
                lastGroupSpawnTime = 0;
                countThisGroup = 0;
            }
            _play = value;
        }
    }
    public float spawnDelay = 0.1f;//spawn delay between sweat within a group
    public float groupSpawnDelay = 0.5f;//spawn delay between groups
    public float initialForce = 2;
    public float angularForce = 5;
    public float angularVariance = 5;

    private float lastSpawnTime;
    private float lastGroupSpawnTime;
    private float countThisGroup = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Play)
        {
            if (countThisGroup < 3)
            {
                if (Time.time > lastSpawnTime + spawnDelay)
                {
                    lastSpawnTime = Time.time;
                    lastGroupSpawnTime = Time.time;
                    spawnDrop();
                    countThisGroup++;
                }
            }
            else if (Time.time > lastGroupSpawnTime + groupSpawnDelay)
            {
                //Spawn next group
                countThisGroup = 0;
            }
        }
    }

    private void spawnDrop()
    {
        GameObject sweat = Instantiate(sweatPrefab);
        sweat.transform.parent = transform;
        sweat.transform.localPosition = Vector3.zero;
        sweat.transform.localScale = Vector3.one;
        sweat.transform.up = Vector2.down;
        Rigidbody2D rb2dSweat = sweat.GetComponent<Rigidbody2D>();
        float dir = (transform.parent) ? transform.parent.localScale.x : transform.localScale.x;
        rb2dSweat.velocity =
            (
                Vector2.up
                + Vector2.right * dir * -1
                    * Random.Range(
                        -1 * angularVariance,
                        angularVariance
                        )
            )
            * initialForce;
        rb2dSweat.angularVelocity = dir * angularForce;
    }
}
