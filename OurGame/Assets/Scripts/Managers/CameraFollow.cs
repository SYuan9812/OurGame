using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f; // ƽ��ʱ�䣨ԽС����Խ����
    public Vector3 offset = new Vector3(0, 0, -5);
    private Vector3 _velocity = Vector3.zero; // ���ڴ洢�ٶ�

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z;

        // ƽ�����᣺�Զ�������ټ��٣�Ч������Ȼ
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _velocity,
            smoothTime
        );
    }
}