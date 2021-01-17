using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightsController : MonoBehaviour
{
    [SerializeField]
    GameObject _light1 = null;
    [SerializeField]
    GameObject _light2 = null;
    [SerializeField]
    GameObject _light3 = null;
    [SerializeField]
    GameObject _light4 = null;
    [SerializeField]
    GameObject _lightDirectional = null;

    Vector3 _lightStartPos1;
    Vector3 _lightStartPos2;
    Vector3 _lightStartPos3;
    Vector3 _lightStartPos4;
    float _lightTime = 0;

    [SerializeField]
    Slider _lightMovementSpeedSlider = null;
    float _lightMovementSpeed = 0.6f;
    public float _lightMovementSpeedAccess
    {
        set
        {
            _lightMovementSpeed = value;
        }
    }

    Vector3 _lightStartRotDirectional;
    float _directionalTime = 0;

    [SerializeField]
    Slider _directionalRotationSpeedSlider = null;
    float _directionalRotationSpeed = 0.2f;
    public float _directionalRotationSpeedAccess
    {
        set
        {
            _directionalRotationSpeed = value;
        }
    }

    bool _movementLightsEnabled = false;
    bool _movementDirectionalEnabled = false;

    private void Start()
    {
        _lightMovementSpeedSlider.value = _lightMovementSpeed;
        _directionalRotationSpeedSlider.value = _directionalRotationSpeed;

        _lightStartPos1 = _light1.transform.localPosition;
        _lightStartPos2 = _light2.transform.localPosition;
        _lightStartPos3 = _light3.transform.localPosition;
        _lightStartPos4 = _light4.transform.localPosition;
        _lightStartRotDirectional = _lightDirectional.transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        if (_movementLightsEnabled) 
        {
            _light1.transform.localPosition = _lightStartPos1 + 
                0.3f * new Vector3(
                    Mathf.Sin(1 * _lightTime), 
                    Mathf.Cos(2 * _lightTime - Mathf.PI / 2.0f), 
                    Mathf.Sin(3 * _lightTime));
            _light2.transform.localPosition = _lightStartPos2 +
                _lightMovementSpeed * 0.3f * new Vector3(
                    Mathf.Sin(2 * _lightTime),
                    Mathf.Cos(1 * _lightTime - Mathf.PI / 2.0f),
                    Mathf.Sin(3 * _lightTime));
            _light3.transform.localPosition = _lightStartPos3 +
                _lightMovementSpeed * 0.3f * new Vector3(
                    Mathf.Sin(3 * _lightTime),
                    Mathf.Cos(1 * _lightTime - Mathf.PI / 2.0f),
                    Mathf.Sin(2 * _lightTime));
            _light4.transform.localPosition = _lightStartPos4 +
                _lightMovementSpeed * 0.3f * new Vector3(
                    Mathf.Sin(3 * _lightTime),
                    Mathf.Cos(2 * _lightTime - Mathf.PI / 2.0f),
                    Mathf.Sin(1 * _lightTime));

            _lightTime += _lightMovementSpeed * Time.deltaTime;
        }
        if (_movementDirectionalEnabled)
        {
            _lightDirectional.transform.localRotation = Quaternion.Euler(
                _lightStartRotDirectional + 180 * new Vector3(
                    Mathf.Sin(1 * _directionalTime),
                    Mathf.Cos(2 * _directionalTime - Mathf.PI / 2.0f),
                    Mathf.Sin(3 * _directionalTime)));

            _directionalTime += _directionalRotationSpeed * Time.deltaTime;
        }
    }

    public void ToggleMovementLights() 
    {
        _movementLightsEnabled = !_movementLightsEnabled;
    }
    public void ToggleMovementDirectional() 
    {
        _movementDirectionalEnabled = !_movementDirectionalEnabled;
    }
}
