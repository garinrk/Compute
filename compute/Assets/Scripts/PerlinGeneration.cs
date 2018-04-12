using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGeneration : MonoBehaviour {

    #region Unity Serialized Fields

    [SerializeField] protected ComputeShader shader;
    [SerializeField] private Renderer renderDestination;
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 128;
    [SerializeField] private Terrain destinationTerrain;

    [Header("Current Texture")]
    [SerializeField] private RenderTexture currentTex;

    #endregion

    #region Private Fields

    private int kernelID = 0;
    private Material destinationMaterial;

    #endregion


    // Use this for initialization
    void Start () {

        currentTex = new RenderTexture(width, height, 24);
        currentTex.wrapMode = TextureWrapMode.Repeat;
        currentTex.enableRandomWrite = true;
        currentTex.filterMode = FilterMode.Point;

        currentTex.Create();
        UpdateTexture();

    }

    // Update is called once per frame
    void UpdateTexture () {

        shader.SetInt("RandOffset", (int)(Time.timeSinceLevelLoad * 100));
        shader.SetTexture(kernelID, "Result", currentTex);
        shader.Dispatch(kernelID, width / 8, height / 8, 1);
        renderDestination.material.mainTexture = currentTex;
        SetTerrain();

    }

    private void SetTerrain()
    {

        int detailHeight = destinationTerrain.terrainData.detailHeight;
        int heightmapHeight = destinationTerrain.terrainData.heightmapHeight;
        Vector3 x = destinationTerrain.terrainData.heightmapScale;
        Color[] colorData = ReadColorDataFromTexture(renderDestination.material.mainTexture);
        float[,] heights = CreateHeightDataFromColors(colorData);
        destinationTerrain.terrainData.SetHeights(0, 0, heights);
    }

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

    private Color[] ReadColorDataFromTexture(Texture i_tex)
    {
        Texture2D result = new Texture2D(i_tex.width, i_tex.height, TextureFormat.RGBA32, false);

        RenderTexture renderTex = new RenderTexture(i_tex.width, i_tex.height, 32);
        Graphics.Blit(i_tex, renderTex);

        result.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        result.Apply();

        return result.GetPixels();
    }

}
