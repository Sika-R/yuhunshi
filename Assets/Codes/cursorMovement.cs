using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;

public class cursorMovement : MonoBehaviour
{

    RaycastHit hit;
    Ray ray;

    gridElement lastHit;
    RectTransform rectTransform;
    void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
        this.rectTransform.sizeDelta = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.collider.tag == "gridElement")
        {
            this.transform.position = hit.collider.transform.position;
            lastHit = hit.collider.gameObject.GetComponent<gridElement>();
            this.rectTransform.sizeDelta = new Vector2(1.0f, lastHit.GetElementHeight());
            if (Input.GetMouseButtonDown(1))
            {
                SetCurserButton(0);
            }
        }



        // reset when press 5
        //read a file, instantiate a constructor by width height. then read an array to enable and disable gridElement.
        if (Input.GetKeyDown("5"))
        {
            constructor.instance.freeMemory();
            constructor.instance.initialize(constructor.instance.info.width, constructor.instance.info.height);
        }
        //save file
        if (Input.GetKeyDown("1"))
        {
            XMLOp.Serialize(constructor.instance.info, "constructorInfo.xml");
        }
        if (Input.GetKeyDown("9"))
        {
            constructorInfo outInfo = new constructorInfo();
            outInfo = XMLOp.Deserialize<constructorInfo>("constructorInfo.xml");
            constructor.instance.info.width = outInfo.width;
            constructor.instance.info.height = outInfo.height;

            constructor.instance.freeMemory();
            constructor.instance.initialize(outInfo.width, outInfo.height);
            for (int i = 0; i < outInfo.width * outInfo.width * outInfo.height; i++)
            {
                if (outInfo.gridElementStatus[i] == 0)
                {
                    constructor.instance.info.gridElementStatus[i] = 0;
                    constructor.instance.gridElements[i].SetDisable();
                }
                else
                {
                    constructor.instance.info.gridElementStatus[i] = 1;
                    constructor.instance.gridElements[i].SetEnable();

                }
            }


        }
    }

    public void SetCurserButton(int input)
    {
        coord coord = lastHit.GetCoord();
        int width = constructor.instance.width;
        int height = constructor.instance.height;

        switch (input)
        {
            case 0:
                //remove gridElement
                if (coord.y > 0)
                {
                    //not that setDisable or enable will influence gridElement's cornerElements.
                    lastHit.SetDisable();
                }
                break;
            case 1:
                //add X+
                if (coord.x < width - 1)
                {
                    constructor.instance.gridElements[coord.x + width * (coord.z + width * coord.y) + 1].SetEnable();
                }
                break;
            case 2:
                //add X-
                if (coord.x >0)
                {
                    constructor.instance.gridElements[coord.x + width * (coord.z + width * coord.y) - 1].SetEnable();
                }
                break;
            case 3:
                //add Z+
                if (coord.z < width -1)
                {
                    constructor.instance.gridElements[coord.x + width * (coord.z + 1 + width * coord.y) ].SetEnable();
                }
                break;
            case 4:
                //add Z-
                if (coord.z  > 0)
                {
                    constructor.instance.gridElements[coord.x + width * (coord.z - 1 + width * coord.y) ].SetEnable();
                }
                break;
            case 5:
                //add Y+
                if (coord.y < height - 1)
                {
                    constructor.instance.gridElements[coord.x + width * (coord.z  + width * (coord.y + 1))].SetEnable();
                }
                break;
        }
    }
}



