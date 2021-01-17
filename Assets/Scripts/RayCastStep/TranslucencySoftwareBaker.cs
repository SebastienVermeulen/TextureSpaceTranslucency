using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslucencySoftwareBaker : MonoBehaviour
{
    [SerializeField]
    TextureBake _baker = null;

    [SerializeField]
    MeshFilter _meshFilter = null;
    Mesh _mesh = null;

    //Verticies
    Vector3[] _verts;
    //Verticies
    Vector3[] _norms;
    //Triangles
    int[] _tris;

    //Color
    Color[] _colors;

    [SerializeField]
    float _distanceDiv = 1.0f;

    bool _capture = false;
    bool _startedCapture = false;
    bool _stepThrough = true;
    bool _computing = false;

    struct Poly
    {
        int indA;
        int indB;
        int indC;
    };

    private void Start()
    {
        if (!_mesh)
        {
            _mesh = _meshFilter.mesh;
        }
     
        _colors = new Color[_mesh.vertexCount];

        //Verticies
        _verts = _mesh.vertices;
        //Verticies
        _norms = _mesh.normals;
        //Triangles
        _tris = _mesh.triangles;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _capture = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _stepThrough = !_stepThrough;
        }
    }

    private void OnPreRender()
    {
        if (!_capture || _capture && _computing)
            return;

        //Generate the vertex colors and apply them to the mesh, if the mesh exists
        if (_mesh && !_startedCapture)
        {
            _startedCapture = true;
            StartCoroutine(RunVertColorGen());
        }
    }

    IEnumerator RunVertColorGen()
    {
        Debug.LogWarning("Started vertex color generation.");

        for (int idx = 0; idx < _verts.Length; idx++) 
        {
            //Run intersection code
            Compute(idx);

            //Step through if asked by the user
            #region CoroutineStep
            if (_stepThrough)
            {
                Debug.Log(idx.ToString() + " Vert calculated, press space to continue.");
                _capture = false;
            }
            else 
            {
                Debug.Log(idx.ToString() + " Vert calculated.");
            }

            //Loop until allowed to continue
            do
            {
                yield return null;
            }
            while (!_capture);
            #endregion
        }

        _mesh.SetColors(_colors);

        _baker.Bake("");
        _startedCapture = false;
        _capture = false;

        Debug.LogWarning("Finished vertex color generation.");
    }

    Vector3 RandomInUnitSphere()
    {
        return Random.onUnitSphere;
    }
    private float RayCast(Ray ray, int triangleID, float biggestDist)
    {
        #region Data
        //Triangle
        int triangleIndicieX = _tris[triangleID];
        int triangleIndicieY = _tris[triangleID + 1];
        int triangleIndicieZ = _tris[triangleID + 2];
        //Polygon corners
        Vector3 vertA = _verts[triangleIndicieX];
        Vector3 vertB = _verts[triangleIndicieY];
        Vector3 vertC = _verts[triangleIndicieZ];
        #endregion

        #region IntersectCalc
        //Poly normal
        Vector3 norm = Vector3.Cross(vertB - vertA, vertC - vertA).normalized;
        //Float distance, hitposition
        float dist = Vector3.Dot(vertA - ray.origin, norm) / Vector3.Dot(ray.direction, norm);
        Vector3 hitPos = ray.origin + dist * ray.direction;

        dist = (dist < 0.0f) ? biggestDist : dist;
        dist = (biggestDist < dist) ? biggestDist : dist;

        //Check if in triangle
        float dotAB = Vector3.Dot(norm, Vector3.Cross(vertB - vertA, hitPos - vertA));
        float dotBC = Vector3.Dot(norm, Vector3.Cross(vertC - vertB, hitPos - vertB));
        float dotCA = Vector3.Dot(norm, Vector3.Cross(vertA - vertC, hitPos - vertC));

        bool AB = (dotAB >= 0) ? true : false;
        bool BC = (dotBC >= 0) ? true : false;
        bool CA = (dotCA >= 0) ? true : false;
        #endregion

        #region Result
        if ((AB && BC && CA || !AB && !BC && !CA) && !float.Equals(norm.sqrMagnitude, 0.0f))
        {
            return dist;
        }
        else
        {
            return biggestDist;
        }
        #endregion
    }
    private void Compute(int idx)
    {
        _computing = true;

        #region Ray
        //Our Ray
        Vector3 rayPos = _verts[idx] - _norms[idx] * 0.01f;
        Vector3 rayDir = RandomInUnitSphere();

        Ray ray = new Ray(rayPos, rayDir);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 5.0f);
        #endregion

        #region Casting
        //Go over all triangles/indecies
        float dist = 100000.0f;
        for (int triangleNr = 0; triangleNr < _mesh.triangles.Length; triangleNr += 3)
        {
            dist = RayCast(ray, triangleNr, dist);
        }
        #endregion

        dist /= _distanceDiv;
        _colors[idx] = new Color(dist, dist, dist, 1);

        _computing = false;
    }
}
