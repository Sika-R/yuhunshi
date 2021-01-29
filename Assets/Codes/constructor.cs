using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class constructor : MonoBehaviour
{
    public static constructor instance;
    public int width = 5;
    public int height = 9;
    public gridElement gridElement;
    public cornerElement cornerElement;

    public cornerElement[] cornerElements;
    public gridElement[] gridElements;

    private float floorHeight = 0.25f, basementHeight;

    private void Start()
    {
        instance = this;

        basementHeight = 1.5f - floorHeight / 2;
        float elementHeight = 1f;

        //GE and CE array
        gridElements = new gridElement[width * width * height];
        cornerElements = new cornerElement[(width + 1) * (width + 1) * (height + 1)];

        //Instantiate and register each CE
        for (int y = 0; y < height + 1; y++)
        {
            for (int x = 0; x < width + 1; x++)
            {
                for (int z = 0; z < width +1; z++)
                {
                    cornerElement cornerElementInstance = Instantiate(cornerElement, Vector3.zero, Quaternion.identity, this.transform);
                    cornerElementInstance.Initialize(x, y, z);
                    cornerElements[x + (width+1) * (z + (width+1) * y)] = cornerElementInstance;
                }
            }
        }



        //Instantiate and register each GE
        for (int y = 0; y < height; y++)
        {
            float yPos = y;
            if(y == 0)
            {
                elementHeight = floorHeight;
            }

            else if ( y == 1)
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
                    gridElement gridElementInstance = Instantiate(gridElement, new Vector3(x, yPos, z), Quaternion.identity, this.transform);
                    gridElementInstance.Initialize(x, y, z, elementHeight);
                    gridElements[x + width * (z + width * y)] = gridElementInstance;
                }
            }
        }


        //register how many GE nearby foreach CE
        foreach(cornerElement corner in cornerElements)
        {
            corner.SetNearGridElements();
        }

        //set every GE enabled.
        foreach(gridElement gridElement in gridElements)
        {
            gridElement.SetEnable();
        }
    }
}
