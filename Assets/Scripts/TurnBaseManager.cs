using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class CanonAttribute
{
	public int Direction;
	public float HP;
	public bool InstantSpawn;
	public bool DoubleSpeed;
	public bool DoubleAtk;
	public bool DoubleFrequency;

}

public enum BattleState
{
	Start,
	PlayerTurn,
	EnemyTurn,
	Win,
	Lose
}

public class TurnBaseManager : MonoBehaviour
{
	BattleState m_State;
	//卡的prefab
	[SerializeField]
	GameObject m_CardPrefab;

	//最多能存多少卡
	[SerializeField]
	int m_CardMax;

	//每次补多少卡
	[SerializeField]
	int m_CardSupplement;

	//最开始有几张卡
	[SerializeField]
	int m_CardCnt = 2;

	//预设的卡牌组
	[SerializeField]
	List<CanonAttribute> m_PlayerDeck = new List<CanonAttribute>();
	//随机补充的卡牌
	List<GameObject> m_PlayerChoice = new List<GameObject>();

	GameObject m_Canvas;
	GameObject m_FinishedButton;
	Vector2 m_CardPos;
	ModelDrag m_ModelDrag;

	public List<GameObject> m_Canon = new List<GameObject>();
	[SerializeField]
	List<GameObject> m_EnemyPrefab = new List<GameObject>();
	List<GameObject> m_Enemy = new List<GameObject>();
	//敌人出生的位置
	[SerializeField]
	float m_GenerateEnemyRadius = 500.0f;

	float m_ScreenWidth;
	float m_ScreenHeight;

	Vector3 m_NewEnemyPos;

	int m_RoundCnt;



    // Start is called before the first frame update
    void Start()
    {
    	m_RoundCnt = 0;
    	m_ModelDrag = GetComponent<ModelDrag>();
    	m_ModelDrag.enabled = false;
    	m_State = BattleState.Start;
    	m_Canvas = GameObject.Find("Canvas").gameObject;
    	m_FinishedButton = m_Canvas.transform.Find("FinishedButton").gameObject;
    	m_FinishedButton.SetActive(false);
    	m_CardPos = new Vector2(UnityEngine.Screen.width * 0.15f, UnityEngine.Screen.height * 0.15f);
        m_ScreenHeight = UnityEngine.Screen.height;
        m_ScreenWidth = UnityEngine.Screen.width;
        m_NewEnemyPos = new Vector3(0, 0, 0);
        StartCoroutine(SetupBattle());
    }


    IEnumerator SetupBattle()
    {
    	m_ModelDrag.enabled = false;
    	m_FinishedButton.SetActive(false);

    	for(int i = 0; i < m_Canon.Count; i++)
    	{
    		m_Canon[i].GetComponent<CanonLogic>().m_Health -= 1;
    		m_Canon[i].GetComponent<CanonLogic>().m_Active = false;
    	}


    	for(int i = 0; i < SpawnEnemyCnt(); i++)
    	{
    		SpawnNewEnemy();
    		yield return new WaitForSeconds(0.3f);
    	}

    	yield return new WaitForSeconds(1f);

    	for(int i = 0; i < m_CardCnt; i++)
        {
        	AddCardToDeck();
        	//AddCardToDeck();
        	yield return new WaitForSeconds(0.5f);
        }

        yield return 0;
        m_State = BattleState.PlayerTurn;
        StartCoroutine(PlayerTurn());
    }


    IEnumerator PlayerTurn()
    {
    	m_ModelDrag.enabled = true;
    	m_FinishedButton.SetActive(true);
    	Debug.Log("PlayerTurn");
    	yield return new WaitForSeconds(2f);
    }

    IEnumerator FightTurn()
    {
    	m_ModelDrag.enabled = false;
    	m_FinishedButton.SetActive(false);
    	for(int i = 0; i < m_Enemy.Count; i++)
    	{
    		m_Enemy[i].GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;
    	}
    	for(int i = 0; i < m_Canon.Count; i++)
    	{
    		m_Canon[i].GetComponent<CanonLogic>().m_Active = true;
    	}

    	
    	yield return new WaitUntil(() => m_Enemy.Count == 0);
    	StartCoroutine(SetupBattle());

    	
    }


    void AddCardToDeck()
    {    	
		GameObject newCard = Instantiate(m_CardPrefab, m_CardPos, transform.rotation) as GameObject;
    	m_CardPos.x += newCard.GetComponent<RectTransform>().rect.width + UnityEngine.Screen.width * 0.02f;
    	newCard.transform.SetParent(m_Canvas.transform);
    	m_PlayerChoice.Add(newCard);
    	if(m_PlayerDeck.Count > 0)
    	{
    		CanonAttribute attribute = m_PlayerDeck[Random.Range(0, m_PlayerDeck.Count)];
    		newCard.GetComponent<CardLogic>().m_CanonAttribute = attribute;
    		newCard.transform.Find("Direction").gameObject.GetComponent<Text>().text = attribute.Direction + " direction";
    		newCard.transform.Find("HP").gameObject.GetComponent<Text>().text = "HP: " + attribute.HP;
    		Text effect = newCard.transform.Find("Effect").gameObject.GetComponent<Text>();
    		effect.text = "";
    		effect.text += attribute.InstantSpawn ? "立即发射 " : "" ;
    		effect.text += attribute.DoubleSpeed ? "速度加倍 " : "" ;
    		effect.text += attribute.DoubleAtk ? "攻击加倍 " : "" ;
    		effect.text += attribute.DoubleFrequency ? "频率加倍 " : "" ;
    		if(effect.text == "")
    		{
    			effect.text = "无特殊效果";
    		}

    	}

    }

    public void DeleteCardFromDeck(GameObject Card)
    {
    	//m_PlayerChoice.Delete(Card);
    	for(int i = m_PlayerChoice.IndexOf(Card); i < m_PlayerChoice.Count; i++)
    	{
    		Vector3 newPos = m_PlayerChoice[i].transform.position;
    		newPos.x -= Card.GetComponent<RectTransform>().rect.width + UnityEngine.Screen.width * 0.02f;
    		m_PlayerChoice[i].transform.position = newPos;
    	}
    	m_CardPos.x -= m_PlayerChoice[m_PlayerChoice.Count - 1].GetComponent<RectTransform>().rect.width + UnityEngine.Screen.width * 0.02f;
    	m_PlayerChoice.RemoveAt(m_PlayerChoice.IndexOf(Card));
    	Destroy(Card);
    }

    public void FinishPlayerTurn()
    {
    	StartCoroutine(FightTurn());
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
        	GameObject newEnemy = Instantiate(m_EnemyPrefab[0], hit.point, Quaternion.Euler(0, 0, 0)) as GameObject;
        	newEnemy.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
        	m_Enemy.Add(newEnemy);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        m_Enemy.RemoveAt(m_Enemy.IndexOf(enemy));
    }

    public void RemoveCanon(GameObject canon)
    {
        m_Canon.RemoveAt(m_Canon.IndexOf(canon));
    }

    int SpawnEnemyCnt()
    {
    	//m_RoundCnt related
    	return 5;
    }
}
