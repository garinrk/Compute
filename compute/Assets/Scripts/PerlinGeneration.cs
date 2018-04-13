/*
Garin Richards - u0738045
CS6610 - Int. Computer Graphics
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGeneration : MonoBehaviour {

    #region Unity Serialized Fields

    [SerializeField] private ComputeShader perlinShader;
    [SerializeField] private ComputeShader blurShader;
    [SerializeField] private Renderer renderDestination;
    [SerializeField] private int texSize = 128;
    [SerializeField] private float updateDelta = .1f;
    [SerializeField] private float gridSize = 1.0f;
    [SerializeField] private Terrain destinationTerrain;
    [SerializeField] private bool usePerlinOutput = true;
    [SerializeField] private int blurSampleSize = 25;
    [SerializeField] private int clampValue = 1;

    //heightmap resolution = size of texture going in 
    [Header("Textures")]
    [SerializeField] private RenderTexture currentTex;
    [SerializeField] private RenderTexture finalResult;

    #endregion

    #region Private Fields

    private int perlinKernelID = 0;
    private int blurKernelID = 0;
    private TerrainData destinationTerrainData;
    
    private float xValue = 0.2f;
    private float yValue = 1f;

    private Texture noiseResult;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        destinationTerrainData = destinationTerrain.terrainData;
    }

    void Start () {

        perlinKernelID = perlinShader.FindKernel("CSMain");
        blurKernelID = blurShader.FindKernel("CSMain");
        currentTex = new RenderTexture(texSize, texSize, 24);
        currentTex.wrapMode = TextureWrapMode.Repeat;
        currentTex.enableRandomWrite = true;
        currentTex.filterMode = FilterMode.Bilinear;

        finalResult = new RenderTexture(texSize, texSize, 24);
        finalResult.wrapMode = TextureWrapMode.Clamp;
        finalResult.enableRandomWrite = true;
        finalResult.filterMode = FilterMode.Bilinear;

        currentTex.Create();
        finalResult.Create();

        UpdateTexture();

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            xValue += updateDelta;
            yValue += updateDelta;
            UpdateTexture();
        }
    }

    private void OnDisable()
    {
        currentTex.DiscardContents();
        finalResult.DiscardContents();
    }

    #endregion

    #region Private Interface
    private void UpdateTexture()
    {

        perlinShader.SetInt("RandOffset", (int)(Time.timeSinceLevelLoad * 100));
        perlinShader.SetTexture(perlinKernelID, "Result", currentTex);
        perlinShader.SetFloat("xVal", xValue);
        perlinShader.SetFloat("yVal", yValue);
        perlinShader.SetInt("width", texSize);
        perlinShader.SetInt("height", texSize);
        perlinShader.SetInt("clampValue", clampValue);
        perlinShader.SetFloat("gridSize", gridSize);
        perlinShader.Dispatch(perlinKernelID, texSize / 8, texSize / 8, 1);

        blurShader.SetFloat("width", texSize);
        blurShader.SetFloat("height", texSize);
        blurShader.SetInt("sampleSize", blurSampleSize);
        blurShader.SetTexture(blurKernelID, "input", currentTex);
        blurShader.SetTexture(blurKernelID, "Result", finalResult);
        blurShader.Dispatch(blurKernelID, texSize / 8, texSize / 8, 1);
      
        renderDestination.material.mainTexture = finalResult;

        if (usePerlinOutput)
            renderDestination.material.mainTexture = currentTex;

        SetTerrain();

    }

    private void SetTerrain()
    {
        Vector3 heightScale = destinationTerrain.terrainData.heightmapScale;
        Color[] colorData = ReadColorDataFromTexture(renderDestination.material.mainTexture);
        float[,] heights = CreateHeightDataFromColors(colorData, heightScale.y);
        destinationTerrain.terrainData.SetHeights(0, 0, heights);
    }

    private float[,] CreateHeightDataFromColors(Color[] i_colorData, float i_max)
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
