using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Move,
    Blocked
}
public class TurnBaseEnemyLogic : MonoBehaviour
{
    public float m_atk;
    float m_currentAtk;
    public float m_freq;
    float m_currentFreq;
    float m_t;


    public int m_shootRange;//射程
    public bool m_ignoreWall;
    //Collider[] m_Collider;

    public int m_splashRange;//攻击范围
    public List<GameObject> m_operator = new List<GameObject>();
    List<GameObject> m_enemy = new List<GameObject>();

    public Buff m_buff = Buff.None;

    public float m_debuff;
    float m_speedDebuff;
    float m_atkCdDebuff;
    public float m_debuffTime;
    float m_debuffTimer;

    [SerializeField]
    float m_Speed;



    [SerializeField]
    float m_maxHealth;
    float m_health;




    public Texture2D blood_red;
    public Texture2D blood_black;

    public Transform m_Destination;

    GameObject m_currentEnemy;
    float m_gridSize = 1;
    public GameObject m_bullet;

    UnityEngine.AI.NavMeshAgent m_NavMeshAgent;
    float m_EnemyHeight;
    GameObject m_Camera;
    GameObject m_TurnBaseManager;

    // Start is called before the first frame update
    void Start()
    {
        m_speedDebuff = m_debuff;
        m_atkCdDebuff = m_debuff;
        m_debuffTimer = 0.0f;
        m_currentAtk = m_atk;
        m_t = m_freq;
        m_Destination = GameObject.Find("Grid").transform.GetChild(0);
        m_Camera = GameObject.Find("Main Camera");
        m_TurnBaseManager = GameObject.Find("TurnBaseManager");
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        

        m_NavMeshAgent.SetDestination(m_Destination.position);
        
        m_NavMeshAgent.speed = m_Speed;   
        float size_y = GetComponent<Collider>().bounds.size.y;
        float scal_y = transform.localScale.y;
        m_EnemyHeight = (size_y *scal_y) ;
        m_health = m_maxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_debuffTimer -= Time.fixedDeltaTime;
        m_t -= Time.fixedDeltaTime;
        if(m_debuffTimer <= 0)
        {
            m_currentFreq = m_freq;
            m_NavMeshAgent.speed = m_Speed;
        }

        if(m_t < 0 && m_operator.Count > 0)
        {
            m_currentEnemy = m_operator[0];
            m_t = m_freq;
            if(m_splashRange == 0)
            {
                SplashAtk(gameObject, m_atk, m_shootRange);
            }
            else
            {
                AttackEnemy(gameObject, gameObject, m_currentEnemy, m_atk);
                //SplashAtk(m_currentEnemy, m_atk, m_shootRange);
            }
        }
        
        
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
        float blood_width = 30 * m_health / m_maxHealth;
        //先绘制黑色血条
        GUI.DrawTexture(new Rect(position.x - (bloodSize.x / 2),position.y - bloodSize.y ,bloodSize.x,bloodSize.y),blood_black);
        //在绘制红色血条
        GUI.DrawTexture(new Rect(position.x - (bloodSize.x / 2),position.y - bloodSize.y ,blood_width,bloodSize.y),blood_red);
        //HP数值
        string name = "HP: " + m_health;
        GUI.color  = Color.white;
        Vector2 nameSize = GUI.skin.label.CalcSize (new GUIContent(name));
        GUI.Label(new Rect(position.x - (nameSize.x / 2),position.y - nameSize.y - bloodSize.y ,nameSize.x,nameSize.y), name);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            if(other.gameObject.GetComponent<TurnBaseBulletLogic>().m_target.gameObject == gameObject)
            {
                HitByBullet(other.gameObject.GetComponent<TurnBaseBulletLogic>());

                Destroy(other.gameObject);

            }

            
        }

        if(other.tag == "Operator")
        {
            m_operator.Add(other.gameObject);
        }

        if(other.tag == "Enemy")
        {
            m_enemy.Add(other.gameObject);
        }

        // if(m_health <= 0)
        // {
        //     if(m_TurnBaseManager)
        //     {
        //         m_TurnBaseManager.GetComponent<TurnBaseManager>().RemoveEnemy(gameObject);
        //     }
        //     Destroy(gameObject);
        // }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            m_operator.Remove(other.gameObject);
        }

        if(other.tag == "Enemy")
        {
            m_enemy.Remove(other.gameObject);
        }
    }

    void HitByBullet(TurnBaseBulletLogic logic)
    {
        logic.Hit();
        m_health -= logic.m_atk;
        if(m_health <= 0)
        {
            logic.DestroyEnemy(gameObject);
            if(m_TurnBaseManager)
            {
                m_TurnBaseManager.GetComponent<TurnBaseManager>().RemoveEnemy(gameObject);
            }
            Destroy(gameObject);
        }
        // if(logic.m_operator.m_buff == Buff.None)
        // {
        //     m_health -= logic.m_atk;
        // }
        if(logic.m_operator && logic.m_operator.m_buff == Buff.Decelerate)
        {
            m_NavMeshAgent.speed = logic.m_operator.m_debuff * m_Speed;
            if(logic.m_operator)
            {
                m_currentFreq = logic.m_operator.m_debuff * m_freq;
                m_debuffTimer = logic.m_operator.m_debuffTime;
            }
            
        }
        else if(logic.m_enemy && logic.m_enemy.m_buff == Buff.Decelerate)
        {
            m_NavMeshAgent.speed = logic.m_enemy.m_debuff * m_Speed;
            if(logic.m_enemy)
            {
                m_currentFreq = logic.m_enemy.m_debuff * m_freq;
                m_debuffTimer = logic.m_enemy.m_debuffTime;
            }
            
        }
        
    }

    void SplashAtk(GameObject target, float atk, int r)
    {
        float len = (2 * r - 1) * m_gridSize;
        Collider[] enemies = Physics.OverlapBox(target.transform.position, new Vector3(len, 0.5f,len), target.transform.rotation, LayerMask.GetMask("Operator"));
        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i].gameObject != target)
            {
                AttackEnemy(gameObject, target, enemies[i].gameObject, atk, true);
            }
            
        }
    }


    public void AttackEnemy(GameObject oprt, GameObject from, GameObject enemy, float atk, bool isSplash = false)
    {
        GameObject bullet = Instantiate(m_bullet, from.transform) as GameObject;
        TurnBaseBulletLogic logic = bullet.GetComponent<TurnBaseBulletLogic>();
        logic.m_enemy = oprt.GetComponent<TurnBaseEnemyLogic>();
        logic.m_target = enemy.transform;
        logic.m_atk = atk;
        logic.m_isSplash = isSplash;
    }

    public void StartSplash(GameObject target)
    {
        SplashAtk(target, m_atk, m_shootRange);
    }
}
