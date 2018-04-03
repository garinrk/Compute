using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseControl : MonoBehaviour {

    #region Unity Serialized Fields

    [SerializeField] private ComputeShader noiseShader;
    [SerializeField] private int resolution = 128;
    

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

    }

    private void Update()
    {
        //if(Input.GetKeyUp(KeyCode.O))
        //{
        //    UpdateNoiseTexture();
        //}

        UpdateNoiseTexture();
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
    }


    #endregion


}
