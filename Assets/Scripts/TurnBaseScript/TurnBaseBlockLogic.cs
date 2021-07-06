using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurnBaseBlockLogic : MonoBehaviour
{
    Collider m_collider;
    public List<GameObject> m_blocked = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            m_blocked.Add(other.gameObject);
        }
    }
}
