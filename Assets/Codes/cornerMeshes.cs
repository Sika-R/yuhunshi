using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// an cornerMeshes Instance will store all the prepared meshes and pick the bitmask corresponding mesh.
/// </summary>
public class cornerMeshes : MonoBehaviour
{
    public static cornerMeshes instance;
    //the input mesh 
    public GameObject mesh;
    // to store the prepared meshes.
    private Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();


    //before start
    void Awake()
    {
        instance = this;
        Initialize();
    }
    
    private void Initialize()
    {
        foreach(Transform child in mesh.transform)
        {
            meshes.Add(child.name, child.GetComponent<MeshFilter>().sharedMesh);
        }
    }

    public Mesh GetCornerMesh(int bitmask, int level)
    {
        Mesh selectedMesh;

        if(level > 1)
        {
            if(meshes.TryGetValue(bitmask.ToString(),  out selectedMesh))
            {
                return selectedMesh;
            }
        }

        else if (level == 0)
        {
            if (meshes.TryGetValue(0 + "_" + bitmask.ToString(), out selectedMesh))
            {
                return selectedMesh;
            }
        }

        else if (level == 1)
        {
            if (meshes.TryGetValue(1 + "_" + bitmask.ToString(), out selectedMesh))
            {
                return selectedMesh;
            }
        }
        return null;

    }
}
