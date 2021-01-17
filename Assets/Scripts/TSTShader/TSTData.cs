using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSTData : MonoBehaviour
{
    [SerializeField]
    MeshFilter _filter = null;
    [SerializeField]
    MeshRenderer _renderer = null;
    Mesh _mesh = null;
    Material _TSTMaterial = null;

    ComputeBuffer verts;
    ComputeBuffer tris;

    [SerializeField]
    float _InverseNormalStepSize = 0.01f;

    //bool _couldInit = false;

    void Start()
    {
        if (!_filter)
        {
            Debug.LogError("Mesh filter was not assigned!");
            return;
        }
        if (!_renderer)
        {
            Debug.LogError("Mesh renderer was not assigned!");
            return;
        }

        _TSTMaterial = _renderer.material;
        _mesh = _filter.mesh;

        if (!_TSTMaterial)
        {
            Debug.LogError("Material was not found!");
            return;
        }
        if (!_mesh)
        {
            Debug.LogError("Mesh was not found!");
            return;
        }



        Vector3[] vertsArray = _mesh.vertices;
        verts = new ComputeBuffer(vertsArray.Length, 3 * sizeof(float));
        verts.SetData(vertsArray);
        _TSTMaterial.SetBuffer("_Verts", verts);
        _TSTMaterial.SetInt("_VertSize", vertsArray.Length);

        int[] trisArray = _mesh.triangles;
        tris = new ComputeBuffer(trisArray.Length, 3 * sizeof(int));
        tris.SetData(trisArray);
        _TSTMaterial.SetBuffer("_Tris", tris);
        _TSTMaterial.SetInt("_TrisSize", trisArray.Length);



        //Set step distance variable
        _TSTMaterial.SetFloat("_InverseNormalStepSize", _InverseNormalStepSize);



        //Finished init
        //_couldInit = true;
    }
    private void OnDestroy()
    {
        verts.Release();
        tris.Release();
    }

    void Update()
    {
        
    }

    private void OnPostRender()
    {
        
    }
}
