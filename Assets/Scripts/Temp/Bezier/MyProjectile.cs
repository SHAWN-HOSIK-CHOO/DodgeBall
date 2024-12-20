using System;
using UnityEngine;

public class MyProjectile : MonoBehaviour
{
    public BezierCurve curve;
    public float       speed;

    public bool shouldRun = false;
    
    private float _sampleTime;

    private void Start()
    {
        _sampleTime = 0f;
        curve.SetCurveAuto(this.transform);
    }

    private void Update()
    {
        if (shouldRun)
        {
            RunPath();
        }
    }

    private void RunPath()
    {
        _sampleTime        += Time.deltaTime * speed;
        transform.position =  curve.EvaluateCurve(_sampleTime);
        transform.forward  =  curve.EvaluateCurve(_sampleTime + 0.001f) - transform.position;

        if (_sampleTime >= 1f)
        {
            Debug.Log("Destroyed Projectile");
            Destroy(this.gameObject);
        }
    }
}
