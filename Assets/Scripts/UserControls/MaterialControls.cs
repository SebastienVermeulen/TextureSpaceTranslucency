using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialControls : MonoBehaviour
{
    [SerializeField]
    MeshRenderer _meshRenderer = null;
    Material _mat = null;


    [SerializeField]
    Slider _metallicSlider =  null;
    float _metallic = 0;
    public float _metallicAccess 
    {
        set 
        {
            _metallic = value;
            _mat.SetFloat("_Metallic", _metallic);
        }
    }

    [SerializeField]
    Slider _glossinessSlider = null;
    float _glossiness = 0;
    public float _glossinessAccess
    {
        set
        {
            _glossiness = value;
            _mat.SetFloat("_Glossiness", _glossiness);
        }
    }

    [SerializeField]
    Slider _molarMassSlider = null;
    float _molarMass = 0;
    public float _molarMassAccess
    {
        set
        {
            _molarMass = value;
            _mat.SetFloat("_MolarMass", _molarMass);
        }
    }

    [SerializeField]
    Slider _molarAbsorbtivitySlider = null;
    float _molarAbsorbtivity = 0;
    public float _molarAbsorbtivityAccess
    {
        set
        {
            _molarAbsorbtivity = value;
            _mat.SetFloat("_MolarAbsorbtivity", _molarAbsorbtivity);
        }
    }

    [SerializeField]
    Slider _glowPowerSlider = null;
    float _glowPower = 0;
    public float _glowPowerAccess
    {
        set
        {
            _glowPower = value;
            _mat.SetFloat("_GlowPower", _glowPower);
        }
    }

    [SerializeField]
    Slider _glowScaleSlider = null;
    float _glowScale = 0;
    public float _glowScaleAccess
    {
        set
        {
            _glowScale = value;
            _mat.SetFloat("_GlowScale", _glowScale);
        }
    }

    void Start()
    {
        _mat = _meshRenderer.material;

        _metallic = _mat.GetFloat("_Metallic");
        _metallicSlider.value = _metallic;
        _glossiness = _mat.GetFloat("_Glossiness");
        _glossinessSlider.value = _glossiness;
        _molarMass = _mat.GetFloat("_MolarMass");
        _molarMassSlider.value = _molarMass;
        _molarAbsorbtivity = _mat.GetFloat("_MolarAbsorbtivity");
        _molarAbsorbtivitySlider.value = _molarAbsorbtivity;
        _glowPower = _mat.GetFloat("_GlowPower");
        _glowPowerSlider.value = _glowPower;
        _glowScale = _mat.GetFloat("_GlowScale");
        _glowScaleSlider.value = _glowScale;
    }
}
