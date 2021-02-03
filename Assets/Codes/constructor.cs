using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
/// <summary>
/// Constructor takes the width and height to build the framework. It holds the following thing.
///     1) all the gridElements and cornerElements. 
///     2) floor, basement Height.
///     3) corner and gridElement prefabs.
///     4) all the PrefabInstance.
///  
/// </summary>
/// 



public class constructorInfo
{
    public int width;
    public int height;
    [XmlArray("GridStatus"), XmlArrayItem("status")]
    public int[] gridElementStatus;
    public constructorInfo()
    {

    }
    public constructorInfo(int setWidth, int setHeight)
    {
        width = setWidth;
        height = setHeight;
        gridElementStatus = new int[width * width * height];
    }
}

public class constructor : MonoBehaviour
{
    public static constructor instance;
    public constructorInfo info;
    public int width;
    public int height;
    //Two prefabs to instantiate
    public GameObject  gridElementPrefab;
    public GameObject cornerElementPrefab;

    public GameObject[] cornerElementPrefabs;
    public GameObject[] gridElementPrefabs;

    public cornerElement[] cornerElements;
    public gridElement[] gridElements;
    private float floorHeight = 0.25f, basementHeight;

    public void initialize(int width, int height)
    {
        info = new constructorInfo(width, height);
        this.height = height;
        this.width = width;
        basementHeight = 1.5f - floorHeight / 2;
        float elementHeight = 1f;

        gridElementPrefabs = new GameObject[width * width * height];
        cornerElementPrefabs = new GameObject[(width + 1) * (width + 1) * (height + 1)];
        gridElements = new gridElement[width * width * height];
        cornerElements = new cornerElement[(width + 1) * (width + 1) * (height + 1)];


        //Instantiate and register each CE
        for (int y = 0; y < height + 1; y++)
        {
            for (int x = 0; x < width + 1; x++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    //intantiate (place) prefab there
                    GameObject cornerElementPrefabInstance = Instantiate(cornerElementPrefab, Vector3.zero, Quaternion.identity, this.transform);
                    cornerElement cornerElementInstance = cornerElementPrefabInstance.GetComponent<cornerElement>();
                    //set instance
                    cornerElementInstance.Initialize(x, y, z);
                    //record cornerElement.
                    cornerElements[x + (width + 1) * (z + (width + 1) * y)] = cornerElementInstance;
                    cornerElementPrefabs[x + (width + 1) * (z + (width + 1) * y)] = cornerElementPrefabInstance;
                }
            }
        }
        //Instantiate and register each GE
        for (int y = 0; y < height; y++)
        {
            float yPos = y;
            if (y == 0) //floor
            {
                elementHeight = floorHeight;
            }

            else if (y == 1)//basement
            {
                elementHeight = basementHeight;
                yPos = floorHeight / 2 + basementHeight / 2;
            }
            else
            {
                elementHeight = 1;
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < width; z++)
                {
                    //intantiate (place) prefab there
                    GameObject gridElementPrefabInstance = Instantiate(gridElementPrefab, new Vector3(x, yPos, z), Quaternion.identity, this.transform);
                    gridElement gridElementInstance = gridElementPrefabInstance.GetComponent<gridElement>();
                    //set instance, record surrounding cornerElement and place them.
                    gridElementInstance.Initialize(x, y, z, elementHeight);
                    //record gridElement.
                    gridElements[x + width * (z + width * y)] = gridElementInstance;
                    gridElementPrefabs[x + width * (z + width * y)] = gridElementPrefabInstance;
                }
            }
        }


        //register surrounding GE nearby foreach CE
        foreach (cornerElement corner in cornerElements)
        {
            corner.SetNearGridElements();
        }

        //set every GE enabled.
        foreach (gridElement gridElement in gridElements)
        {
            gridElement.SetEnable();
        }
    }

    public void freeMemory()
    {
        foreach(GameObject gePrefab in gridElementPrefabs)
        {
            Destroy(gePrefab);
        }
        foreach(GameObject cePrefab in cornerElementPrefabs)
        {
            Destroy(cePrefab);
        }
    }
    /// <summary>
    /// /////////////---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    /// </summary>
    private void Start()
    {
        instance = this;
        instance.initialize(width, height);
    }
}
