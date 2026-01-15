using System;
using UnityEngine;

public class TestPsRender : MonoBehaviour
{
    public RenderTexture inputrt;
    public ComputeShader computeshader;
    public Material material;
    public Material material2;

    public Texture2D pen;
    public Texture2D noise;

    private RenderTexture rt;
    private int kernelIndex;
    
    void Start()
    {
        rt = new RenderTexture(inputrt.width, inputrt.height, 16);
        rt.enableRandomWrite = true;
        rt.Create();
        
        material.mainTexture = inputrt;
        material2.mainTexture = rt;
        
        kernelIndex = computeshader.FindKernel("CSMain"); // 对应上面的核函数
        
        computeshader.SetTexture(kernelIndex, "Input", inputrt);
        computeshader.SetTexture(kernelIndex, "Result", rt);
        computeshader.SetTexture(kernelIndex, "Pen", pen);
        computeshader.SetTexture(kernelIndex, "Noise", noise);
    }

    void Update()
    {
        computeshader.Dispatch(kernelIndex, rt.width / 8, rt.height / 8, 1);
    }

    private void OnDestroy()
    {
        rt.Release();
    }
}
