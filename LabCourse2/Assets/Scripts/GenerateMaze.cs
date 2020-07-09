using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ClipperLib;
using EPPZ.Geometry.Source;
using PolygonFactory = EPPZ.Geometry.Model.Polygon;

// [RequireComponent(typeof(MeshFilter))]
public class GenerateMaze : MonoBehaviour {
	public static readonly float positionAccuracy = 1000;
	public GameObject wallPrefab;
	public bool simplified;
	public int size;
	int emptySpacesSize = 8;
	Transform floor;
	Transform walls;
	Polygon floorPolygon;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
	}

	// Use this for initialization
	void Awake () {
		GenerateFloor();
		GenerateWalls();
	}

	/// <summary>
	/// Callback to draw gizmos that are pickable and always drawn.
	/// </summary>
	void OnDrawGizmos()
	{
		for (int i = 0; i <= size; i++)
		{
			for (int j = 0; j <= size; j++)
			{
				Gizmos.DrawSphere(new Vector3(i, this.transform.position.y, j), 0.1f);		
			}
		}
	}

	void GenerateFloor() {
		floor = transform.Find("Floor");
		floorPolygon = floor.GetComponent<Polygon>();
		floorPolygon.Init(4);
		var points = floorPolygon.points;
		var startPosition = this.transform.position + new Vector3(1, 1, 0);
		var floorSize = size - 2;

		for (int i = 0; i < points.Length; i++)
		{
			points[i].position = startPosition;
			switch(i) {
				case 0: startPosition.y += floorSize; break;
				case 1: startPosition.x += floorSize; break;
				case 2: startPosition.y -= floorSize; break;
			}
		}
		floor.transform.Rotate(new Vector3(90, 0, 0));
		floorPolygon.UpdateModel();
	}

    void GenerateWalls()
    {
        var wallsBoolean = GetWallsArray();
        walls = transform.Find("Walls");
		for (int z = 0; z < size; z++)
		{
			for (int x = 0; x < size; x++)
			{
				if (isBorder(x, z)) InstantiateWall(x, z, false);
				else if (wallsBoolean[z][x]) InstantiateWall(x, z);
			}
		}
    }
	bool isBorder(int x, int z) => x == 0 || z == 0 || x == size - 1 || z == size - 1;
	bool isFromStart(int index) => index < emptySpacesSize;
	bool isFromEnd(int index) => index > size - emptySpacesSize;
	bool isCenter(int x, int z) {
		var half = size / 2;
		return x >= half - 1 && x <= half && z >= half - 1 && z <= half;
	}

    bool[][] GetWallsArray()
    {
        var wallsB = new bool[size][];
        for (int z = 0; z < size; z++)
        {
            wallsB[z] = new bool[size];
            for (int x = 0; x < size; x++)
            {
                if (isBorder(x, z)) continue;
                else if (isCenter(x, z)) wallsB[z][x] = true;
                else if (isFromStart(x) && isFromStart(z)) continue;
                else if (isFromEnd(x) && isFromStart(z)) continue;
                else if (isFromStart(x) && isFromEnd(z)) continue;
                else if (isFromEnd(x) && isFromEnd(z)) continue;
                else if (!simplified)
                {
                    System.Func<float, float> abs = num => Mathf.Abs(num);

                    var minWDist = x < abs(x - size) ? x : abs(x - size);
                    var minDDist = z < abs(z - size) ? z : abs(z - size);
                    var totalDist = abs(minDDist - minWDist);

                    var half = size / 2.0f;
                    var chance = totalDist / half;

                    if (Random.Range(0f, 1f) <= chance) wallsB[z][x] = true;
                }
            }
        }
		CheckCorners(wallsB);
        return wallsB;
    }

    void CheckCorners(bool[][] wallsB)
    {
        var fixes = 1;
        while (fixes > 0)
        {
            fixes = 0;
            for (int z = 1; z < size - 2; z++)
            {
                var row = wallsB[z];
                var nextRow = wallsB[z + 1];
                for (int x = 1; x < size - 2; x++)
                {
                    if (row[x] == nextRow[x + 1] && row[x + 1] == nextRow[x] && row[x] != row[x + 1])
                    {
                        if (row[x]) row[x + 1] = true;
                        else row[x] = true;
                        fixes = 1;
                    }
                }
            }
        }
    }

    void InstantiateWall(int x, int z, bool createHole = true) {
		var prefabSize = wallPrefab.GetComponent<Renderer>().bounds.size;
		var xPos = x + this.transform.position.x;
		var zPos = z + this.transform.position.z;
		if (createHole) CreateHole(xPos, zPos, prefabSize);
		var xHalfSize = prefabSize.x / 2;
		var zHalfSize = prefabSize.z / 2;
		var go = Instantiate(
					wallPrefab, 
					new Vector3(
						xPos + xHalfSize, 
						this.transform.position.y,
						zPos + zHalfSize
					), 
					Quaternion.identity
				);
		go.transform.parent = walls;
	}
	private void CreateHole(float x, float z, Vector3 holeSize) {
		var points = new Vector2[4];
		var currentPosition = new Vector2(x, z);

		for (int i = 0; i < points.Length; i++)
		{
			points[i] = currentPosition;
			switch(i) {
				case 0: currentPosition.x += holeSize.x; break;
				case 1: currentPosition.y += holeSize.z; break;
				case 2: currentPosition.x -= holeSize.x; break;
			}
		}
		floorPolygon.SubtractPolygon(PolygonFactory.PolygonWithPoints(points));
	}
}