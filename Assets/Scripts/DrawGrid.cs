using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class DrawGrid : MonoBehaviour
{
	[SerializeField]
    bool bShowGizmos = true;

    [SerializeField]
    int m_column = 9; //列
    [SerializeField]
    int m_row = 9; //行
 
 	[SerializeField]
    public float m_gridSize = 1.0f; //大概的大小
    private readonly Color m_color = Color.white;

    private BoxCollider m_collider; //用来做射线检测

    [HideInInspector]
    public GameObject m_obstaclePrefab;
 
 	
 	private void Start()
    {
        //gameObject.layer = 31;
        m_collider = GetComponent<BoxCollider>();
        if (m_collider == null)
        {
            m_collider = gameObject.AddComponent<BoxCollider>();
            m_collider.size = new Vector3(100, 0.1f, 100);
        }
    }

    private void GridGizmo(int cols, int rows)
    {
        for (int i = 0; i <= cols; i++)
        {
            Gizmos.DrawLine(new Vector3(i * m_gridSize, 0, 0), new Vector3(i * m_gridSize, 0, rows * m_gridSize));
        }
        for (int j = 0; j <= rows; j++)
        {
            Gizmos.DrawLine(new Vector3(0, 0, j * m_gridSize), new Vector3(cols * m_gridSize, 0, j * m_gridSize));
        }
    }
 
    private void OnDrawGizmos()
    {
    	if (bShowGizmos == false || Application.isEditor == false )
    	{
            return;
    	}
        Gizmos.color = m_color;
        GridGizmo(m_column, m_row);
    }

    public Vector3 IdentifyCenter(Vector3 hit)
    {
    	float x = hit.x - transform.position.x;
    	float z = hit.z - transform.position.z;
    	if(x < 0 || x > m_column * m_gridSize || z < 0 || z > m_row * m_gridSize)
    	{
    		return new Vector3(-1, -1, -1);
    	}
    	int xx = (int)Mathf.Floor(x / m_gridSize);
    	int zz = (int)Mathf.Floor(z / m_gridSize);
    	return new Vector3((xx + 0.5f) * m_gridSize, transform.position.y + 0.5f * m_gridSize, (zz + 0.5f) * m_gridSize);
    }
}

