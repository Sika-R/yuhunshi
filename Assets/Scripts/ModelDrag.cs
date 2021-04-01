using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModelDrag : MonoBehaviour
{
    private Camera m_Camera;//发射射线的摄像机
    private GameObject m_Land;//射线碰撞的物体
    private Vector3 m_ScreenSpace;
    private Vector3 m_Offset;
    private Vector3 m_HitPoint;
    public bool m_isDrag = false;
    public GameObject m_CanonPrefab;


    public GameObject m_MouseCanon;

    public bool m_Click = false;
    public GameObject m_Card;
    public TurnBaseManager m_TurnBaseManager;




    void Start()
    {
        m_Camera = Camera.main;
        m_TurnBaseManager = GameObject.Find("TurnBaseManager").GetComponent<TurnBaseManager>();
    }

    void Update()
    {
        //整体初始位置 
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
        //从摄像机发出到点击坐标的射线
        RaycastHit hitInfo;



//         if (Physics.Raycast(ray, out hitInfo))
//         {
//             //划出射线，只有在scene视图中才能看到
//             //Debug.DrawLine(ray.origin, hitInfo.point);
//             m_Land = hitInfo.collider.gameObject;
//             //print(btnName);
//             m_ScreenSpace = m_Camera.WorldToScreenPoint(m_Land.transform.position);
//             m_Offset = m_Land.transform.position - m_Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ScreenSpace.z));
//         }


        if(m_Click)
        {
            m_Click = false;
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ScreenSpace.z);
            Vector3 currentPosition = m_Camera.ScreenToWorldPoint(currentScreenSpace);
            m_MouseCanon = Instantiate(m_CanonPrefab, currentPosition, new Quaternion(0, 0, 0, 0)) as GameObject;
            m_MouseCanon.GetComponent<CanonLogic>().m_Active = false;
            m_MouseCanon.GetComponent<CanonLogic>().m_isDragged = true;
        }
        if(m_isDrag)
        {
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ScreenSpace.z);
            Vector3 currentPosition = m_Camera.ScreenToWorldPoint(currentScreenSpace);
            m_MouseCanon.transform.position = currentPosition;
            if (Physics.Raycast(ray, out hitInfo))
            {
                //划出射线，只有在scene视图中才能看到
                //Debug.DrawLine(ray.origin, hitInfo.point);
                m_Land = hitInfo.collider.gameObject;
                m_ScreenSpace = m_Camera.WorldToScreenPoint(m_Land.transform.position);
                m_Offset = m_Land.transform.position - m_Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ScreenSpace.z));
                m_HitPoint = hitInfo.point;
            }
        }

        if (m_isDrag && Input.GetMouseButtonDown(0) && m_Land && m_Land.tag == "Land")
        {
//             Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ScreenSpace.z);
//             Vector3 currentPosition = m_Camera.ScreenToWorldPoint(currentScreenSpace) + m_Offset;
            m_HitPoint.y += m_CanonPrefab.GetComponent<CapsuleCollider>().height / 2;
            //Debug.Log(m_HitPoint);
            GameObject newCanon = Instantiate(m_CanonPrefab, m_HitPoint, new Quaternion(0, 0, 0, 0)) as GameObject;
            CanonLogic logic= newCanon.GetComponent<CanonLogic>();
            
            SetCanonAttribute(logic, m_Card);
            m_TurnBaseManager.m_Canon.Add(newCanon);
            
            Destroy(m_MouseCanon);
            //m_Land.GetComponent<LevelLogic>().pet = Instantiate(pet, m_Land.GetComponent<LevelLogic>().Spawn_Point.transform) as GameObject;
            m_isDrag = false;
            m_TurnBaseManager.DeleteCardFromDeck(m_Card);
        }      
    }

    void SetCanonAttribute(CanonLogic logic, GameObject card)
    {
        CanonAttribute attribute = card.GetComponent<CardLogic>().m_CanonAttribute;
        logic.m_Active = false;
        logic.m_DirectionCnt = attribute.Direction;
        logic.m_MaxHealth = attribute.HP;
        logic.m_InstantSpawn = attribute.InstantSpawn;
        logic.m_DoubleSpeed = attribute.DoubleSpeed;
        logic.m_DoubleAtk = attribute.DoubleAtk;
        logic.m_DoubleFrequency = attribute.DoubleFrequency;

    }
}

