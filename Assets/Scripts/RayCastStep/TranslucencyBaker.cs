using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslucencyBaker : MonoBehaviour
{
    struct Triangle 
    {
        public int indicy1;
        public int indicy2;
        public int indicy3;
    }

    [SerializeField]
    TextureBake _baker = null;

    [SerializeField]
    ComputeShader _shader = null;
    [SerializeField]
    Texture _noiseTexture = null;

    [SerializeField]
    MeshFilter _meshFilter = null;
    Mesh _mesh = null;

    //Verticies
    Vector3[] _verts;
    //Verticies
    Vector3[] _norms;
    //Triangles
    Triangle[] _tris;
    //Color
    Color[] _DepthColors;
    Color[] _NormalColors;

    [SerializeField]
    int _nrOfSamples = 1;

    bool _capture = false;

    struct Poly
    {
        int indA;
        int indB;
        int indC;
    };

    private void Start()
    {
        //Find mesh
        if (!_mesh)
        {
            _mesh = _meshFilter.mesh;
        }

        //Create software buffers
        _verts = _mesh.vertices;
        _norms = _mesh.normals;
        _tris = new Triangle[_mesh.triangles.Length / 3];
        for (int idx = 0; idx < _mesh.triangles.Length; idx += 3) 
        {
            _tris[idx / 3].indicy1 = _mesh.triangles[idx];
            _tris[idx / 3].indicy2 = _mesh.triangles[idx + 1];
            _tris[idx / 3].indicy3 = _mesh.triangles[idx + 2];
        }
        _DepthColors = new Color[_mesh.vertices.Length];
        _NormalColors = new Color[_mesh.vertices.Length];

        //Set noise texture
        _shader.SetTexture(_shader.FindKernel("Compute"), "_noiseTexture", _noiseTexture);
    }
    private void Update()
    {
        _capture = false;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _capture = true;
        }
    }

    private void OnPreRender() 
    {
        if (!_capture)
            return;

        //Generate the vertex colors and apply them to the mesh, if the mesh exists
        if (_mesh)
        {
            RunVertColorGen();
        }

        _capture = false;
    }

    void RunVertColorGen()
    {
        Debug.LogWarning("Started vertex color generation.");

        int kernelHandle = _shader.FindKernel("Compute");

        #region Buffers
        //Set all the mesh data
        //Verticies
        ComputeBuffer bufferVert = new ComputeBuffer(_verts.Length, 3 * sizeof(float));
        bufferVert.SetData(_verts);
        _shader.SetBuffer(kernelHandle, "_verts", bufferVert);
        //Normals
        ComputeBuffer bufferNorm = new ComputeBuffer(_norms.Length, 3 * sizeof(float));
        bufferNorm.SetData(_norms);
        _shader.SetBuffer(kernelHandle, "_normals", bufferNorm);
        //Triangles
        ComputeBuffer bufferTria = new ComputeBuffer(_tris.Length, 3 * sizeof(int));
        bufferTria.SetData(_tris);
        _shader.SetBuffer(kernelHandle, "_triangles", bufferTria);

        //Set inverenormal step distance
        _shader.SetFloat("_inverseNormalStepSize", 0.01f);
        _shader.SetInt("_nrOfSamples", _nrOfSamples);

        //Set texture color data
        ComputeBuffer bufferDepth = new ComputeBuffer(_mesh.vertexCount, 4 * sizeof(float));
        _shader.SetBuffer(kernelHandle, "_randomDepth", bufferDepth);        
        ComputeBuffer bufferNormal = new ComputeBuffer(_mesh.vertexCount, 4 * sizeof(float));
        _shader.SetBuffer(kernelHandle, "_randomNormal", bufferNormal);
        #endregion

        #region RunShader
        //Run shader
        int nrToRun = (int)Mathf.Ceil(_verts.Length / 1024.0f);
        _shader.Dispatch(kernelHandle, nrToRun, 1, 1);

        //Get resulting data
        bufferDepth.GetData(_DepthColors);
        _mesh.SetColors(_DepthColors);
        _baker.Bake("_Depth");
        bufferNormal.GetData(_NormalColors);
        _mesh.SetColors(_NormalColors);
        _baker.Bake("_Normal");
        #endregion

        #region BuffersDestruction
        //Release data
        bufferVert.Release();
        bufferNorm.Release();
        bufferTria.Release();
        bufferDepth.Release();
        bufferNormal.Release();
        #endregion

        Debug.LogWarning("Finished vertex color generation.");
    }
}
