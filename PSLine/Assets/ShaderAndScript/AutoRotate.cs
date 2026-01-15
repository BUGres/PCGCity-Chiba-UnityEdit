using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [Header("旋转速度")]
    public float rotateSpeed = 1f;
    private float r = 0;
    void Start()
    {
        r = transform.localRotation.eulerAngles.y;
    }
    
    void Update()
    {
        r += Time.deltaTime * rotateSpeed;
        transform.localRotation = Quaternion.Euler(0, r, 0);
    }
}
