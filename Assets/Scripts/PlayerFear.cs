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

    private float _fear;
    public float Fear
    {
        get => _fear;
        set
        {
            if (value < _fear || Invincible)
            {
                _fear = Mathf.Clamp(value, 0, maxFear);
                pc.speedFactor = (_fear / maxFear) * (maxSpeedFactor - minSpeedFactor) + minSpeedFactor;
            }
        }
    }

    private float invincibleStartTime;
    public bool Invincible
    {
        get => invincibleStartTime > 0 && Time.time <= invincibleStartTime + invincibleDuration;
        set => invincibleStartTime = (value) ? Time.time : 0;
    }

    private PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Fear > 0)
        {
            Fear -= fearDrainPerSecond * Time.deltaTime;
        }
    }

    public void stun()
    {
        FindObjectOfType<Timer>().freezeTime(1);
        Invincible = true;
    }
}
