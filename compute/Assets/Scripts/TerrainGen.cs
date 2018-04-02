using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour {


    #region Unity Serialized Fields

    
    [SerializeField] private Life gameOfLife;
    #endregion

    #region Private Fields

    private int width;
    private int height;
    float[,] heightData;
    float[,] backupHeightData;
    private Terrain myTerr;
    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        myTerr = GetComponent<Terrain>();
    }

    // Use this for initialization
    void Start()
    {

        Color[] colorData = gameOfLife.LifeOneFrame();
        int square = (int)Mathf.Sqrt(colorData.Length);
        //myTerr.terrainData.size = new Vector3(square,0,square);


        int colorIndex = 0;
        //heightData = myTerr.terrainData.GetHeights(0, 0, myTerr.terrainData.heightmapWidth, myTerr.terrainData.heightmapHeight);
        heightData = new float[square, square];
        for (int i = 0; i < square; i++)
        {
            for(int j = 0; j < square; j++)
            {
                heightData[i, j] = colorData[colorIndex].r;
                
                colorIndex++;
            }
        }

        myTerr.terrainData.SetHeights(0, 0, heightData);

    }

    //private void OnDisable()
    //{
    //    int backupWidth = backup.terrainData.heightmapWidth;
    //    int backupHeight = backup.terrainData.heightmapHeight;

    //    backupHeightData = backup.terrainData.GetHeights(0, 0, backupWidth, backupHeight);

    //    myTerr.terrainData.SetHeights(0, 0, backupHeightData);
    //}

    // Update is called once per frame
    void Update () {

        //heightData = myTerr.terrainData.GetHeights(0, 0, width, height);

        //for(int i = 0; i < width / 2; i++)
        //{
        //    for(int j = 0; j < height; j++)
        //    {
        //        heightData[i, j] = 0.24f;
        //    }
        //}

        //myTerr.terrainData.SetHeights(0, 0, heightData);
	}

    #endregion
}
