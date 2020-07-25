using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry.Model;
using UnityEngine;
using System.Linq;

public class Sensor
{
    public Segment Segment { get => segment; }
    public Vector2? IntersectionPoint { get => intersectionPoint; }
    private Vector2? intersectionPoint;
    private Vector2? intersectionPointWithWall;
    private Segment segment;
    private Polygon floor;
    public Sensor(Vector2 a, Vector2 b, Polygon floorPolygon) {
        segment = Segment.SegmentWithPoints(a, b);
        intersectionPoint = null;
        floor = floorPolygon;
    }

    public Sensor(Polygon floorPolygon) : this(new Vector2(), new Vector2(), floorPolygon) {}

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

    public void UpdateSegment(Vector2 bodyPos, Vector2 targetPos) {
        segment.a = bodyPos;
        segment.b = targetPos;
    }
    public bool IsActive() {
        if (intersectionPoint == null) return false;
        if (intersectionPointWithWall == null) return true;
        var dirTointersectionPoint = (intersectionPoint.Value - segment.a).magnitude;
        var dirTointersectionPointWithWall = (intersectionPointWithWall.Value - segment.a).magnitude;
        return dirTointersectionPoint <= dirTointersectionPointWithWall;
    }
}
