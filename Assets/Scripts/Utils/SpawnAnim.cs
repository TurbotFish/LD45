using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnim : MonoBehaviour
{
    public float duration;
    public AnimationCurve curve;
    public float amplitude;
    float counter;

    private void Start()
    {
        counter = 0;
        StartCoroutine(SpawnScale());
    }

    private IEnumerator SpawnScale ()
    {
        float t = 0;
        while(counter < 1)
        {
            t += Time.deltaTime;
            counter += t/duration;
            transform.localScale = Vector3.one* curve.Evaluate(counter) * amplitude;
            yield return new WaitForEndOfFrame();
        }
    }
}
