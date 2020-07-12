using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{

    public List<HazardSpawnData> hazards;

    public float spawnDelay = 5;

    private float lastSpawnTime;
    private float totalChance;

    private Timer timer;
    private Bounds spawnBounds;

    // Start is called before the first frame update
    void Start()
    {
        lastSpawnTime = Time.time;
        timer = FindObjectOfType<Timer>();
        totalChance = hazards.Sum(hazard => hazard.commonality);
        spawnBounds = GetComponent<Collider2D>().bounds;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastSpawnTime + spawnDelay)
        {
            lastSpawnTime += spawnDelay;
            int itemCount = 3;
            for (int i = 0; i < itemCount; i++)
            {
                spawnRandomItem();
            }
        }
        if (timer.TimeElapsed % 10 < Time.deltaTime)
        {
            float min = 0.5f;
            float max = 5;
            float maxTime = 180;
            spawnDelay = (1 - (timer.TimeElapsed / maxTime)) * (max - min) + min;
        }
    }

    void spawnRandomItem()
    {
        float randFloat = Random.Range(0, totalChance);
        foreach (HazardSpawnData hazard in hazards)
        {
            if (timer.TimeElapsed >= hazard.timeFloor)
            {
                if (randFloat <= hazard.commonality)
                {
                    spawnItem(hazard.hazardPrefab);
                    break;
                }
            }
            randFloat -= hazard.commonality;
        }
    }

    void spawnItem(GameObject hazard)
    {
        hazard = Instantiate(hazard);
        float randX = Random.Range(
            spawnBounds.min.x,
            spawnBounds.max.x
            );
        float randY = Random.Range(
            spawnBounds.min.y,
            spawnBounds.max.y
            );
        hazard.transform.position = new Vector2(
            randX,
            randY
            );
    }
}
