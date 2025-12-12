using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class BuildingBuilder : MonoBehaviour
{
    public int mainSeed;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void BuildBuildingP1()
    {
        var linerender = GetComponent<LineRenderer>();
        if (linerender == null)
        {
            Debug.LogError("No line renderer");
            return;
        }

        if (linerender.positionCount <= 2)
        {
            Debug.LogError("No Area");
            return;
        }

        if (linerender.GetPosition(0) != linerender.GetPosition(linerender.positionCount - 1))
        {
            Debug.LogError("No Area");
            return;
        }

        if (GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();
        }
        if (GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }

        List<Vector3> verts = new List<Vector3>();
        List<int> conts = new List<int>();
        float height = 20;

        for (int i = 0; i < linerender.positionCount - 1; i++)
        {
            var posthis = linerender.GetPosition(i);
            
            verts.Add(posthis + Vector3.up * height);
            verts.Add(posthis);
            conts.Add(2 * i);
            conts.Add(2 * i + 1);
            conts.Add(2 * i + 3);
            
            conts.Add(2 * i);
            conts.Add(2 * i + 2);
            conts.Add(2 * i + 3);
        }

        for (int i = 0; i < conts.Count; i++)
        {
            if (conts[i] >= 2 * (linerender.positionCount - 1))
            {
                conts[i] %= 2 * (linerender.positionCount - 1);
            }
        }

        var mf = GetComponent<MeshFilter>();
        var mr = GetComponent<MeshRenderer>();
        
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.SetIndices(conts, MeshTopology.Triangles, 0);
        
        // 3. 应用到MeshFilter
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void BuildBuildingP2()
    {
        
    }

    public void ClearBuilding()
    {
        
    }
}

[CustomEditor(typeof(BuildingBuilder))]
[CanEditMultipleObjects]
public class BuildingBuilderEditor : Editor
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
            (target as BuildingBuilder).mainSeed = (int)(DateTime.UtcNow.Ticks % 999999);
            (target as BuildingBuilder)?.ClearBuilding();
            (target as BuildingBuilder)?.BuildBuildingP1();
            (target as BuildingBuilder)?.BuildBuildingP2();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("[test]生成街道P1"))
        {
            (target as BuildingBuilder)?.BuildBuildingP1();
        }
        if (GUILayout.Button("[test]生成街道P2"))
        {
            (target as BuildingBuilder)?.BuildBuildingP2();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("[test]清理街道"))
        {
            (target as BuildingBuilder)?.ClearBuilding();
        }
        GUILayout.Label("");
        GUILayout.Label("来自Mono序列化", bb);
        base.OnInspectorGUI();
    }
}