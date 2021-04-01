using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    UIManager m_UIManager;

	[SerializeField]
    public float m_HealthMax = 100;
    float m_Health;
    // Start is called before the first frame update
    void Start()
    {
        m_Health = m_HealthMax;
        GameObject GameManager = GameObject.Find("GameManager");
        if(GameManager)
        {
            m_UIManager = GameManager.GetComponent<UIManager>();
        }
        else
        {
            m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        }
        m_UIManager.SetHP(m_Health);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerHurt(float damage)
    {
    	m_Health -= damage;
        m_UIManager.SetHP(m_Health);
    }
}
