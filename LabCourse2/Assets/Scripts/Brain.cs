using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry.Model;
using System.Linq;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public int sensorsCount;
    public GameObject zones;
    public GameObject floor;
    [Range(0f, 5f)]
    public float speed;
    ANN neuralNetwork;
    Sensor[] sensors;
    ViewArea viewArea;
    PersonPositionZone personPositionZone;
    Polygon floorPolygon;
    bool forward;

    /// Awake is called when the script instance is being loaded.
    void Awake()
    {
        sensors = new Sensor[sensorsCount];
        viewArea = zones.GetComponentInChildren<ViewArea>();
        personPositionZone = zones.GetComponentInChildren<PersonPositionZone>();
        floorPolygon = floor.GetComponent<EPPZ.Geometry.Source.Polygon>().polygon;
        forward = true;
        InitializeSensors();
        UpdateSensors();
    }

    void InitializeSensors() {
        for (int i = 0; i < sensorsCount; i++)
        {
            sensors[i] = new Sensor(floorPolygon);
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
        if (Input.GetKeyDown("r")) ChangeDirection();
        UpdateSensors();
        var goal = GetCurrentGoalToMove();
        transform.LookAt(goal);
        var moveDirection = (goal - this.transform.position).normalized;
        transform.position += moveDirection * Time.deltaTime * speed;
    }

    void OnDrawGizmos()
    {
        if (sensors == null || personPositionZone.mainPolygon == null) return;
        System.Func<Vector2, Vector3> to3Dpos = vector2 => 
            new Vector3(vector2.x, zones.transform.position.y + 0.03f, vector2.y);
        foreach (var sensor in sensors)
        {
            if (sensor.IsActive()) {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(to3Dpos(sensor.IntersectionPoint.Value), 0.3f);
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(to3Dpos(sensor.Segment.a), to3Dpos(sensor.Segment.b));
        }
    }

    Vector3 GetCurrentGoalToMove() {
        var currenSensorsList = new List<Sensor>(sensors);
        
        if (forward) {
            var firstSensorIndex = (int) sensorsCount / 2 + 1;
            currenSensorsList.RemoveRange(0, firstSensorIndex);
            currenSensorsList.AddRange(sensors.Take(firstSensorIndex));
        } else {
            currenSensorsList.Reverse();
        }
        Sensor firstSensor;
        if (currenSensorsList.Count > 0 && currenSensorsList.First().IsActive()) {
            firstSensor = currenSensorsList.First();
            ChangeDirection();
        } else firstSensor = currenSensorsList.Find(sensor => sensor.IsActive());
        if (firstSensor == null) return GetTemporaryGoal();
        var interSectionPoint2d = firstSensor.IntersectionPoint.Value;
        var goal3d = new Vector3(interSectionPoint2d.x, transform.position.y, interSectionPoint2d.y);
        return goal3d;
    }

    Vector3 GetTemporaryGoal() => transform.position + Vector3.right;

    void ChangeDirection() => forward = !forward;
}
