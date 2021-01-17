//Adapted from a blog from Snelha Belkhale, 
//https://snayss.medium.com/bake-a-pretty-or-computationally-challenging-shader-into-a-texture-unity-b524569d7384

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureBake : MonoBehaviour
{
    //To be baked object
    [SerializeField]
    private GameObject _objectToBake = null;
    //Materials for the unwrap
    [SerializeField]
    private Material _uvMaterial = null;
    [SerializeField]
    private Material _dilateMaterial = null;
    //Clear color
    [SerializeField]
    private Color _backgroundColor = new Color(0, 0, 0, 1);

    //Texture
    private Texture2D _texture = null;
    //Size of the texture
    [SerializeField]
    private Vector2Int _textureDim = new Vector2Int(256, 256);

    void Start()
    {
        _dilateMaterial.SetColor("BackgroundColor", _backgroundColor);
    }

    public void Bake(string fileSuff)
    {
        Debug.LogWarning("Started texture bake.");

        #region RenderStep
        //Create a surface to render to
        RenderTexture renderTexture = RenderTexture.GetTemporary(_textureDim.x, _textureDim.y, 16, RenderTextureFormat.ARGB32);
        //Get the mesh
        Mesh mesh = _objectToBake.GetComponent<MeshFilter>().mesh;

        //Set target
        Graphics.SetRenderTarget(renderTexture);
        //Make sure that the texture starts off cleared
        GL.Clear(true, true, _backgroundColor);
        //Make the camera oriented properly, orthographic
        GL.PushMatrix();
        GL.LoadOrtho();
        //Set to the first and only pass
        _uvMaterial.SetPass(0);
        Graphics.DrawMeshNow(mesh, Matrix4x4.identity);

        //Remove the render target
        Graphics.SetRenderTarget(null);

        //Create second surface to render to
        RenderTexture renderTexture2 = RenderTexture.GetTemporary(_textureDim.x, _textureDim.y, 16, RenderTextureFormat.ARGB32);
        //Apply the material effect as if it were PP, using target 1 as input
        Graphics.Blit(renderTexture, renderTexture2, _dilateMaterial);

        GL.PopMatrix();
        #endregion

        //Save
        Debug.LogWarning("Saving baked texture.");
        SaveToTexture(renderTexture2, _objectToBake.name + fileSuff);

        #region RenderCleanup
            //Cleanup
            RenderTexture.ReleaseTemporary(renderTexture);
            RenderTexture.ReleaseTemporary(renderTexture2);
            //GL.PopMatrix();
        #endregion

        Debug.LogWarning("Finished texture bake.");
    }
    private void SaveToTexture(RenderTexture renderTexture, string meshName)
    {
        //Setup absolute path
        string fullPath = Application.dataPath + "/Textures/" + meshName + ".png";
        //Encode png from texture2D
        byte[] _bytes = ConvertToTexture2D(renderTexture).EncodeToPNG();
        //Overwrite if needed
        if (File.Exists(fullPath)) 
        {
            File.Delete(fullPath);
        }
        File.WriteAllBytes(fullPath, _bytes);
    }
    private Texture2D ConvertToTexture2D(RenderTexture renderTexture)
    {
        //Make a texture2Dof correct size
        _texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        //Set the current rt as the active one, same as graphics.setrendertarget
        RenderTexture.active = renderTexture;
        //Read the pixels from the active rt and apply to the texture2D
        _texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        _texture.Apply();
        return _texture;
    }
}
