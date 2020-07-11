using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject focusTarget;
    public Collider2D boundsColl2D;
    private Bounds bounds;
    public Vector3 offset;

    private Vector2 camSizeWorld;
    private Vector2 camSizeWorldHalf;

    // Start is called before the first frame update
    void Start()
    {
        bounds = boundsColl2D.bounds;
        offset = transform.position - focusTarget.transform.position;
        camSizeWorld = Camera.main.ViewportToWorldPoint(Vector2.one) - Camera.main.ViewportToWorldPoint(Vector2.zero);
        camSizeWorldHalf = camSizeWorld / 2;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 camPos = focusTarget.transform.position + offset;
        camPos.x = Mathf.Clamp(
            camPos.x,
            bounds.min.x+camSizeWorldHalf.x,
            bounds.max.x - camSizeWorldHalf.x
            );
        camPos.y = Mathf.Clamp(
            camPos.y,
            bounds.min.y + camSizeWorldHalf.y,
            bounds.max.y - camSizeWorldHalf.y
            );
        transform.position = camPos;
    }
}
