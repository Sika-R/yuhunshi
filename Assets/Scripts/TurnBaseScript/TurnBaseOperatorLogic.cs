using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Buff
{
    None, Decelerate,
}

public class TurnBaseOperatorLogic : MonoBehaviour
{
    GameObject m_camera;
    float m_gridSize = 1;

    public float m_atk;
    float m_currentAtk;
    public float m_freq;
    float m_currentFreq;

    float m_t;

    public int m_shootRange;//射程
    //Collider[] m_Collider;

    public int m_splashRange;//攻击范围
    public List<GameObject> m_enemy = new List<GameObject>();
    public GameObject m_currentEnemy;

    public Buff m_buff = Buff.None;

    public float m_debuff;
    public float m_debuffTime;
    float m_speedDebuff;
    float m_atkCdDebuff;
    float m_debuffTimer;

    [SerializeField]
    float m_maxHealth;
    float m_health;

    public Texture2D blood_red;
    public Texture2D blood_black;

    public bool m_isBlock;
    int m_maxBlock;

    [SerializeField]
    GameObject m_bullet;
    // public bool m_isThreat;
    // public bool m_isRoad;
    // public int m_gridCnt;
    float m_height;
    GameObject m_TurnBaseManager;







    // Start is called before the first frame update
    void Start()
    {
        m_t = 0.5f;
        m_speedDebuff = m_debuff;
        m_atkCdDebuff = m_debuff;
        m_debuffTimer = 0.0f;
        m_currentAtk = m_atk;
        if(m_isBlock)
        {
            m_maxBlock = 100;
        }
        else
        {
            m_maxBlock = 0;
        }
        m_camera = GameObject.Find("Main Camera");
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.size = new Vector3((2 * m_shootRange + 1) * m_gridSize, collider.bounds.size.y, (2 * m_shootRange + 1) * m_gridSize);
        float size_y = GetComponent<Collider>().bounds.size.y;
        float scal_y = transform.localScale.y;
        m_height = (size_y *scal_y) ;
        m_health = m_maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(m_t <= 0 && m_enemy.Count > 0)
        {
            m_currentEnemy = m_enemy[0];
            m_t = m_freq;
            if(m_splashRange == 0)
            {
                SplashAtk(gameObject, m_atk, m_splashRange);
            }
            else
            {
                AttackEnemy(gameObject, gameObject, m_currentEnemy, m_atk);
                //SplashAtk(m_currentEnemy, m_atk, m_splashRange);
            }
        }
        m_t -= Time.fixedDeltaTime;
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

        if(other.tag == "Enemy")
        {
            m_enemy.Add(other.gameObject);
            // if(m_buff == Buff.Decelerate)
            // {
            //     DecelerteEnemy(other.gameObject, m_speedDebuff, m_atkCdDebuff, m_debuffTime);
            // }
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Enemy")
        {
            m_enemy.Remove(other.gameObject);
        }
        
    }


    void OnGUI()
    {
        Vector3 worldPosition = new Vector3 (transform.position.x , transform.position.y + m_height,transform.position.z);
        //根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标
        Vector2 position = m_camera.GetComponent<Camera>().WorldToScreenPoint (worldPosition);
        //得到真实NPC头顶的2D坐标
        position = new Vector2 (position.x, Screen.height - position.y + 10);
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

    void SplashAtk(GameObject target, float atk, int r)
    {
        float len = (2 * r - 1) * m_gridSize;
        Collider[] enemies = Physics.OverlapBox(target.transform.position, new Vector3(len, 0.5f,len), target.transform.rotation, LayerMask.GetMask("Enemy"));

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
        logic.m_operator = oprt.GetComponent<TurnBaseOperatorLogic>();
        logic.m_target = enemy.transform;
        logic.m_atk = atk;
        logic.m_isSplash = isSplash;
    }

    public void StartSplash(GameObject target)
    {
        SplashAtk(target, m_atk, m_splashRange);
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
            TurnBaseBlockLogic blockLogic = transform.GetChild(0).gameObject.GetComponent<TurnBaseBlockLogic>();
            for(int i = 0; i < blockLogic.m_blocked.Count; i++)
            {
                blockLogic.m_blocked[i].GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;
            }
            Destroy(gameObject);

        }
        // if(logic.m_operator.m_buff == Buff.None)
        // {
        //     m_health -= logic.m_atk;
        // }
       
        
    }

    // public void DecelerteEnemy(GameObject enemy, float spdDebuff, float freqDebuff, float t)
    // {
    //     Debug.Log(m_debuff);
    // }
}
