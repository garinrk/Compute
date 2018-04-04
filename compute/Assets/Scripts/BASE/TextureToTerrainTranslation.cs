using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureToTerrainTranslation : MonoBehaviour {


    #region Unity Serialized Fields

    [SerializeField] protected ComputeShader shader;
    [SerializeField] private float updateEverySeconds = 0.5f;
    [SerializeField] protected Texture photoInput;
    [SerializeField] private Renderer renderDestination;
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 128;

    [Header("Current Texture")]
    [SerializeField] private RenderTexture currentTex;

    #endregion

    #region Protected Fields

    protected Terrain destinationTerrain;

    #endregion

    #region Private Fields

    private int kernelID = 0;
    private Material destinationMaterial;

    #endregion

    #region Unity LifeCycle

    protected virtual void Awake()
    {
        destinationTerrain = GetComponent<Terrain>();
    }

    protected virtual void Start()
    {

        destinationMaterial = renderDestination.material;
        currentTex = new RenderTexture(width, height, 24);
        kernelID = shader.FindKernel("CSMain");
        InvokeRepeating("UpdateCall", 0.0f, updateEverySeconds);

    }

    #endregion

    #region Private Interface

    private void UpdateCall()
    {
        UpdateTexture();
        UpdateTerrain();
    }

    private void UpdateTerrain()
    {
        Color[] colorData = ReadColorDataFromTexture(destinationMaterial.mainTexture);
        float[,] heights = CreateHeightDataFromColors(colorData);
        destinationTerrain.terrainData.SetHeights(0, 0, heights);
    }

    private void UpdateTexture()
    {

        //life properties
        shader.SetFloat("width", width);
        shader.SetFloat("height", height);
        shader.SetTexture(kernelID, "pictureInput", photoInput); //send in picture

        //noise properties
        shader.SetInt("RandOffset", (int)(Time.timeSinceLevelLoad * 100));

        currentTex = new RenderTexture(width, height, 24);
        currentTex.wrapMode = TextureWrapMode.Repeat;
        currentTex.enableRandomWrite = true;
        currentTex.filterMode = FilterMode.Point;
        
        currentTex.Create();

        shader.SetTexture(kernelID, "Result", currentTex);
        shader.Dispatch(kernelID, width / 8, height / 8, 1);
        photoInput = currentTex;
        destinationMaterial.mainTexture = currentTex;
    }

    #endregion

    #region Protected Interface

   protected float[,] CreateHeightDataFromColors(Color[] i_colorData)
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

   protected Color[] ReadColorDataFromTexture(Texture i_tex)
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
