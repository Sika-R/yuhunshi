using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    [SerializeField]
    public float m_BulletLifeTime = 2.0f; //edited by Chris

    // The speed of the bullet
    [SerializeField]
    public float m_BulletSpeed = 15.0f;

    [SerializeField]
    float m_NoTriggerTime = 0.05f;
    float m_time = 0;

    public GameObject m_Canon;
    public float m_Attack;

    // Use this for initialization
    void Start()
    {
        // Add velocity to the bullet
        GetComponent<Rigidbody>().velocity = transform.forward * m_BulletSpeed;
        GetComponent<Collider>().enabled = false;
    }

    // Update is called once per frame
    void Update ()
    {
    	//前0.1s不做判定
    	if(m_time >= m_NoTriggerTime)
    	{
    		GetComponent<Collider>().enabled = true;
    	}


    	m_time += Time.deltaTime;

        m_BulletLifeTime -= Time.deltaTime;
        if(m_BulletLifeTime < 0.0f)
        {
            Impact();
        }
    }

    void Impact()
    {
        Destroy(gameObject);
    }

    public void Hit()
    {
        m_Canon.GetComponent<CanonLogic>().m_Duration = m_Canon.GetComponent<CanonLogic>().m_DurationTime;

    }


}
