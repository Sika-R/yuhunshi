using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonLogic : MonoBehaviour
{
	//射出的子弹模型
	public GameObject m_BulletPrefab;

	//向n个方向发射炮弹
	[SerializeField]
	int m_DirectionCnt;

	List<Vector3> m_SpawnPoints = new List<Vector3>();
	List<Quaternion> m_SpawnRotations = new List<Quaternion>();

	//生成子弹的频率
	[SerializeField]
	float m_InitFrequency;
	float m_Frequency;

	
	[SerializeField]
	float m_bonusTime = 10;
	float m_cd = -1;
	float m_time = 0;

	//子弹初始速度
	[SerializeField]
	float m_InitSpeed;
	float m_Speed;

	//子弹攻击力
	[SerializeField]
	float m_InitBulletAtk;
	float m_BulletAtk;

	//炮台的半径
	float m_CanonRadius;

	[SerializeField]
	//击中后是否立即生成一次炮弹
	public bool m_InstantSpawn;
	//接下来立即生成的次数
	int m_NextInstantSpawn;

	[SerializeField]
	//击中后下一次是否速度加倍
	public bool m_DoubleSpeed;
	//接下来速度加倍的次数
	int m_NextDoubleSpeed = 0;

	[SerializeField]
	//击中后下一次是否攻击加倍
	public bool m_DoubleAtk;
	//接下来攻击加倍的次数
	int m_NextDoubleAtk = 0;

	[SerializeField]
	//击中后十秒是否频率加倍
	public bool m_DoubleFrequency;



    // Start is called before the first frame update
    void Start()
    {
    	m_Frequency = m_InitFrequency;
    	m_Speed = m_InitSpeed;
    	m_BulletAtk = m_InitBulletAtk;
    	//得有capsulecollider才可以这么获得数据
    	m_CanonRadius = GetComponent<CapsuleCollider>().radius;


        Vector3 initSpawnPoint = transform.position + m_CanonRadius * transform.forward;
        m_SpawnPoints.Add(initSpawnPoint);
        m_SpawnRotations.Add(Quaternion.Euler(transform.forward));
        Vector3 nextDirection = transform.forward;
        Quaternion nextRotation = transform.rotation;
        for(int i = 0; i < m_DirectionCnt - 1; ++i)
        {
        	Quaternion q = Quaternion.AngleAxis(360 / m_DirectionCnt, transform.up);
        	nextDirection = q * nextDirection;
        	nextRotation = q * nextRotation;
        	m_SpawnPoints.Add(transform.position + m_CanonRadius * nextDirection);
        	m_SpawnRotations.Add(nextRotation);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if(m_time <= 0)
        {
        	Vector3 initSpawnPoint = transform.position + m_CanonRadius * transform.forward;
		    
		    Vector3 pos = initSpawnPoint;
		    Vector3 nextDirection = transform.forward;
		    Quaternion nextRotation = transform.rotation;
		    for(int i = 0; i < m_DirectionCnt; ++i)
		    {
		    	GameObject newBullet = Instantiate(m_BulletPrefab, pos, nextRotation) as GameObject;
	        	BulletLogic newBulletLogic = newBullet.GetComponent<BulletLogic>();
	        	newBulletLogic.m_Canon = gameObject;
	        	if(m_NextDoubleAtk > 0)
	        	{
					newBulletLogic.m_Attack = m_BulletAtk * 2;
					m_NextDoubleAtk--;
	        	}
	        	if(m_NextDoubleSpeed > 0)
	        	{
	        		newBulletLogic.m_BulletSpeed *= 2;
	        		m_NextDoubleSpeed--;
	        	}


		    	Quaternion q = Quaternion.AngleAxis(360 / m_DirectionCnt, transform.up);
		    	nextDirection = q * nextDirection;
		    	nextRotation = q * nextRotation;
		    	pos = transform.position + m_CanonRadius * nextDirection;
		    }

	        m_time = m_Frequency;
        }

        if(m_cd == m_bonusTime)
        {
        	m_Frequency = m_Frequency / 2;
        	m_time -= m_Frequency;
        }
        else if(m_cd <= 0)
        {
        	m_Frequency = m_InitFrequency;
        }

        m_cd -= Time.deltaTime;
        m_time -= Time.deltaTime;

    }

    //子弹射出去之前会和自身的炮台发生反应，现在的解决方案是射出时前0.1s不要判定
    void OnTriggerEnter(Collider other)
    {
    	
    	if(other.tag == "Bullet")
    	{
    		CanonLogic OtherCanon = other.gameObject.GetComponent<BulletLogic>().m_Canon.GetComponent<CanonLogic>();
    		//直接发射一次炮弹
    		if(OtherCanon.m_InstantSpawn)
    		{
    			m_time = 0;
    		}

    		//速度加倍
    		if(OtherCanon.m_DoubleSpeed)
    		{

    			//m_Speed = m_Speed * 2;
    			m_NextDoubleSpeed++;
    		}

			//攻击力加倍
    		if(OtherCanon.m_DoubleAtk)
    		{
    			//m_BulletAtk = m_BulletAtk * 2;
    			m_NextDoubleAtk++;
    		}

    		//十秒内攻击频率加倍
    		//m_Frequency = m_Frequency * 2;
    		if(OtherCanon.m_DoubleFrequency)
    		{
    			m_cd = m_bonusTime;
    		}
    		

    		//BulletLogic bulletLogic = other.gameObject.GetComponent<BulletLogic>();
    	}
    }
    

}
