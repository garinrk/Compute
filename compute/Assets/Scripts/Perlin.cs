using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin : MonoBehaviour {

    private Renderer sourceRenderer;
    [SerializeField] private Terrain destinationTerrain;

    private Material sourceMaterial;

    private void Awake()
    {
        sourceRenderer = GetComponent<Renderer>();
        sourceMaterial = sourceRenderer.material;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            SetTerrain();
        }
    }

    private void SetTerrain()
    {
        int x = 3;
        Color[] colorData = ReadColorDataFromTexture(sourceMaterial.mainTexture);
        float[,] heights = CreateHeightDataFromColors(colorData);
        destinationTerrain.terrainData.SetHeights(0, 0, heights);
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
}
