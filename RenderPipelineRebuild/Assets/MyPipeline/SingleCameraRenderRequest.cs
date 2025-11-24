using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SingleCameraRenderRequest : MonoBehaviour
{
    public Camera[] cameras;
    public RenderTexture[] renderTextures;

    void Start()
    {
        // Make sure all data is valid before you start the component
        if (cameras == null || cameras.Length == 0 || renderTextures == null || cameras.Length != renderTextures.Length)
        {
            Debug.LogError("Invalid setup");
            return;
        }

        // Start the asynchronous coroutine
        StartCoroutine(RenderSingleRequestNextFrame());
        
        // Call a method called OnEndContextRendering when a camera finishes rendering
        RenderPipelineManager.endContextRendering += OnEndContextRendering;
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        // Create a log to show cameras have finished rendering
        Debug.Log("All cameras have finished rendering.");
    }

    void OnDestroy()
    {
        // End the subscription to the callback
        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
    }

    IEnumerator RenderSingleRequestNextFrame()
    {
        // Wait for the main camera to finish rendering
        yield return new WaitForEndOfFrame();

        // Enqueue one render request for each camera
        SendSingleRenderRequests();

        // Wait for the end of the frame
        yield return new WaitForEndOfFrame();

        // Restart the coroutine
        StartCoroutine(RenderSingleRequestNextFrame());
    }

    void SendSingleRenderRequests()
    {
        //Iterates over the cameras array.        
        for (int i = 0; i < cameras.Length; i++)
        {
            UniversalRenderPipeline.SingleCameraRequest request =
                new UniversalRenderPipeline.SingleCameraRequest();

            // Check if the active render pipeline supports the render request
            if (RenderPipeline.SupportsRenderRequest(cameras[i], request))
            {
                // Set the destination of the camera output to the matching RenderTexture
                request.destination = renderTextures[i];
                
                // Render the camera output to the RenderTexture synchronously
                RenderPipeline.SubmitRenderRequest(cameras[i], request);

                // At this point, the RenderTexture in renderTextures[i] contains the scene rendered from the point
                // of view of the Camera in cameras[i]
            }
        }
    }
}
