using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolygonModel = EPPZ.Geometry.Model.Polygon;
using PolygonSource = EPPZ.Geometry.Source.Polygon;
using EPPZ.Geometry.AddOns;

public class PersonPositionZone : MonoBehaviour
{
    public GameObject floor;
    public GameObject viewArea;
    [Range(0f, 2f)]
    public float updateAccuracy;
    public float secondsToUpdate;
    float secondsPassed = 0;
    public DrawType drawType;
    public float victimSpeed;

    public enum DrawType {
        drawWholePolygon = 0, drawCircleAround = 1, hide = 2
    }
    [HideInInspector]
    public PolygonModel mainPolygon;
    PolygonModel floorPolygon;
    PolygonModel viewPolygon;
    ViewArea viewAreaScript;
    MeshFilter meshFilter;
    Vector3 prevPos;
    Vector3 newPos;
    bool ended = false;
    void Awake()
    {
        InitComponents();
        InitializeMainPolygon();
        UpdatePosition();
        CalculateNewArea();
    }
    void InitializeMainPolygon() {
        PolygonModel biggest = null;
        var resultPolygon = new PolygonModel();
        floorPolygon.EnumeratePolygons(poly => {
            poly.Calculate();
            if (poly.isCCW) resultPolygon.AddPolygon(poly);
            else if (biggest == null || poly.area < biggest.area) biggest = poly;
            else if (Mathf.Abs(biggest.area - poly.area) < Mathf.Abs(biggest.area * 0.05f) && poly.polygonCount < biggest.polygonCount)
                biggest = poly;    
        });
        resultPolygon.AddPolygon(biggest);
        mainPolygon = resultPolygon;
        this.transform.Rotate(90, 0, 0);
    }
    void InitComponents() {
        floorPolygon = floor.GetComponent<PolygonSource>().polygon;
        viewPolygon = viewArea.GetComponent<PolygonSource>().polygon;
        viewAreaScript = viewArea.GetComponent<ViewArea>();
        meshFilter = GetComponent<MeshFilter>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ended) return;
        if (mainPolygon == null) { EndIt(); return; }
        secondsPassed += Time.deltaTime;
        if (Input.GetKeyDown("q")) {
            drawType = drawType == DrawType.hide ? 0 : drawType + 1;
            ReDrawArea();
        }
        UpdatePosition();
        CalculateNewArea();
        if (NewAreaRedrawIsNeeded()) ReDrawArea();
    }

    void EndIt() {
        ended = true;
        meshFilter.mesh = null;
    }

    void UpdatePosition(){
        newPos = viewAreaScript.mainObject.transform.position;
    }

    void CalculateNewArea() {
        mainPolygon = mainPolygon.OffsetPolygon(victimSpeed * Time.deltaTime);
        mainPolygon = mainPolygon.IntersectPolygon(floorPolygon);
        mainPolygon = mainPolygon.SubtractPolygon(viewPolygon);
    }

    void ReDrawArea() {
        prevPos = newPos;
        var polygonToShow = GetPolygonToDraw();
        meshFilter.mesh = polygonToShow?.Mesh(Color.green, TriangulatorType.Dwyer);
    }

    PolygonModel GetPolygonToDraw() {
        switch(drawType) {
            case DrawType.hide: return null;
            case DrawType.drawWholePolygon: return mainPolygon;
            case DrawType.drawCircleAround: return mainPolygon.IntersectPolygon(viewPolygon.OffsetPolygon(2));
            default: return null;
        }
    }
    bool NewAreaRedrawIsNeeded() {
        if (secondsPassed >= secondsToUpdate) {
            secondsPassed = 0;
            return true;
        }
        System.Func<float, float, bool> isIdentical = (pos1, pos2) =>
        {
            return Mathf.Abs(RoundPosition(pos1) - RoundPosition(pos2)) < updateAccuracy;
        };
        var xIsIdentical = isIdentical(newPos.x, prevPos.x);
        var zIsIdentical = isIdentical(newPos.z, prevPos.z);
        return !(xIsIdentical && zIsIdentical);
    }

    private float RoundPosition(float pos1)
    {
        return (float) System.Math.Round(pos1, 1);
    }

    // GUIStyle guiStyle = new GUIStyle();
    // GUIStyle groupStyle = new GUIStyle();
    // void OnGUI()
    // {
    //     var width = 250;
    //     var height = 125;
    //             Color[] pix = new Color[width*height];
    //     for(int i = 0; i < pix.Length; i++)
    //         pix[i] = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    //     Texture2D result = new Texture2D(width, height);
    //     result.SetPixels(pix);
    //     result.Apply();

    //     guiStyle.fontSize = 25;
    //     guiStyle.normal.textColor = Color.white;
    //     groupStyle.normal.background = result;
        
    //     GUI.BeginGroup(new Rect(10, 10, width, height), groupStyle);
    //     GUI.Box(new Rect(0,0,140,140), "Seconds passed: " + secondsPassed, guiStyle);
    //     // GUI.Label(new Rect(10, 25, 200, 30), string.Format("x: {0}, z: {1}", RoundPosition(prevPos.x), RoundPosition(prevPos.z)), guiStyle);
    //     // GUI.Box(new Rect(0, 50, 200, 30), "New position", guiStyle);
    //     // GUI.Label(new Rect(10, 75, 200, 30), string.Format("x: {0}, z: {1}", RoundPosition(newPos.x), RoundPosition(newPos.z)), guiStyle);
    //     GUI.EndGroup();
    // }
}
