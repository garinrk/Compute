using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{


    #region Unity Serialized Fields
    [SerializeField] private Life gameOfLife;
    #endregion

    #region Private Fields
    private float[,] heightData;
    private Terrain myTerr;
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        myTerr = GetComponent<Terrain>();
    }


    #endregion

    #region Private Interface

    private void CopyColorsToHeights(Color[] i_colorData, int i_dim)
    {
        heightData = new float[i_dim, i_dim];
        int colorIndex = 0;
        for (int i = 0; i < i_dim; i++)
        {
            for (int j = 0; j < i_dim; j++)
            {
                heightData[i, j] = i_colorData[colorIndex].r;

                colorIndex++;
            }
        }
    }

    #endregion

    #region Public Interface
    public void UpdateTerrain(Color[] i_colorData)
    {
        int square = (int)Mathf.Sqrt(i_colorData.Length);

        CopyColorsToHeights(i_colorData, square);

        myTerr.terrainData.SetHeights(0, 0, heightData);
    }

    #endregion
}
