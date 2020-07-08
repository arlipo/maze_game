using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPPZ.Geometry.Source;

public class GeneratePoints : MonoBehaviour
{

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    public void Awake()
    {
        var polygon = GetComponent<Polygon>();
        var count = polygon.points.Length;
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("Point" + i);
            go.transform.parent = this.transform;
            var point = polygon.points[i];
            // if (point != null) Destroy(point.gameObject);
            point = go.transform;
        }
    }

    public void RegeneratePoints() {
        
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
