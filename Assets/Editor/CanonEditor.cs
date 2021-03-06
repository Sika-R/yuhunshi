using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DiscLogic))]
public class CanonEditor : Editor 
{	
	string m_SlotCnt;

    // Start is called before the first frame update
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		DiscLogic discLogic = (DiscLogic)target;
		m_SlotCnt = EditorGUILayout.TextField("槽数量", m_SlotCnt);
		if(int.TryParse(m_SlotCnt, out discLogic.m_SlotCnt))
		{
			if(discLogic.m_CanonPrefab)
			{
				if(GUILayout.Button("生成"))
				{
					while(discLogic.transform.childCount > 0)
					{  
						Undo.DestroyObjectImmediate(discLogic.transform.GetChild(0).gameObject);
						//DestroyImmediate(m_Center.transform.GetChild(0).gameObject);  
					}

					float m_DiscRadius = discLogic.GetComponent<Collider>().bounds.max.x;
                	Vector3 pos = discLogic.transform.position;
		            Vector3 nextDirection = discLogic.transform.forward;
			        Quaternion nextRotation = discLogic.transform.rotation;
			        GameObject canon;
			        pos.y += 0.5f;

			        for(int i = 0; i < discLogic.m_SlotCnt; ++i)
			        {
			        	canon = Instantiate(discLogic.m_CanonPrefab, pos + (m_DiscRadius - 1.0f) * nextDirection, nextRotation) as GameObject;
			        	Undo.RegisterCreatedObjectUndo(canon,"NewCanon");
			        	canon.transform.parent = discLogic.transform;
			        	canon.transform.localScale = new Vector3(canon.transform.localScale.x, 2f, canon.transform.localScale.z);


			        	Quaternion q = Quaternion.AngleAxis(360 / discLogic.m_SlotCnt, discLogic.transform.up);
			        	nextDirection = q * nextDirection;
			        	nextRotation = q * nextRotation;

				        
			        }
				}
			}
		}
		

		
	}
}
