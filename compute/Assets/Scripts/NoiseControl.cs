using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseControl : MonoBehaviour {

    #region Unity Serialized Fields

    [SerializeField] private ComputeShader noiseShader;
    [SerializeField] private int resolution = 128;
    [SerializeField] private NoiseTerrainGen ntg;

    #endregion

    #region Private Fields

    private Renderer renderer;
    private RenderTexture rendererTex;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        renderer.enabled = true;
    }

    private void Start()
    {
        rendererTex = new RenderTexture(resolution, resolution, 24);
        rendererTex.enableRandomWrite = true;
        rendererTex.Create();

        InvokeRepeating("UpdateNoiseTexture", 0, 0.5f);

    }

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.O))
        //{
        //    UpdateNoiseTexture();
        //}

        //UpdateNoiseTexture();
    }

    #endregion

    #region Private Interface

    private void UpdateNoiseTexture()
    {
        int kernelID = noiseShader.FindKernel("CSMain");
        noiseShader.SetInt("RandOffset", (int)(Time.timeSinceLevelLoad * 100));

        noiseShader.SetTexture(kernelID, "Result", rendererTex);
        noiseShader.Dispatch(kernelID, resolution / 8, resolution / 8, 1);

        renderer.material.SetTexture("_MainTex", rendererTex);

        Color[] colorData = ConvertToTexture2D(renderer.material.mainTexture);
        ntg.UpdateTerrain(colorData);
    }

    private Color[] ConvertToTexture2D(Texture i_tex)
    {
        Texture2D result = new Texture2D(i_tex.width, i_tex.height, TextureFormat.RGBA32, false);

        RenderTexture renderTex = new RenderTexture(i_tex.width, i_tex.height, 32);
        Graphics.Blit(i_tex, renderTex);

        result.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        result.Apply();

        return result.GetPixels();
    }

    #endregion


}
