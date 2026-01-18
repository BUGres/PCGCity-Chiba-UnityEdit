using UnityEngine;
using UnityEditor;
using System.IO;

public class editor_capture : Editor
{
    [MenuItem("Tools/截屏/快速截屏")]
    public static void TakeScreenshot()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Screenshot_{timestamp}.png";
        string path = Path.Combine(Application.dataPath, fileName);
        
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log($"截屏已保存: {path}");
        
        AssetDatabase.Refresh();
    }
}