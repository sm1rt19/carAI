using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    private LineRenderer line;
    private float maxDistance;

    // Start is called before the first frame update
    public void SetBeamMaxDistance(float maxDistance)
    {
        this.maxDistance = maxDistance;
        line = GetComponent<LineRenderer>();
        line.SetPositions(new Vector3[] {
            Vector3.zero,
            Vector3.forward * maxDistance
        });
    }

    public float Measure()
    {
        int layerMask = 1 << 9;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, layerMask))
        {
            line.startColor = Color.red;
            line.endColor = Color.red;
            return hit.distance;
        }
        else
        {
            line.startColor = Color.white;
            line.endColor = Color.white;
            return maxDistance;
        }
    }
}
