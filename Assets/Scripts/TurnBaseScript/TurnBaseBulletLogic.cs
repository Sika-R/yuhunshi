using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBaseBulletLogic : MonoBehaviour
{
    public Transform m_target;

    [SerializeField]
    float m_velocity;
    public float m_atk;
    public bool m_isSplash;
    public TurnBaseOperatorLogic m_operator;
    public TurnBaseEnemyLogic m_enemy;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(m_target);
        transform.Translate(m_velocity * Vector3.forward * Time.fixedDeltaTime, Space.Self);
        if(!m_target)
        {
            Destroy(gameObject);
        }
        if(!m_operator && ! m_enemy)
        {
            Destroy(gameObject);

        }
    }

    public void Hit()
    {
        if(!m_isSplash)
        {
            if(m_operator)
            {
                GetComponentInParent<TurnBaseOperatorLogic>().StartSplash(m_target.gameObject);
            }
            else if(m_enemy)
            {
                GetComponentInParent<TurnBaseEnemyLogic>().StartSplash(m_target.gameObject);
            }
            
        }
    }

    public void DestroyEnemy(GameObject enemy)
    {
        if(m_operator)
        {
            m_operator.m_enemy.Remove(enemy);
        }
        if(m_enemy)
        {
            m_enemy.m_operator.Remove(enemy);
        }
    }


}
