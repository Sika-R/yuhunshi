using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonLogic : MonoBehaviour
{
	//射出的子弹模型
	public GameObject m_BulletPrefab;

	//向n个方向发射炮弹
	[SerializeField]
	public int m_DirectionCnt;

	List<Vector3> m_SpawnPoints = new List<Vector3>();
	List<Quaternion> m_SpawnRotations = new List<Quaternion>();

	//生成子弹的频率
	[SerializeField]
	public float m_InitFrequency;
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

	//子弹持续时间，added by Chris
	[SerializeField]
	float m_BulletDuration; 

	//炮台的半径
	float m_CanonRadius;
    float m_CanonHeight;



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

    GameObject m_Camera;

    [SerializeField]
    public float m_MaxHealth;

    public float m_Health;


    public Texture2D blood_red;
    public Texture2D blood_black;


    [SerializeField]
    public float m_DurationTime;

    public float m_Duration;

    public bool m_Active = true;
    public bool m_isDragged = false;

    GameObject m_TurnBaseManager;





    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GameObject.Find("Main Camera");
        m_TurnBaseManager = GameObject.Find("TurnBaseManager");

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

        float size_y = GetComponent<Collider>().bounds.size.y;
        float scal_y = transform.localScale.y;
        m_CanonHeight = (size_y *scal_y) ;
        m_Health = m_MaxHealth;
        m_Duration = m_DurationTime;

    }

    // Update is called once per frame
    void Update()
    {
        if(m_Active)
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
	    newBulletLogic.m_BulletLifeTime = m_BulletDuration; //added by Chris
	    newBulletLogic.m_Attack = m_BulletAtk; //added by Chris
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
            m_Duration -= Time.deltaTime;

            if(m_Health <= 0)
            {
                Destroy(gameObject);
            }
            if(m_Duration < 0)
            {
                if(m_TurnBaseManager)
                {
                    m_TurnBaseManager.GetComponent<TurnBaseManager>().RemoveCanon(gameObject);
                }
                Destroy(gameObject);
            }
        }
        

    }

    //子弹射出去之前会和自身的炮台发生反应，现在的解决方案是射出时前0.1s不要判定
    void OnTriggerEnter(Collider other)
    {
    	if(other.tag == "Bullet")
    	{
            other.gameObject.GetComponentInParent<BulletLogic>().Hit();
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

    void OnGUI()
    {
        if(!m_isDragged)
        {
            Vector3 worldPosition = new Vector3 (transform.position.x , transform.position.y + m_CanonHeight,transform.position.z);
            //根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标
            Vector2 position = m_Camera.GetComponent<Camera>().WorldToScreenPoint (worldPosition);
            //得到真实NPC头顶的2D坐标
            position = new Vector2 (position.x, Screen.height - position.y);
            //注解2
            //计算出血条的宽高
            //Vector2 bloodSize = GUI.skin.label.CalcSize (new GUIContent(blood_red));
            Vector2 bloodSize = new Vector2(30, 5);
     
            //通过血值计算红色血条显示区域
            float blood_width = 30 * m_Health / m_MaxHealth;
            //先绘制黑色血条
            GUI.DrawTexture(new Rect(position.x - (bloodSize.x / 2),position.y - bloodSize.y ,bloodSize.x,bloodSize.y),blood_black);
            //在绘制红色血条
            GUI.DrawTexture(new Rect(position.x - (bloodSize.x / 2),position.y - bloodSize.y ,blood_width,bloodSize.y),blood_red);
            //HP数值
            string name = "HP: " + m_Health;
            if(m_InstantSpawn)
            {
                name = name + " 立即发射";
            }
            if(m_DoubleAtk)
            {
                name = name + " 加攻";
            }
            if(m_DoubleSpeed)
            {
                name = name + " 加速";
            }
            if(m_DoubleFrequency)
            {
                name = name + " 加频率";
            }
            GUI.color  = Color.white;
            Vector2 nameSize = GUI.skin.label.CalcSize (new GUIContent(name));
            GUI.Label(new Rect(position.x - (nameSize.x / 2),position.y - nameSize.y - bloodSize.y ,nameSize.x,nameSize.y), name);
        }
    }



}
