﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour {

    #region Unity Serialized Fields

    [SerializeField] private ComputeShader compute;
    [SerializeField] private RenderTexture result;
    [SerializeField] private int width = 512;
    [SerializeField] private int height = 512;
    [SerializeField] private Texture photoInput;
    [SerializeField] private Material mat;
    [SerializeField] private TerrainGen tg;
    

    #endregion

    #region Private Fields

    private int kernelID;

    #endregion

	
	// Update is called once per frame
	void Update () {

        kernelID = compute.FindKernel("Life"); //returns an id for this function

        compute.SetTexture(kernelID, "pictureInput", photoInput); //send in picture
        compute.SetFloat("width", width);
        compute.SetFloat("height", height);

        result = new RenderTexture(width, height, 24);
        result.wrapMode = TextureWrapMode.Repeat; //wrap around texture
        result.enableRandomWrite = true;
        result.filterMode = FilterMode.Point; //texture pixels become blocky up close
        //how to render when transformed by 3d things
        result.useMipMap = false;
        result.Create();

        compute.SetTexture(kernelID, "Result", result);
        compute.Dispatch(kernelID, width / 8, height / 8, 1);

        photoInput = result;
        mat.mainTexture = photoInput;

        tg.UpdateTerrain(ConvertToTexture2D(mat.mainTexture));

    }

    public Color[] LifeOneFrame()
    {
        result = new RenderTexture(width, height, 24);
        result.enableRandomWrite = true;
        result.Create();

        kernelID = compute.FindKernel("Life"); //returns an id for this function

        compute.SetTexture(kernelID, "pictureInput", photoInput); //send in picture
        compute.SetFloat("width", width);
        compute.SetFloat("height", height);

        result = new RenderTexture(width, height, 24);
        result.wrapMode = TextureWrapMode.Repeat; //wrap around texture
        result.enableRandomWrite = true;
        result.filterMode = FilterMode.Point; //texture pixels become blocky up close
        //how to render when transformed by 3d things
        result.useMipMap = false;
        result.Create();

        compute.SetTexture(kernelID, "Result", result);
        compute.Dispatch(kernelID, width / 8, height / 8, 1);

        mat.mainTexture = result;

        return ConvertToTexture2D(mat.mainTexture);
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
}
