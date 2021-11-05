using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    public Camera _camera;

    public const float _delta = 0.5f;

    public const float _nearDist = 0.1f;

    public const float _farDist = 50f;

    public const float _moveSpeed = 2f;

    public const float _zoomSpeed = 2f;

    private void Awake()
    {
        LookAt(new Vector3(0, 0, 0));
    }

    public void Pan()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 dir = transform.up * zInput + transform.right * xInput;
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }

    public void Zoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float dist = Vector3.Distance(transform.position, _camera.transform.position);

        if (dist <= _nearDist && scrollInput > 0f) return;
        if (dist >= _farDist && scrollInput < 0f) return;

        _camera.transform.position += _camera.transform.forward * scrollInput * _zoomSpeed;
    }

    public void Rotate()
    {
        if (!Input.GetMouseButton(0)) return;

        float xRot = Input.GetAxis("Mouse X") * _moveSpeed;
        float yRot = -Input.GetAxis("Mouse Y") * _moveSpeed;

        transform.Rotate(new Vector3(yRot, xRot, 0));
        float x = transform.rotation.eulerAngles.x;
        float y = transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(x, y, 0);
    }

    public void LookAt(Vector3 target)
    {
        transform.position = target;
    }

    public void Update()
    {
        Pan();
        Zoom();
        Rotate();
    }
}
