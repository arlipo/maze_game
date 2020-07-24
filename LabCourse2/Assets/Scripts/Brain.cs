using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry.Model;
using System.Linq;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public int sensorsCount;
    public GameObject zones;
    ANN neuralNetwork;
    Sensor[] sensors;
    ViewArea viewArea;
    PersonPositionZone personPositionZone;
    bool forward;

    /// Awake is called when the script instance is being loaded.
    void Awake()
    {
        sensors = new Sensor[sensorsCount];
        viewArea = zones.GetComponentInChildren<ViewArea>();
        personPositionZone = zones.GetComponentInChildren<PersonPositionZone>();
        forward = true;
        InitializeSensors();
        UpdateSensors();
    }

    void InitializeSensors() {
        for (int i = 0; i < sensorsCount; i++)
        {
            sensors[i] = new Sensor();
        }
    }

    void UpdateSensors() {
        var edges = Circle.Create(this.transform.position, viewArea.radius + 2, sensorsCount);
        var center = new Vector2(this.transform.position.x, this.transform.position.z);
        for (int i = 0; i < sensorsCount; i++)
        {
            var edge = new Vector2(edges[i].x, edges[i].y);
            sensors[i].UpdateSegment(center, edge);
            sensors[i].UpdateIntersectionWithPolygon(personPositionZone.mainPolygon);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSensors();
        transform.position += transform.forward * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        if (sensors == null || personPositionZone.mainPolygon == null) return;
        System.Func<Vector2, Vector3> to3Dpos = vector2 => 
            new Vector3(vector2.x, this.transform.position.y + 0.03f, vector2.y);
        foreach (var sensor in sensors)
        {
            if (sensor.IsActive) {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(to3Dpos(sensor.IntersectionPoint.Value), 0.3f);
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(to3Dpos(sensor.Segment.a), to3Dpos(sensor.Segment.b));
        }
    }
}
