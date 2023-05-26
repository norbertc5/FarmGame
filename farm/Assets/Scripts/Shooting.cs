using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] float crosshairSpread = 20;

    void Update()
    {
        // scale of weapons is independent of player scale
        transform.localScale = new Vector3(1 / transform.parent.localScale.x,
        1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);

        RaycastHit hit;
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Crosshair.spread += crosshairSpread;
            Physics.Raycast(ray, out hit);
            Debug.Log(hit.collider.name);
        }
    }
}
