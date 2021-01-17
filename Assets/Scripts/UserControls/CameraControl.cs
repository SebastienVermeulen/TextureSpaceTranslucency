using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    GameObject _rotationPoint = null;
    GameObject _cameraObject = null;
    Camera _camera = null;
    bool _backgroundOn = true;

    Vector3 _rotation = new Vector3(0,0,-20);
    float _cameraDistance = -2.0f;
    float _rotationPointHeight = 0.75f;

    [SerializeField]
    float _rotationSpeed = 1.0f;
    [SerializeField]
    float _movementSpeed = 1.0f;
    [SerializeField]
    float _zoomSpeed = 1.0f;

    void Start()
    {
        _cameraObject = this.gameObject;
        _camera = this.GetComponent<Camera>();
    }

    void Update()
    {
        #region Inputs
        if (Input.GetMouseButton(1)) //Left click
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                _rotation.z += Input.GetAxis("Mouse Y") * _rotationSpeed;
                _rotation.y += Input.GetAxis("Mouse X") * _rotationSpeed;
            }
        }
        if (Input.GetMouseButton(2)) //Middle mouse
        {
            if (Input.GetAxis("Mouse Y") != 0)
            {
                _rotationPointHeight -= Input.GetAxis("Mouse Y") * _movementSpeed;
                _rotationPointHeight = Mathf.Clamp(_rotationPointHeight, 0, 1.15f);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _cameraDistance += -Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
            _cameraDistance = Mathf.Clamp(_cameraDistance, -10, -0.5f);
        }
        #endregion

        #region Visuals
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Cursor.visible = false;
        }
        else 
        {
            Cursor.visible = true;
        }
        #endregion

        #region Movement
        if (_rotationPoint) 
        {
            _rotationPoint.transform.rotation = Quaternion.Euler(_rotation);
            _rotationPoint.transform.localPosition = new Vector3(0, _rotationPointHeight, 0);
        }
        _cameraObject.transform.localPosition = new Vector3(_cameraDistance, 0, 0);
        #endregion
    }

    public void SwitchBackground() 
    {
        _backgroundOn = !_backgroundOn;
        if (_backgroundOn)
        {
            _camera.clearFlags = CameraClearFlags.Skybox;
        }
        else
        {
            _camera.clearFlags = CameraClearFlags.Color;
        }
    }
}
