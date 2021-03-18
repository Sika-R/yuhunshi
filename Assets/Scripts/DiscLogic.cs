using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscLogic : MonoBehaviour
{
	float m_deltaY;
    // Vector2 m_MousePos;
    // Vector2 m_PreMousePos;
    // float m_RotateAngle;
    // Vector3 m_LocalEuler;
    // Vector2 m_ScreenPos;

	bool selected = false;

    [SerializeField]
    float m_SpinSpeed = 5.0f;

    [HideInInspector]
    public int m_SlotCnt;
    //可调整的旋转拟合角度
    [SerializeField]
    int m_AngleCnt;
    float m_SpinAngle;
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
        m_SlotCnt = transform.childCount;
        if(m_AngleCnt == 0)
        {
            m_AngleCnt = m_SlotCnt;
        }
        
        m_SpinAngle = 360.0f / m_AngleCnt;
        Debug.Log(m_SpinAngle);
        //m_ScreenPos = Camera.main.WorldToScreenPoint(transform.position);


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
	        	m_deltaY = -Input.GetAxis("Mouse X") * m_SpinSpeed;
                //m_PreMousePos = Input.mousePosition;
	        }
    	}

        if (Input.GetMouseButton(0) && selected)
        {
        	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

	        RaycastHit hit;

	        if (Physics.Raycast(ray, out hit, 100.0f) && hit.rigidbody == GetComponent<Rigidbody>())
	        {
                /*鼠标移动角度控制转盘
                m_MousePos = Input.mousePosition;
                m_RotateAngle = Vector2.Angle(m_PreMousePos - m_ScreenPos, m_MousePos - m_ScreenPos);
                if(m_RotateAngle != 0)
                {
                    Quaternion q = Quaternion.FromToRotation(m_PreMousePos - m_ScreenPos, m_MousePos - m_ScreenPos);
                    float k = q.z > 0 ? 1 : -1;
                    m_LocalEuler.y -= k * m_RotateAngle;
                    Debug.Log(m_LocalEuler);
                    transform.localEulerAngles = m_LocalEuler;
                }
                m_PreMousePos = m_MousePos;*/
                //鼠标移动距离控制转盘
	        	//Debug.DrawLine(ray.origin,hit.point,Color.red,100);
	        	m_deltaY = -Input.GetAxis("Mouse X") * m_SpinSpeed;
	        }
			
		}
        else
		{
			// if(transform.rotation.eulerAngles.y % m_SpinAngle < m_SpinAngle / 2)
   //          {
   //              m_deltaY *= -0.95f;
   //          }
            m_deltaY *= 0.9f;
            if(transform.rotation.eulerAngles.y % m_SpinAngle < 2)
            {
                m_deltaY = 0.0f;
            }
            else if(Mathf.Abs(m_deltaY) < 0.8f)
            {
                m_deltaY = m_deltaY > 0 ? 0.8f : -0.8f;
            }
            
		}
		transform.Rotate (new Vector3 (0, m_deltaY, 0),Space.World);
        
		if(Input.GetMouseButtonUp(0))
		{
            if(transform.rotation.eulerAngles.y % m_SpinAngle < m_SpinAngle / 2)
            {
                m_deltaY = -Mathf.Abs(m_deltaY);
            }
			selected = false;
		}
    }
}
