using UnityEngine;

public class coord
{
    public int x, y, z;

    public coord(int setX, int setY, int setZ)
    {
        x = setX;
        y = setY;
        z = setZ;
    }
}



public class gridElement : MonoBehaviour
{

    public coord coord;
    private Collider col;
    private Renderer rend;

    private bool isEnabled;

    private float elementHeight;

    //array register CE nearby
    public cornerElement[] corners = new cornerElement[8];

   
    public void Initialize(int setX, int setY, int setZ, float setElementHeight)
    {
        int width = constructor.instance.width;
        //record the coordinates
        coord = new coord(setX, setY, setZ);
        this.name = "GE_" + this.coord.x + "_" + this.coord.y + "_" + this.coord.z;
        //scale it
        this.elementHeight = setElementHeight;
        this.transform.localScale = new Vector3(1.0f, elementHeight, 1.0f);
        this.col = this.GetComponent<Collider>();
        this.rend = this.GetComponent<Renderer>();

        //register CEs when initialize a GE (read from global constructor instance's CE array)
        corners[0] = constructor.instance.cornerElements[coord.x + (width + 1) * (coord.z + (width + 1) * coord.y)];
        corners[1] = constructor.instance.cornerElements[coord.x + 1 + (width + 1) * (coord.z + (width + 1) * coord.y)];
        corners[2] = constructor.instance.cornerElements[coord.x + (width + 1) * (coord.z + 1 + (width + 1) * coord.y)];
        corners[3] = constructor.instance.cornerElements[coord.x + 1 + (width + 1) * (coord.z + 1 + (width + 1) * coord.y)];
        corners[4] = constructor.instance.cornerElements[coord.x + (width + 1) * (coord.z + (width + 1) * (coord.y + 1))];
        corners[5] = constructor.instance.cornerElements[coord.x + 1 + (width + 1) * (coord.z + (width + 1) * (coord.y + 1))];
        corners[6] = constructor.instance.cornerElements[coord.x + (width + 1) * (coord.z + 1 + (width + 1) * (coord.y + 1))];
        corners[7] = constructor.instance.cornerElements[coord.x + 1 + (width + 1) * (coord.z + 1 + (width + 1) * (coord.y + 1))];

        float posY = this.transform.position.y;
        float deltaY = elementHeight / 2;
        //positioning cornerElements
        corners[0].SetPosition(col.bounds.min.x, posY - deltaY, col.bounds.min.z);
        corners[1].SetPosition(col.bounds.max.x, posY - deltaY, col.bounds.min.z);
        corners[2].SetPosition(col.bounds.min.x, posY - deltaY, col.bounds.max.z);
        corners[3].SetPosition(col.bounds.max.x, posY - deltaY, col.bounds.max.z);
        corners[4].SetPosition(col.bounds.min.x, posY + deltaY, col.bounds.min.z);
        corners[5].SetPosition(col.bounds.max.x, posY + deltaY, col.bounds.min.z);
        corners[6].SetPosition(col.bounds.min.x, posY + deltaY, col.bounds.max.z);
        corners[7].SetPosition(col.bounds.max.x, posY + deltaY, col.bounds.max.z);
    }

    public coord GetCoord()
    {
        return coord;
    }

    //triggerd by mouse
    public void SetEnable()
    {
        int width = constructor.instance.width;
        this.isEnabled = true;
        this.col.enabled = true;
        constructor.instance.info.gridElementStatus[coord.x + width * (coord.z + width* coord.y)] = 1;
        foreach (cornerElement ce in corners)
        {
            ce.SetCornerElement();
        }
    }

    public void SetDisable()
    {
        int width = constructor.instance.width;
        this.isEnabled = false;
        this.col.enabled = false;
        //this.rend.enabled = false;
        constructor.instance.info.gridElementStatus[coord.x + width * (coord.z + width * coord.y)] = 0;
        foreach (cornerElement ce in corners)
        {
            ce.SetCornerElement();
        }
    }

    public bool GetEnabled()
    {
        return isEnabled;
    }

    public float GetElementHeight()
    {
        return elementHeight;
    }


}

