using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{

    public BezierSpline spline;
    public LineRenderer lr;

    public int resolution;

    public int pointIndex;
    public Vector3 PIpos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        spline.SetControlPoint(pointIndex, PIpos);
    }

    public void UpdateLine()
    {

    }
}
