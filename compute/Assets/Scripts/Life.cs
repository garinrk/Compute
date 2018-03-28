using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour {

    #region Unity Serialized Fields

    [SerializeField] private ComputeShader compute;
    [SerializeField] private RenderTexture result;

    #endregion

    #region Public Fields

    public Color myColor;

    #endregion

    #region Private Fields

    private int kernelID;

    #endregion

    // Use this for initialization
    void Start () {

  

	}
	
	// Update is called once per frame
	void Update () {

        kernelID = compute.FindKernel("Life"); //returns an id for this function

        result = new RenderTexture(512, 512, 24);
        result.enableRandomWrite = true;
        result.Create();


        compute.SetTexture(kernelID, "Result", result); //out var from .compute shader
        compute.Dispatch(kernelID, 512 / 8, 512 / 8, 1); //thread for every pixel and rendering it
        compute.SetVector("color", myColor);
    }
}
