using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float dampeningRatio = 0.1f;
    public float reverseDragMode = 1.0f;

    public float prev_y = 0;

    SoilManager _soil;

    // Start is called before the first frame update
    void Start()
    {
        _soil = FindObjectOfType<SoilManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            prev_y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            float curr_y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            transform.Translate(reverseDragMode * dampeningRatio * Vector3.up * (curr_y - prev_y));
            prev_y = curr_y;

            if (transform.position.y > 3.0f) transform.position = new Vector3(transform.position.x, 3.0f, transform.position.z);
            if (transform.position.y < 3.0f - _soil.Depth()) transform.position = new Vector3(transform.position.x, 3.0f - _soil.Depth(), transform.position.z);
        }
    }

    public void ReverseDrag(bool value)
    {
        if (value) reverseDragMode = -1.0f;
        else reverseDragMode = 1.0f;

    }
}
