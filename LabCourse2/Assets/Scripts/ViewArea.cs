using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry.AddOns;
using EPPZ.Geometry.Source;
using UnityEngine;

public class ViewArea : MonoBehaviour
{
    public int radius;
    public int fovEdges;
    public GameObject mainObject;
    public bool hide;
    bool prevHide;
    Polygon polygon;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        polygon = GetComponent<Polygon>();
        polygon.Init(fovEdges);
        this.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
        UpdateAreaPosition();
        polygon.UpdateModel();
        HideOrShowPolygon();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("h")) hide = !hide;
        if (HideStatusChanged()) HideOrShowPolygon();
        UpdateAreaPosition();
    }

    bool HideStatusChanged() => hide != prevHide;

    void HideOrShowPolygon() {
        prevHide = hide;
        if (hide) {
            GetComponent<EPPZ.Geometry.Source.Mesh>().enabled = false;
            GetComponent<MeshFilter>().mesh = null;
        } else {
            GetComponent<EPPZ.Geometry.Source.Mesh>().enabled = true;
        }
    }

    void UpdateAreaPosition() {
        var oneAngle = 2 * Mathf.PI / fovEdges;
        for (int i = 0; i < fovEdges; i++)
        {
            var angle = oneAngle * i;
            var x = mainObject.transform.position.x + radius * Mathf.Sin(angle);
            var z = mainObject.transform.position.z + radius * Mathf.Cos(angle);
            polygon.points[i].position = new Vector3(x, z, 0);   
        }
    }
}
