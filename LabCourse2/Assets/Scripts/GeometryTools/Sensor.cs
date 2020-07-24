using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry.Model;
using UnityEngine;

public class Sensor
{
    public Segment Segment { get => segment; }
    public bool IsActive { get => intersectionPoint != null; }
    public Vector2? IntersectionPoint { get => intersectionPoint; }
    private Vector2? intersectionPoint;
    private Segment segment;
    public Sensor(Vector2 a, Vector2 b) {
        segment = Segment.SegmentWithPoints(a, b);
        intersectionPoint = null;
    }

    public Sensor() : this(new Vector2(), new Vector2()) {}

    public void UpdateIntersectionWithPolygon(Polygon polygon) {
        Vector2 resPos = new Vector2();
        var isIntersecting = polygon?.IsIntersectingWithSegment(segment, Segment.defaultAccuracy, out resPos);
        intersectionPoint = (isIntersecting ?? false) ? (Vector2?) resPos : null;
    }

    public void UpdateSegment(Vector2 bodyPos, Vector2 targetPos) {
        segment.a = bodyPos;
        segment.b = targetPos;
    }
}
