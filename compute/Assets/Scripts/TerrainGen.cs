using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour {


    #region Unity Serialized Fields

    [SerializeField] private Terrain myTerr;
    [SerializeField] private Terrain backup;
    [SerializeField] private Texture2D tex;

    #endregion

    #region Private Fields

    private int width;
    private int height;
    float[,] heightData;
    float[,] backupHeightData;

    #endregion

    #region Unity Lifecycle

    // Use this for initialization
    void Start () {

        width = myTerr.terrainData.heightmapWidth;
        height = myTerr.terrainData.heightmapHeight;

        myTerr.heightmapMaximumLOD = 0;
		
	}

    private void OnDisable()
    {
        int backupWidth = backup.terrainData.heightmapWidth;
        int backupHeight = backup.terrainData.heightmapHeight;

        backupHeightData = backup.terrainData.GetHeights(0, 0, backupWidth, backupHeight);

        myTerr.terrainData.SetHeights(0, 0, backupHeightData);
    }

    // Update is called once per frame
    void Update () {

        heightData = myTerr.terrainData.GetHeights(0, 0, width, height);

        for(int i = 0; i < width / 2; i++)
        {
            for(int j = 0; j < height; j++)
            {
                heightData[i, j] = 0.24f;
            }
        }

        myTerr.terrainData.SetHeights(0, 0, heightData);
	}

    #endregion
}
