using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShaderGraph.Serialization;
using Random = System.Random;

public class RoadNetBuilder : MonoBehaviour
{
    public GameObject roadNet;
    public Material roadMaterial;
    public Texture2D roadNetNoise;

    public float Latitude;
    public float Longitude;

    public int mainSeed;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }


    private const float roadRange = 500;
    private const float uvUnit2UnityUnit = 5000;
    private const float offsetWeight = 200;
    
    public Vector3 fromPosWSgetUVOffset(Vector3 vec, Vector2 uvRandomOffset)
    {
        Vector2 offsetUV = new Vector2(vec.x / uvUnit2UnityUnit, vec.z / uvUnit2UnityUnit);
        offsetUV += uvRandomOffset;
        if (offsetUV.x < 0) offsetUV.x += (int)Mathf.Abs(offsetUV.x) + 10;
        if (offsetUV.y < 0) offsetUV.y += (int)Mathf.Abs(offsetUV.y) + 10;
        if (offsetUV.x > 1) offsetUV.x -= (int)offsetUV.x;
        if (offsetUV.y > 1) offsetUV.y -= (int)offsetUV.y;
        var posoffset = new Vector3(
            roadNetNoise.GetPixel((int)(offsetUV.x * roadNetNoise.width), (int)(offsetUV.y * roadNetNoise.height)).r, 
            roadNetNoise.GetPixel((int)((1 - offsetUV.x) * roadNetNoise.width), (int)((1 - offsetUV.y) * roadNetNoise.height)).r, 
            0);
        // vec += posoffset;
        vec.x = (posoffset.x * 2 - 1) * offsetWeight;
        vec.y = 0;
        vec.z = (posoffset.y * 2 - 1) * offsetWeight;
        return vec;
    }

    public GameObject CreateARaod(List<Vector3> posLine, string name)
    {
        GameObject go = new GameObject();
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.startColor = Color.black;
        lr.endColor = Color.black;
        lr.startWidth = 1f;
        lr.endWidth = 1f;
        lr.material = roadMaterial;
        lr.positionCount = posLine.Count;
        lr.SetPositions(posLine.ToArray());
        go.transform.parent = roadNet.transform;
        go.name = name;
        return go;
    }
    
    public GameObject CreateAArea(List<Vector3> posLine, string name)
    {
        GameObject go = new GameObject();
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.startColor = Color.yellow;
        lr.endColor = Color.yellow;
        lr.startWidth = 1f;
        lr.endWidth = 1f;
        lr.material = roadMaterial;
        lr.positionCount = posLine.Count;
        lr.SetPositions(posLine.ToArray());
        go.transform.parent = roadNet.transform;
        go.name = name;
        return go;
    }

    public List<Vector3> lerpRoad(Vector3 startPos, Vector3 endPos, float t, Random rd)
    {
        Vector2 offsetUVRandom = new Vector2(rd.Next(0, 10000) / 10000f, rd.Next(0, 10000) / 10000f);
        List<Vector3> posLine = new List<Vector3>();
        var tarVec = endPos - startPos;
        tarVec.Normalize();
        for (float d = 0; d + t < Vector3.Distance(startPos, endPos); d += t)
        {
            posLine.Add(startPos + tarVec * d * t);
            posLine[^1] += (d / Vector3.Distance(startPos, endPos)) * 
                           (Mathf.Abs(Vector3.Distance(startPos, endPos) - d) / Vector3.Distance(startPos, endPos)) * 
                           fromPosWSgetUVOffset(posLine[^1], offsetUVRandom);
        }
        posLine.Add(endPos);
        return posLine;
    }

    // 生成街道
    public void BuildRoadNetP1()
    {
        System.Random random = new System.Random(mainSeed);

        var csjson = JsonUtility.FromJson<MapCSJson>(Resources.Load<TextAsset>("Json/Map").text);
        foreach(var item in csjson.Items)
        {
            if (item.type == "road")
            {
                List<Vector3> posLine = new List<Vector3>();
                foreach (var pos in item.Points)
                {
                    float x = (float)((pos.X - Latitude) / 180 * 6371000 * 2 * Mathf.PI); // 从经度转为米
                    float y = (float)((pos.Y - Longitude) / 180 * 6371000 * 2 * Mathf.PI);
                    posLine.Add(new Vector3(x, 0, y));
                }

                CreateARaod(posLine, "road" + item.name);
            }
            
            else if (item.type == "area")
            {
                List<Vector3> posLine = new List<Vector3>();
                foreach (var pos in item.Points)
                {
                    float x = (float)((pos.X - Latitude) / 180 * 6371000 * 2 * Mathf.PI);
                    float y = (float)((pos.Y - Longitude) / 180 * 6371000 * 2 * Mathf.PI);
                    posLine.Add(new Vector3(x, 0, y));
                }

                CreateAArea(posLine, "area" + item.name);
            }
        }
    }
    // 生成街道
    public void BuildRoadNetP2()
    {
        
    }

    // 清理街道
    public void ClearNet()
    {
        int itemCount = roadNet.transform.childCount;
        for (int i = 0; i < itemCount; i++)
        {
            GameObject.DestroyImmediate(roadNet.transform.GetChild(0).gameObject);
        }
    }
}

[CustomEditor(typeof(RoadNetBuilder))]
[CanEditMultipleObjects]
public class RoadNetBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle bb = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
        };
        
        
        GUILayout.Label("编辑器功能", bb);
        if (GUILayout.Button("清理并随机种子，之后重新生成全部"))
        {
            (target as RoadNetBuilder).mainSeed = (int)(DateTime.UtcNow.Ticks % 999999);
            (target as RoadNetBuilder)?.ClearNet();
            (target as RoadNetBuilder)?.BuildRoadNetP1();
            (target as RoadNetBuilder)?.BuildRoadNetP2();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("[test]生成街道P1"))
        {
            (target as RoadNetBuilder)?.BuildRoadNetP1();
        }
        if (GUILayout.Button("[test]生成街道P2"))
        {
            (target as RoadNetBuilder)?.BuildRoadNetP2();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("[test]清理街道"))
        {
            (target as RoadNetBuilder)?.ClearNet();
        }
        GUILayout.Label("");
        GUILayout.Label("来自Mono序列化", bb);
        base.OnInspectorGUI();
    }
}