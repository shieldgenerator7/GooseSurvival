using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public GameObject caster;//the object that casts this shadow
    public float minWidthFactor = 0.1f;//the thinnest it can be when the caster is up really high
    public float maxForeseeableHeight = 5;

    private Vector3 defaultScale;

    private float _casterHeight = 0;
    public float CasterHeight
    {
        get => _casterHeight;
        set
        {
            _casterHeight = Mathf.Clamp(value, 0, maxForeseeableHeight);
            Bounds casterBounds = caster.GetComponent<SpriteRenderer>().bounds;
            Bounds bounds = GetComponent<SpriteRenderer>().bounds;
            Vector3 scale = defaultScale;
            scale.x = Mathf.Clamp(
                defaultScale.x * (1 - (_casterHeight / maxForeseeableHeight)),
                0.1f,
                1
                );
            scale.y = scale.x * 0.10f;
            transform.localScale = scale;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!caster)
        {
            if (transform.parent)
            {
                caster = transform.parent.gameObject;
            }
        }
        if (caster)
        {
            //Set size of shadow
            Bounds casterBounds = caster.GetComponent<SpriteRenderer>().bounds;
            Bounds bounds = GetComponent<SpriteRenderer>().bounds;
            defaultScale = transform.localScale;
            defaultScale.x = casterBounds.size.x / bounds.size.x;
            defaultScale.y = defaultScale.x * 0.10f;
            transform.localScale = defaultScale;
            checkGroundDistance();
        }
        else
        {
            //Delete shadow
            Debug.LogError("Shadow (" + gameObject.name + ") cannot find caster! caster: " + caster);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkGroundDistance();
    }

    void checkGroundDistance()
    {
        RaycastHit2D[] rch2ds = Physics2D.RaycastAll(caster.transform.position, Vector2.down);
        foreach (RaycastHit2D rch2d in rch2ds)
        {
            if (!rch2d.collider.isTrigger)
            {
                if (rch2d.collider.gameObject != caster)
                {
                    transform.position = rch2d.point;
                    CasterHeight = caster.transform.position.y - transform.position.y;
                    break;
                }
            }
        }
    }
}
