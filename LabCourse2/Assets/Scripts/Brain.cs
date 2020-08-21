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
        var edges = Circle.Create(this.transform.position, viewArea.radius + 2, sensorsCount);

        for (int i = 0; i < sensorsCount; i++)
        {
            var edge = new Vector2(edges[i].x, edges[i].y);
            sensors[i] = new Sensor(gameObject, edge, floorPolygon);
        }
    }

    void UpdateSensors() {
        for (int i = 0; i < sensorsCount; i++)
        {
            
            sensors[i].UpdateSegment();
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
        var y = zones.transform.position.y + 0.03f;
        foreach (var sensor in sensors)
        {
            if (sensor.IsActive()) {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(DimensionsConverter.To3Dpos(sensor.IntersectionPoint.Value, y), 0.3f);
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawLine(DimensionsConverter.To3Dpos(sensor.Segment.a, y), DimensionsConverter.To3Dpos(sensor.Segment.b, y));
        }
    }

    Vector3 GetCurrentGoalToMove() {
        var currenSensorsList = new List<Sensor>(sensors);
        
        if (forward) {
            var firstSensorIndex = (int) (sensorsCount - sensorsCount / 4) - 1;
            currenSensorsList.RemoveRange(0, firstSensorIndex);
            currenSensorsList.AddRange(sensors.Take(firstSensorIndex));
        } else {
            var firstSensorIndex = (int) (sensorsCount - sensorsCount / 4) + 1;
            currenSensorsList.RemoveRange(0, firstSensorIndex);
            currenSensorsList.Reverse();
            currenSensorsList.AddRange(sensors.Take(firstSensorIndex).Reverse());
        }
        Sensor firstSensor;
        // if (currenSensorsList.Count > 0 && currenSensorsList.Last().IsActive()) {
        //     firstSensor = currenSensorsList.Last();
        //     ChangeDirection();
        // } else 
        firstSensor = currenSensorsList.Find(sensor => sensor.IsActive());
        if (firstSensor == null) return GetTemporaryGoal();
        var interSectionPoint2d = firstSensor.IntersectionPoint.Value;
        var goal3d = new Vector3(interSectionPoint2d.x, transform.position.y, interSectionPoint2d.y);
        return goal3d;
    }

    Vector3 GetTemporaryGoal() {
        var nearestWallIndex = 0;
        float? minDist = null;
        for (int i = 0; i < sensorsCount; i++)
        {
            var sensor = sensors[i];
            var goal = sensor.IntersectionPointWithWall;
            if (goal == null) continue;
            var distToCurrentGoal = (goal.Value - sensor.Segment.a).magnitude;
            if (minDist == null || minDist.Value > distToCurrentGoal) {
                minDist = distToCurrentGoal;
                nearestWallIndex = i;
            }
        }
        int moveIndex;
        if (minDist != null && minDist <= 0.45f) {    
            var qtrOfSensors = sensorsCount / 4;
            if (forward) {
                moveIndex = (nearestWallIndex + qtrOfSensors) % sensorsCount;
            } else {
                var negIndex = nearestWallIndex - qtrOfSensors;
                moveIndex = negIndex < 0 ? sensorsCount + negIndex : negIndex;
            }
        } else moveIndex = nearestWallIndex;
        return DimensionsConverter.To3Dpos(sensors[moveIndex].Segment.b, transform.position.y);
    } 
        

    void ChangeDirection() => forward = !forward;

    GUIStyle guiStyle = new GUIStyle();
    GUIStyle groupStyle = new GUIStyle();
    void OnGUI()
    {
        var width = 130;
        var height = 45;
                Color[] pix = new Color[width*height];
        for(int i = 0; i < pix.Length; i++)
            pix[i] = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        groupStyle.normal.background = result;
        
        GUI.BeginGroup(new Rect(10, 10, width, height), groupStyle);
        GUI.Label(new Rect(10, 10, width, 30), forward ? "forward" : "backward", guiStyle);
        GUI.EndGroup();
    }
}
