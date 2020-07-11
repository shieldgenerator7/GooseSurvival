using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public GameObject caster;//the object that casts this shadow

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
            Vector3 scale = transform.localScale;
            scale.x = casterBounds.size.x / bounds.size.x;
            scale.y = scale.x * 0.10f;
            transform.localScale = scale;
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
        RaycastHit2D[] rch2ds = Physics2D.RaycastAll(caster.transform.position, Vector2.down);
        foreach (RaycastHit2D rch2d in rch2ds)
        {
            if (!rch2d.collider.isTrigger)
            {
                if (rch2d.collider.gameObject != caster)
                {
                    transform.position = rch2d.point;
                    break;
                }
            }
        }
    }
}
