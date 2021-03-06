using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscLogic : MonoBehaviour
{
	float m_deltaY;

	bool selected = false;

    [SerializeField]
    float m_SpinSpeed = 5.0f;

    [HideInInspector]
    public int m_SlotCnt;
    // [SerializeField]
    // GameObject m_SlotPrefab;
    [SerializeField]
    public GameObject m_CanonPrefab;

    float m_DiscRadius;
	List<Vector3> m_SpawnPoints = new List<Vector3>();
	List<Quaternion> m_SpawnRotations = new List<Quaternion>();
	GameObject canon;
    // Start is called before the first frame update
    void Start()
    {
        // m_DiscRadius = GetComponent<Collider>().bounds.max.x;

        
        // //GameObject canon = Instantiate(m_CanonPrefab, initSpawnPoint, Quaternion.Euler(transform.forward)) as GameObject;
        // Vector3 nextDirection = transform.forward;
        // Quaternion nextRotation = transform.rotation;
        // Vector3 pos = transform.position;
        // pos.y += 0.5f;

        

        // for(int i = 0; i < m_SlotCnt; ++i)
        // {
        // 	canon = Instantiate(m_CanonPrefab, pos + (m_DiscRadius - 1.0f) * nextDirection, nextRotation) as GameObject;
        // 	canon.transform.parent = transform;
        // 	canon.transform.localScale = new Vector3(canon.transform.localScale.x, 2f, canon.transform.localScale.z);


        // 	Quaternion q = Quaternion.AngleAxis(360 / m_SlotCnt, transform.up);
        // 	nextDirection = q * nextDirection;
        // 	nextRotation = q * nextRotation;

	        
        // }
    }

    // Update is called once per frame
    void Update()
    {
    	if(Input.GetMouseButtonDown(0))
    	{
    		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

	        RaycastHit hit;

	        if (Physics.Raycast(ray, out hit, 100.0f) && hit.rigidbody == GetComponent<Rigidbody>())
	        {
	        	selected = true;
	        	m_deltaY = -Input.GetAxisRaw ("Mouse X") * m_SpinSpeed;
	        }
    	}

        if (Input.GetMouseButton(0) && selected)
        {
        	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

	        RaycastHit hit;

	        if (Physics.Raycast(ray, out hit, 100.0f) && hit.rigidbody == GetComponent<Rigidbody>())
	        {
	        	//Debug.DrawLine(ray.origin,hit.point,Color.red,100);
	        	m_deltaY = -Input.GetAxisRaw ("Mouse X") * m_SpinSpeed;
	        }
			
		}
		else
		{
			m_deltaY *= 0.9f;
		}
		transform.Rotate (new Vector3 (0, m_deltaY, 0),Space.World);
		if(Input.GetMouseButtonUp(0))
		{
			selected = false;
		}
    }
}
