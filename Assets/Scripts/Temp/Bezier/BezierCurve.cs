using System;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Transform From;
    public Transform To;
    public Transform CP;

    private void SetTargetLocation(Vector3 newTo)
    {
        To.position = newTo;
    }

    private void SetCPLocation(Vector3 newCP)
    {
        CP.position = newCP;
    }

    public void SetFromLocation(Transform newFrom)
    {
        From = newFrom;
    }

    public void SetCurveAuto(Transform newFrom)
    {
        From        = newFrom;
        To.position = From.position + new Vector3(0f, 0f, 6f);
        CP.position = From.position + new Vector3(0f, 4f, 3f);
    }
    
    public Vector3 EvaluateCurve(float t)
    {
        Vector3 ac = Vector3.Lerp(From.position, CP.position, t);
        Vector3 cb = Vector3.Lerp(CP.position,   To.position, t);
        return Vector3.Lerp(ac, cb, t);
    }

    private void OnDrawGizmos()
    {
        if (From == null || To == null || CP == null)
        {
            Debug.Log("Empty Fields");
            return;
        }

        for (int i = 0; i < 20; i++)
        {
            Gizmos.DrawWireSphere(EvaluateCurve(i / 20f), 0.1f);
        }
    }
}
