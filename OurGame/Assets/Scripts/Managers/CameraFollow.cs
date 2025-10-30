using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f; // 平滑时间（越小跟随越紧）
    public Vector3 offset = new Vector3(0, 0, -5);
    private Vector3 _velocity = Vector3.zero; // 用于存储速度

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z;

        // 平滑阻尼：自动处理加速减速，效果更自然
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _velocity,
            smoothTime
        );
    }
}