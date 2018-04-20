////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Assets\Scripts\PerlinGeneration.cs
//
// summary: The PerlinGeneration class handles dispatching the compute shader, and translating it to a heightmap that can be
// used with a terrain object.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PerlinGeneration : MonoBehaviour {

    #region Unity Serialized Fields

    [SerializeField] private ComputeShader perlinShader;
    [SerializeField] private Renderer renderDestination;
    [SerializeField] private int texSize = 128;
    [SerializeField] private float gridSize = 1.0f;
    [SerializeField] private Terrain destinationTerrain;
    [SerializeField] private int clampValue = 1;
    [SerializeField] private InputField clampField;
    [SerializeField] private InputField sizeField;

    //heightmap resolution = size of texture going in 
    [Header("Textures")]
    [SerializeField] private RenderTexture finalResult;

    #endregion

    #region Private Fields

    private int perlinKernelID = 0;
    private TerrainData destinationTerrainData;
    
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        destinationTerrainData = destinationTerrain.terrainData;
    }

    void Start () {

        perlinKernelID = perlinShader.FindKernel("CSMain");

        finalResult = new RenderTexture(texSize, texSize, 24);
        finalResult.wrapMode = TextureWrapMode.Clamp;
        finalResult.enableRandomWrite = true;
        finalResult.filterMode = FilterMode.Bilinear;

        finalResult.Create();

        UpdateTexture();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnDisable()
    {
        finalResult.DiscardContents();
    }

    #endregion

    #region Public Interface

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Updates the texture with parameters, read from the input fields
    ///             on the user interface </summary>
    ///
    /// <remarks>   garinrk, 4/20/2018. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void UpdateTextureWithParameters()
    {
        float newSampleSize = float.Parse(sizeField.text);
        int newClampValue = -99;

        Int32.TryParse(clampField.text, out newClampValue);

        gridSize = newSampleSize;
        clampValue = newClampValue;
        UpdateTexture();
    }

    #endregion

    #region Private Interface

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Updates the texture, by dispatching the compute shader </summary>
    ///
    /// <remarks>   garinrk, 4/20/2018. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void UpdateTexture()
    {

        perlinShader.SetInt("RandOffset", (int)(Time.timeSinceLevelLoad * 100));
        perlinShader.SetTexture(perlinKernelID, "Result", finalResult);
        perlinShader.SetInt("width", texSize);
        perlinShader.SetInt("height", texSize);
        perlinShader.SetInt("clampValue", clampValue);
        perlinShader.SetFloat("gridSize", gridSize);
        perlinShader.Dispatch(perlinKernelID, texSize / 8, texSize / 8, 1);

        renderDestination.material.mainTexture = finalResult;


        SetTerrain();

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Sets the terrain with data read from the generated
    ///             perlin noise texture.                              </summary>
    ///
    /// <remarks>   garinrk, 4/20/2018. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private void SetTerrain()
    {
        Color[] colorData = ReadColorDataFromTexture(renderDestination.material.mainTexture);
        float[,] heights = CreateHeightDataFromColors(colorData);
        destinationTerrain.terrainData.SetHeights(0, 0, heights);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates height data from colors, taking into account the r channel
    ///             of the pixel. (Adequate since the output of the compute shader is
    ///             black and white </summary>
    ///
    /// <remarks>   garinrk, 4/20/2018. </remarks>
    ///
    /// <param name="i_colorData">  array of colors </param>
    ///
    /// <returns>   The new height data from colors. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private float[,] CreateHeightDataFromColors(Color[] i_colorData)
    {
        int dim = (int)Mathf.Sqrt(i_colorData.Length);

        float[,] heightData = new float[dim, dim];

        int colorIndex = 0;

        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                heightData[i, j] = i_colorData[colorIndex].r;

                colorIndex++;
            }
        }

        return heightData;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Reads color data from texture, using a blit call. Blit coipes source texture
    ///             onto a destination render texture </summary>
    ///
    /// <remarks>   garinrk, 4/20/2018. </remarks>
    ///
    /// <param name="i_tex">    input texture </param>
    ///
    /// <returns>   An array of colors. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private Color[] ReadColorDataFromTexture(Texture i_tex)
    {
        Texture2D resultTex = new Texture2D(i_tex.width, i_tex.height, TextureFormat.RGBA32, false);

        RenderTexture renderTex = new RenderTexture(i_tex.width, i_tex.height, 32);
        renderTex.filterMode = FilterMode.Bilinear;

        Graphics.Blit(i_tex, renderTex);
        renderTex.DiscardContents();

        resultTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        resultTex.Apply();

        return resultTex.GetPixels();
        
    }

    #endregion
}
