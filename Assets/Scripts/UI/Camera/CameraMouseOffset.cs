using Cinemachine;
using UnityEngine;

public class CameraMouseOffset : MonoBehaviour
{
    [Header("Mouse Offset Settings")]
    [SerializeField] private float mouseInfluence = 1.5f;
    [SerializeField] private float maxOffset = 3f;
    [SerializeField] private float smoothSpeed = 5f;

    private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer framingTransposer;
    private Vector3 currentOffset;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        framingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        if (framingTransposer == null) return;

        Vector3 mouseViewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        Vector2 mouseOffset = new(
            (mouseViewport.x - 0.5f) * mouseInfluence,
            (mouseViewport.y - 0.5f) * mouseInfluence
        );

        mouseOffset = Vector2.ClampMagnitude(mouseOffset, maxOffset);

        Vector3 targetOffset = new(mouseOffset.x, mouseOffset.y, 0);
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);

        framingTransposer.m_TrackedObjectOffset = currentOffset;
    }
}