using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class editor_mesh : Editor
{
    [MenuItem("GameObject/<MeshFilter>邻接数据填充到顶点颜色")]
    public static void ModifyMesh()
    {
        var mf = Selection.activeGameObject.GetComponent<MeshFilter>();
        Debug.Log("顶点总数" + mf.mesh.vertexCount + "三角总数" + mf.mesh.triangles.Length);
        Debug.Log("源文件顶点总数" + mf.sharedMesh.vertexCount + "源文件三角总数" + mf.sharedMesh.triangles.Length);
        
        Vector3[] vertices = mf.mesh.vertices;
        int[] triangles = mf.mesh.triangles; // 里面是控制点索引
        List<Vector3> verticesN = new List<Vector3>();
        List<int> trianglesN = new List<int>();
        List<Vector3> normalsN = new List<Vector3>();
        List<Vector3> verticesA = new List<Vector3>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            // 获取顶点位置
            Vector3 vertex1 = vertices[index1];
            Vector3 vertex2 = vertices[index2];
            Vector3 vertex3 = vertices[index3];

            Vector3 a1 = new Vector3(0, 0, 0);
            Vector3 a2 = new Vector3(0, 0, 0);
            Vector3 a3 = new Vector3(0, 0, 0);
            
            for (int j = 0; j < triangles.Length; j += 3)
            {
                int jndex1 = triangles[j];
                int jndex2 = triangles[j + 1];
                int jndex3 = triangles[j + 2];

                if (index1 == jndex1 && index2 == jndex2 && index3 == jndex3)
                {
                    continue;
                }
                
                #region 下面是邻接判断，并把邻接三角形不在邻边上的点保存到顶点颜色

                if ((vertices[index1] == vertices[jndex1] && vertices[index2] == vertices[jndex2]) ||
                    (vertices[index1] == vertices[jndex2] && vertices[index2] == vertices[jndex1]))
                {
                    a1 = new Vector3(vertices[jndex3].x, vertices[jndex3].y, vertices[jndex3].z);
                }
                else if ((vertices[index1] == vertices[jndex1] && vertices[index2] == vertices[jndex3]) ||
                    (vertices[index1] == vertices[jndex3] && vertices[index2] == vertices[jndex1]))
                {
                    a1 = new Vector3(vertices[jndex2].x, vertices[jndex2].y, vertices[jndex2].z);
                }
                else if ((vertices[index1] == vertices[jndex3] && vertices[index2] == vertices[jndex2]) ||
                    (vertices[index1] == vertices[jndex2] && vertices[index2] == vertices[jndex3]))
                {
                    a1 = new Vector3(vertices[jndex1].x, vertices[jndex1].y, vertices[jndex1].z);
                }
                
                if ((vertices[index1] == vertices[jndex1] && vertices[index3] == vertices[jndex2]) ||
                    (vertices[index1] == vertices[jndex2] && vertices[index3] == vertices[jndex1]))
                {
                    a3 = new Vector3(vertices[jndex3].x, vertices[jndex3].y, vertices[jndex3].z);
                }
                else if ((vertices[index1] == vertices[jndex1] && vertices[index3] == vertices[jndex3]) ||
                    (vertices[index1] == vertices[jndex3] && vertices[index3] == vertices[jndex1]))
                {
                    a3 = new Vector3(vertices[jndex2].x, vertices[jndex2].y, vertices[jndex2].z);
                }
                else if ((vertices[index1] == vertices[jndex3] && vertices[index3] == vertices[jndex2]) ||
                    (vertices[index1] == vertices[jndex2] && vertices[index3] == vertices[jndex3]))
                {
                    a3 = new Vector3(vertices[jndex1].x, vertices[jndex1].y, vertices[jndex1].z);
                }
                
                if ((vertices[index2] == vertices[jndex1] && vertices[index3] == vertices[jndex2]) ||
                    (vertices[index2] == vertices[jndex2] && vertices[index3] == vertices[jndex1]))
                {
                    a2 = new Vector3(vertices[jndex3].x, vertices[jndex3].y, vertices[jndex3].z);
                }
                else if ((vertices[index2] == vertices[jndex1] && vertices[index3] == vertices[jndex3]) ||
                    (vertices[index2] == vertices[jndex3] && vertices[index3] == vertices[jndex1]))
                {
                    a2 = new Vector3(vertices[jndex2].x, vertices[jndex2].y, vertices[jndex2].z);
                }
                else if ((vertices[index2] == vertices[jndex3] && vertices[index3] == vertices[jndex2]) ||
                    (vertices[index2] == vertices[jndex2] && vertices[index3] == vertices[jndex3]))
                {
                    a2 = new Vector3(vertices[jndex1].x, vertices[jndex1].y, vertices[jndex1].z);
                }
                
                #endregion
            }
            
            verticesN.Add(vertex1);
            verticesN.Add(vertex2);
            verticesN.Add(vertex3);
            
            trianglesN.Add(i);
            trianglesN.Add(i + 1);
            trianglesN.Add(i + 2);
            
            normalsN.Add(mf.mesh.normals[triangles[i]]);
            normalsN.Add(mf.mesh.normals[triangles[i + 1]]);
            normalsN.Add(mf.mesh.normals[triangles[i + 2]]);

            verticesA.Add(a1);
            verticesA.Add(a2);
            verticesA.Add(a3);
        }
        
        var colors = from vec in verticesA select new Color(vec.x, vec.y, vec.z);
        mf.mesh.vertices = verticesN.ToArray();
        mf.mesh.triangles = trianglesN.ToArray();
        mf.mesh.normals = normalsN.ToArray();
        mf.mesh.colors = colors.ToArray();
        
        Debug.Log("Mesh修改完成，当前使用" + verticesN.Count + "顶点" + triangles.Length + "三角");
    }
    
    [MenuItem("GameObject/<SkinnedMeshRenderer>邻接数据填充到顶点颜色[效果错误正在修复]")]
    public static void ModifySkinMesh()
    {
        var smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
        // smr.sharedMesh = smr.sharedMesh;
        Debug.Log("顶点总数" + smr.sharedMesh.vertexCount + "三角总数" + smr.sharedMesh.triangles.Length);
        // Debug.Log("源文件顶点总数" + mf.sharedMesh.vertexCount + "源文件三角总数" + mf.sharedMesh.triangles.Length);
        Vector3[] vertices = smr.sharedMesh.vertices;
        int[] triangles = smr.sharedMesh.triangles; // 里面是控制点索引
        List<Vector3> verticesN = new List<Vector3>();
        List<int> trianglesN = new List<int>();
        List<Vector3> normalsN = new List<Vector3>();
        List<Vector3> verticesA = new List<Vector3>();
        for (int i = 0; i < triangles.Length && i < 1000; i += 3)
        {
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            // 获取顶点位置
            Vector3 vertex1 = vertices[index1];
            Vector3 vertex2 = vertices[index2];
            Vector3 vertex3 = vertices[index3];

            Vector3 a1 = new Vector3(0, 0, 0);
            Vector3 a2 = new Vector3(0, 0, 0);
            Vector3 a3 = new Vector3(0, 0, 0);
            
            for (int j = 0; j < triangles.Length; j += 3)
            {
                int jndex1 = triangles[j];
                int jndex2 = triangles[j + 1];
                int jndex3 = triangles[j + 2];

                if (index1 == jndex1 && index2 == jndex2 && index3 == jndex3)
                {
                    continue;
                }
                
                #region 下面是邻接判断，并把邻接三角形不在邻边上的点保存到顶点颜色

                if ((vertices[index1] == vertices[jndex1] && vertices[index2] == vertices[jndex2]) ||
                    (vertices[index1] == vertices[jndex2] && vertices[index2] == vertices[jndex1]))
                {
                    a1 = new Vector3(vertices[jndex3].x, vertices[jndex3].y, vertices[jndex3].z);
                }
                else if ((vertices[index1] == vertices[jndex1] && vertices[index2] == vertices[jndex3]) ||
                    (vertices[index1] == vertices[jndex3] && vertices[index2] == vertices[jndex1]))
                {
                    a1 = new Vector3(vertices[jndex2].x, vertices[jndex2].y, vertices[jndex2].z);
                }
                else if ((vertices[index1] == vertices[jndex3] && vertices[index2] == vertices[jndex2]) ||
                    (vertices[index1] == vertices[jndex2] && vertices[index2] == vertices[jndex3]))
                {
                    a1 = new Vector3(vertices[jndex1].x, vertices[jndex1].y, vertices[jndex1].z);
                }
                
                if ((vertices[index1] == vertices[jndex1] && vertices[index3] == vertices[jndex2]) ||
                    (vertices[index1] == vertices[jndex2] && vertices[index3] == vertices[jndex1]))
                {
                    a3 = new Vector3(vertices[jndex3].x, vertices[jndex3].y, vertices[jndex3].z);
                }
                else if ((vertices[index1] == vertices[jndex1] && vertices[index3] == vertices[jndex3]) ||
                    (vertices[index1] == vertices[jndex3] && vertices[index3] == vertices[jndex1]))
                {
                    a3 = new Vector3(vertices[jndex2].x, vertices[jndex2].y, vertices[jndex2].z);
                }
                else if ((vertices[index1] == vertices[jndex3] && vertices[index3] == vertices[jndex2]) ||
                    (vertices[index1] == vertices[jndex2] && vertices[index3] == vertices[jndex3]))
                {
                    a3 = new Vector3(vertices[jndex1].x, vertices[jndex1].y, vertices[jndex1].z);
                }
                
                if ((vertices[index2] == vertices[jndex1] && vertices[index3] == vertices[jndex2]) ||
                    (vertices[index2] == vertices[jndex2] && vertices[index3] == vertices[jndex1]))
                {
                    a2 = new Vector3(vertices[jndex3].x, vertices[jndex3].y, vertices[jndex3].z);
                }
                else if ((vertices[index2] == vertices[jndex1] && vertices[index3] == vertices[jndex3]) ||
                    (vertices[index2] == vertices[jndex3] && vertices[index3] == vertices[jndex1]))
                {
                    a2 = new Vector3(vertices[jndex2].x, vertices[jndex2].y, vertices[jndex2].z);
                }
                else if ((vertices[index2] == vertices[jndex3] && vertices[index3] == vertices[jndex2]) ||
                    (vertices[index2] == vertices[jndex2] && vertices[index3] == vertices[jndex3]))
                {
                    a2 = new Vector3(vertices[jndex1].x, vertices[jndex1].y, vertices[jndex1].z);
                }
                
                #endregion
            }
            
            verticesN.Add(vertex1);
            verticesN.Add(vertex2);
            verticesN.Add(vertex3);
            
            trianglesN.Add(i);
            trianglesN.Add(i + 1);
            trianglesN.Add(i + 2);
            
            normalsN.Add(smr.sharedMesh.normals[triangles[i]]);
            normalsN.Add(smr.sharedMesh.normals[triangles[i + 1]]);
            normalsN.Add(smr.sharedMesh.normals[triangles[i + 2]]);

            verticesA.Add(a1);
            verticesA.Add(a2);
            verticesA.Add(a3);
        }
        
        var colors = from vec in verticesA select new Color(vec.x, vec.y, vec.z);
        smr.sharedMesh.vertices = verticesN.ToArray();
        smr.sharedMesh.triangles = trianglesN.ToArray();
        smr.sharedMesh.normals = normalsN.ToArray();
        smr.sharedMesh.colors = colors.ToArray();
        
        Debug.Log("Mesh修改完成，当前使用" + verticesN.Count + "顶点" + triangles.Length + "三角");
    }

    [MenuItem("GameObject/<SkinnedMeshRenderer>转为<MeshFilter MeshRenderer>")]
    public static void ModifySkinMeshToMesh()
    {
        var smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
        var mf = Selection.activeGameObject.AddComponent<MeshFilter>();
        var mr = Selection.activeGameObject.AddComponent<MeshRenderer>();
        
        mf.sharedMesh = smr.sharedMesh;
        // mr.materials = smr.materials;
    }

    [MenuItem("GameObject/<MeshFilter>四边面维护数据")]
    public static void QuadMeshDealWith()
    {
        var mf = Selection.activeGameObject.GetComponent<MeshFilter>();
        var m = mf.mesh;
        for (int i = 0; i < 10; i++)
        {
            Debug.LogError(m.vertices[i].x);
        }
        // Debug.Log("顶点总数" + mf.mesh.vertexCount + "三角总数" + mf.mesh.triangles.Length);
        // Debug.Log("源文件顶点总数" + mf.sharedMesh.vertexCount + "源文件三角总数" + mf.sharedMesh.triangles.Length);
        // Debug.Log(m.getvertex);
        // Debug.Log(m.normals.Length);
    }
}
