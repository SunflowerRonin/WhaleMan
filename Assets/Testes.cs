using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testes : MonoBehaviour
{
    public Rigidbody rb;
    public int clickForce = 500;
    private Plane plane = new Plane(Vector2.up, Vector2.zero);

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float enter;
            if (plane.Raycast(ray, out enter))
            {
                var hitPoint = ray.GetPoint(enter);
                var mouseDir = hitPoint - gameObject.transform.position;
                mouseDir = mouseDir.normalized;
                rb.AddForce(mouseDir * clickForce);
            }
        }
    }
}
