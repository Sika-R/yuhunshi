using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	List<GameObject> m_Enemy = new List<GameObject>();
	//敌人出生的位置
	[SerializeField]
	float m_GenerateEnemyRadius = 360.0f;

	//开局计时器
	float m_Time = 0;

	//敌人生成计时器
	float m_EnemySpawnCd = 0;



	float m_ScreenWidth;
	float m_ScreenHeight;

	Vector3 m_NewEnemyPos;

	UIManager m_UIManager;


    // Start is called before the first frame update
    void Start()
    {
        m_ScreenHeight = UnityEngine.Screen.height;
        m_ScreenWidth = UnityEngine.Screen.width;
        //m_EnemySpawnCd = EnemySpawnFrequencyCalculation();
        m_NewEnemyPos = new Vector3(0, 0, 0);
        m_UIManager = GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
    	m_Time += Time.deltaTime;
    	m_UIManager.SetTimeText(m_Time);
        if(m_EnemySpawnCd <= 0)
        {
	int i = 0; 
	//根据游戏时常增加生成的敌人数量，每30秒增加1个
	for (; i<1+ (int)m_Time/30; i++)
	{
		SpawnNewEnemy();
	}
        	m_EnemySpawnCd = EnemySpawnFrequencyCalculation();
        }
        m_EnemySpawnCd -= Time.deltaTime;
    }

    void SpawnNewEnemy()
    {
    	float theta = Random.Range(0, 360) * Mathf.Deg2Rad;
    	//int theta = Random.Range(0, 360);
    	m_NewEnemyPos.x = Mathf.Sin(theta) * m_GenerateEnemyRadius + m_ScreenWidth / 2;
    	m_NewEnemyPos.y = Mathf.Cos(theta) * m_GenerateEnemyRadius + m_ScreenHeight / 2;

        Ray ray = Camera.main.ScreenPointToRay(m_NewEnemyPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
        	
        	Instantiate(m_Enemy[0], hit.point, Quaternion.Euler(0, 0, 0));
        }
    }
    //敌人生成频率计算函数
    float EnemySpawnFrequencyCalculation()
    {
        //初始是十秒一生成，m_Time是时间变量
        //修改为根据游戏开始时间逐渐递增，每过30秒-1，最低为5s
    	float result=10.0f - (int)m_Time/30;
	if(result < 5.0f)
	{
		result = 5.0f; 
	}
	return result;
    }
}
