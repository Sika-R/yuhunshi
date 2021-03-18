using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
	[SerializeField]
    int m_Attack;

    [SerializeField]
    float m_MaxHealth;
    float m_Health;


    [SerializeField]
    float m_Speed;

   	public Texture2D blood_red;
	public Texture2D blood_black;

    Transform m_Destination;

    UnityEngine.AI.NavMeshAgent m_NavMeshAgent;
    float m_EnemyHeight;
    GameObject m_Camera;

    // Start is called before the first frame update
    void Start()
    {
    	m_Destination = GameObject.Find("Center").transform;
    	m_Camera = GameObject.Find("Main Camera");
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_NavMeshAgent.SetDestination(m_Destination.position);
        m_NavMeshAgent.speed = m_Speed;   
        float size_y = GetComponent<Collider>().bounds.size.y;
		float scal_y = transform.localScale.y;
		m_EnemyHeight = (size_y *scal_y) ;
		m_Health = m_MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
	{
		//得到NPC头顶在3D世界中的坐标
		//默认NPC坐标点在脚底下，所以这里加上m_EnemyHeight它模型的高度即可
		Vector3 worldPosition = new Vector3 (transform.position.x , transform.position.y + m_EnemyHeight,transform.position.z);
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
 		GUI.color  = Color.white;
 		Vector2 nameSize = GUI.skin.label.CalcSize (new GUIContent(name));
 		GUI.Label(new Rect(position.x - (nameSize.x / 2),position.y - nameSize.y - bloodSize.y ,nameSize.x,nameSize.y), name);
		
	}

    void OnTriggerEnter(Collider other)
    {
    	if(other.tag == "Bullet")
    	{
    		HitByBullet(other.gameObject.GetComponentInParent<BulletLogic>().m_Attack);
            other.gameObject.GetComponentInParent<BulletLogic>().Hit();
    	}

    	if(other.tag == "Player")
    	{
    		other.gameObject.GetComponentInParent<PlayerLogic>().PlayerHurt(m_Attack);
    		m_Health = 0;
    	}

    	if(m_Health <= 0)
    	{
    		Destroy(gameObject);
    	}
    }

    void HitByBullet(float atk)
    {
    	m_Health -= atk;
    	//速度减半
    	m_Speed /= 2;
    }
}
