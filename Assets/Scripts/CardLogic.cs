using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLogic : MonoBehaviour
{
	public CanonAttribute m_CanonAttribute;
	GameObject m_TurnBaseManager;
	ModelDrag m_ModelDrag;
	public bool m_Clicked;
    // Start is called before the first frame update
    void Start()
    {
        m_TurnBaseManager = GameObject.Find("TurnBaseManager");
        m_ModelDrag = m_TurnBaseManager.GetComponent<ModelDrag>();
        m_Clicked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click()
    {
    	if(!m_ModelDrag.enabled)
    	{
    		return;
    	}
    	if(!m_Clicked)
    	{
    		if(m_ModelDrag.m_isDrag)
    		{
    			Destroy(m_ModelDrag.m_MouseCanon);
    		}
    		m_ModelDrag.m_Click = true;
    		m_ModelDrag.m_isDrag = true;
    		m_ModelDrag.m_Card = gameObject;
    		
    	}
    	else
    	{
    		Destroy(m_ModelDrag.m_MouseCanon);
    		m_ModelDrag.m_isDrag = false;
    	}
    	m_Clicked = !m_Clicked;
    	
    }
}
