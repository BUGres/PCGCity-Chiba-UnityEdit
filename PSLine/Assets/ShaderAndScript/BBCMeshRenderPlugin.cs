using UnityEngine;
using System.Collections.Generic;

public class BBCMeshRenderPlugin : MonoBehaviour
{
    public static List<Color> colors = new List<Color>()
    {
        // new Color(44f / 255f, 44f/ 255f, 44f/255f, 1),
        // new Color(83f / 255f, 104f/ 255f, 135f/255f, 1),
        // new Color(121f / 255f, 142f/ 255f, 163f/255f, 1),
        new Color(150f / 255f, 154f/ 255f, 163f/255f, 1),
        new Color(171f / 255f, 166f/ 255f, 170f/255f, 1),
        new Color(213f / 255f, 206f/ 255f, 194f/255f, 1),
        new Color(219f / 255f, 225f/ 255f, 220f/255f, 1),
        new Color(255f / 255f, 255f/ 255f, 255f/255f, 1),
        
        // 
        new Color(150f / 255f, 154f/ 255f, 163f/255f, 1),
        new Color(171f / 255f, 166f/ 255f, 170f/255f, 1),
        new Color(213f / 255f, 206f/ 255f, 194f/255f, 1),
        new Color(219f / 255f, 225f/ 255f, 220f/255f, 1),
        new Color(255f / 255f, 255f/ 255f, 255f/255f, 1),
        
        new Color(213f / 255f, 206f/ 255f, 194f/255f, 1),
        new Color(219f / 255f, 225f/ 255f, 220f/255f, 1),
        new Color(255f / 255f, 255f/ 255f, 255f/255f, 1),
    };
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var cc = transform.childCount;
        for (int i = 0; i < cc; i++)
        {
            var child = transform.GetChild(i);
            var mr = child.GetComponent<MeshRenderer>();
            mr.material = new  Material(mr.material);
            mr.material.SetColor("_BaseColor", colors[i % colors.Count]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
