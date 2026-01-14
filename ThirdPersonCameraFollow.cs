using UnityEngine;

public class ThirdPersonCameraFollow : MonoBehaviour
{
    public Transform target;          // Player
    public Vector3 offset = new Vector3(0f, 1.6f, -4f);
    public float followSpeed = 8f;
    public float rotateSpeed = 10f;

    void LateUpdate()
    {
        if (!target) return;

        // Follow position
        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // Follow rotation
        Quaternion desiredRotation = Quaternion.LookRotation(
            target.position - transform.position,
            Vector3.up
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotateSpeed * Time.deltaTime
        );
    }
}
