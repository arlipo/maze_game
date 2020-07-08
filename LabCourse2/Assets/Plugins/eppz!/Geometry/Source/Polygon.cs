//
// Copyright (c) 2017 Geri Borbás http://www.twitter.com/_eppz
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace EPPZ.Geometry.Source
{


	public class Polygon : MonoBehaviour
	{


		[UnityEngine.Serialization.FormerlySerializedAs("pointTransforms")]
		public Transform[] points;
		public float offset = 0.0f;

		public enum UpdateMode { Awake, Update, LateUpdate };
		public UpdateMode update = UpdateMode.Awake;	

		public enum Coordinates { World, Local }
		public Coordinates coordinates = Coordinates.World;	

		Model.Polygon _polygon;
		Model.Polygon _offsetPolygon;		
		public Model.Polygon polygon { get { return (offset != 0.0f) ? _offsetPolygon : _polygon; } }

		void Awake()
		{
			if (points == null) return;
			RecheckPoints();
			// Construct a polygon model from transforms (if not created by a root polygon already).
			InitPolygons();
		}

		void InitPolygons() {
			if (_polygon == null) _polygon = Model.Polygon.PolygonWithSource(this);
			UpdateOffsetPolygon();
		}

		public void Init(int pointsCount) {
			SetNewPointsCount(pointsCount);
			InitPolygons();
		}

		void Update()
		{
			if (update == UpdateMode.Update)
			{ UpdateModel(); }
		}

		void LateUpdate()
		{
			if (update == UpdateMode.LateUpdate)
			{ UpdateModel(); }
		}

		void UpdateOffsetPolygon() {
			if (offset != 0.0f) _offsetPolygon = _polygon.OffsetPolygon(offset);
		}

		public void UpdateModel()
		{
			// Update polygon model with transforms, also update calculations.
			_polygon.UpdatePointPositionsWithSource(this);
			UpdateOffsetPolygon();
		}

		public void AddPolygon(Model.Polygon polygonToAdd) {
			_polygon.AddPolygon(polygonToAdd);
			UpdateOffsetPolygon();
		}

		public void SubtractPolygon(Model.Polygon polygonToSubtract) {
			_polygon = _polygon.SubtractPolygon(polygonToSubtract);
			UpdateOffsetPolygon();
		}

		void SetNewPointsCount(int count) {
			bool needCheck;
			if (points == null) {
				points = new Transform[count];
				needCheck = true;
			} else {
				var newPoints = new Transform[count];
				var oldPointsList = new List<Transform>(points);
				var i = 0;
				needCheck = true;
				foreach (var point in points)
				{
					if (i >= count) {
						Destroy(point.gameObject);
						needCheck = false;
					} else {
						newPoints[i] = point;
					}
					oldPointsList.Remove(point);
					i++;
				}
				points = newPoints;
			}
			if (needCheck) RecheckPoints();
		}

		void RecheckPoints() {
			if (points.Any(point => point == null)) {
				for (int i = 0; i < points.Length; i++)
				{
					if (points[i] == null) {
						var go = new GameObject("Point" + i);
						go.transform.parent = this.transform;
						points[i] = go.transform;
						points[i].position += new Vector3(i, 0, 0);
					}
				}
			}
		}
	}
}
