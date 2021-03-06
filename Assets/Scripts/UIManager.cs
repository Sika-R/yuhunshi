using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
	[SerializeField]
	Text m_TimeText;

	[SerializeField]
	GameObject m_PlayerHP;

	Text m_HPText;
	Slider m_HPBar;

	[SerializeField]
	PlayerLogic m_PlayerLogic;
    // Start is called before the first frame update
    void Start()
    {

    	m_HPText = m_PlayerHP.transform.Find("HPText").gameObject.GetComponent<Text>();
    	m_HPBar = m_PlayerHP.transform.Find("HPBar").gameObject.GetComponent<Slider>();
    	m_HPBar.maxValue = m_PlayerLogic.m_HealthMax;
    	m_HPBar.minValue = 0;
        m_TimeText.text = "Time: 0s";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTimeText(float t)
    {
    	if (m_TimeText)
        {
            m_TimeText.text = "Time: " + t + "s";
        }
    }

    public void SetHP(float hp)
    {
    	if (m_HPText)
        {
            m_HPText.text = "HP: " + hp;
        }
        m_HPBar.value = hp;
    }



}
