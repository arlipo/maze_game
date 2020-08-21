using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry.Model;
using UnityEngine;
using System.Linq;

public class Sensor
{
    public Segment Segment { get => segment; }
    public Vector2? IntersectionPoint { get => intersectionPoint; }
    public Vector2? IntersectionPointWithWall { get => intersectionPointWithWall; }
    private Vector2? intersectionPoint;
    private Vector2? intersectionPointWithWall;
    private Segment segment;
    private Polygon floor;
    private Vector2 sensorDirection;
    private float sensorLength;
    private GameObject mainObject;

    private Vector2 CurrentPosition => DimensionsConverter.To2Dpos(mainObject.transform.position);

    public Sensor(GameObject _mainObject, Vector2 pointingAt, Polygon floorPolygon) {
        mainObject = _mainObject;
        var curPos = CurrentPosition;
        segment = Segment.SegmentWithPoints(curPos, pointingAt);
        var sensorsVector = pointingAt - curPos;
        sensorLength = sensorsVector.magnitude;
        sensorDirection = sensorsVector.normalized;
        intersectionPoint = null;
        floor = floorPolygon;
    }

    public void UpdateIntersectionWithPolygon(Polygon polygon) {
        var resPositions = new List<Vector2>();
        var isIntersecting = polygon?.IsIntersectingWithSegment(segment, Segment.defaultAccuracy, out resPositions);
        intersectionPoint = (isIntersecting ?? false) ? (Vector2?) GetNearest(resPositions) : null;
        var wallIsIntersecting = floor.IsIntersectingWithSegment(segment, Segment.defaultAccuracy, out resPositions);
        intersectionPointWithWall = wallIsIntersecting ? (Vector2?) GetNearest(resPositions) : null;
    }

    private Vector2 GetNearest(List<Vector2> positions) {
        if (positions.Count == 0) {
            Debug.LogError("No Positions found");
            return Vector2.zero;
        }
        var from = segment.a;
        var ordered = positions.OrderBy(pos => (pos - from).magnitude);
        return ordered.First();
    }

    public void UpdateSegment() {
        var curPos = CurrentPosition;
        segment.a = curPos;
        segment.b = curPos + sensorDirection * sensorLength;
    }
    public bool IsActive() {
        if (intersectionPoint == null) return false;
        if (intersectionPointWithWall == null) return true;
        var distTointersectionPoint = (intersectionPoint.Value - segment.a).magnitude;
        var distTointersectionPointWithWall = (intersectionPointWithWall.Value - segment.a).magnitude;
        return distTointersectionPoint <= distTointersectionPointWithWall;
    }
}
