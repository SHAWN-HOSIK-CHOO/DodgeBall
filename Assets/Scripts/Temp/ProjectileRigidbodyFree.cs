using System;
using System.Collections;
using UnityEngine;

public class ProjectileRigidbodyFree : MonoBehaviour
{
    [SerializeField] private float        _initialVelocity;
    [SerializeField] private float        _angle;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float        _step;
    [SerializeField] private Transform    _firePoint;

    private void Start()
    {
        _firePoint = this.transform;
    }

    private void Update()
    {
       AimReady();
    }

    public void AimReady()
    {
        Debug.Log("Called update : " + Input.mousePosition);
        Ray        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 direction       = hit.point - _firePoint.position;
            Vector3 groundDirection = new Vector3(direction.x, 0f, direction.z);

            Vector3 targetPos = new Vector3(groundDirection.magnitude, direction.y, 0f);
            float   height    = targetPos.y + targetPos.magnitude / 2f;
            height = Mathf.Max(0.01f, height);
            float angle;
            float v0;
            float time;
            CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);

            DrawPath(groundDirection.normalized,v0, angle, time, _step);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                StartCoroutine(CoMovement(groundDirection.normalized,v0, angle, time));
            }
        }
    }

    private void DrawPath(Vector3 dir,float v0, float angle, float time, float step)
    {
        step                        = Mathf.Max(0.01f, step);
        _lineRenderer.positionCount = (int)( time / step ) + 2;
        int count = 0;

        for (float i = 0f; i < time; i+= step)
        {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle)                  - 0.5f * -Physics.gravity.y * Mathf.Pow(i, 2);
            _lineRenderer.SetPosition(count, _firePoint.position + dir  * x + Vector3.up     * y);
            count++;
        }

        float xFinal = v0 * time * Mathf.Cos(angle);
        float yFinal = v0 * time * Mathf.Sin(angle) - 0.5f * -Physics.gravity.y * Mathf.Pow(time, 2);
        _lineRenderer.SetPosition(count,_firePoint.position + dir * xFinal + Vector3.up * yFinal);
    }

    private float QuadraticEquation(float a, float b, float c, float sign)
    {
        return( -b + sign * Mathf.Sqrt(b * b - 4 * a * c) ) / (2 * a);
    }
    
    private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g  = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = ( -0.5f * g );
        float c = -yt;

        float tPlus = QuadraticEquation(a, b, c, 1);
        float tMin  = QuadraticEquation(a, b, c, -1);
        time  = tPlus > tMin ? tPlus : tMin;
        angle = Mathf.Atan(b * time / xt);
        v0    = b / Mathf.Sin(angle);
    }

    IEnumerator CoMovement(Vector3 dir, float v0, float angle, float time)
    {
        float t = 0f;
        while (t < time)
        {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - ( 1f / 2f ) * -Physics.gravity.y * Mathf.Pow(t, 2);
            transform.position =  _firePoint.position + dir * x + Vector3.up * y;
            t                  += Time.deltaTime;
            yield return null;
        }
    }
}
