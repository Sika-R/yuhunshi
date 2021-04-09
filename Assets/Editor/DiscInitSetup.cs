using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DiscInitSetup : EditorWindow
{
	string m_numObjects;

	GameObject m_Center;


	List<string> m_radiusString = new List<string>();
	List<float> m_radius = new List<float>();
	bool m_radiusCheck = true;


	[MenuItem("Generate/GenerateDisc")]
    static void SpawnObjectsWindowInit()
    {
        EditorWindow window = GetWindow(typeof(DiscInitSetup));
        window.Show();
    }

    // Start is called before the first frame update
    void OnGUI()
    {
    	if(!m_Center)
    	{
    		GUILayout.Label("选择一下圆盘生成的中心");
			m_Center = Selection.activeObject as GameObject;
		    GUILayout.Space(10);

    	}

    	
    	if(m_Center)
    	{
    		GUILayout.Label("已选择中心: " + m_Center.name);

    		if(GUILayout.Button("重新选择生成中心"))
			{
				m_Center = null;
			}
    	
    		GUILayout.Label("要生成几个圆盘？");

	    	m_numObjects = GUILayout.TextField(m_numObjects);

	        GUILayout.Space(10);

	        int numObjects = 0;
	        if(!int.TryParse(m_numObjects, out numObjects))
	        {
	            return;
	        }
	        if(numObjects > 0)
	        {

	            GUILayout.Label("从小到大输入圆盘的半径");
	            
	            for(int i = 0; i < numObjects; i++)
	            {	
	            	if(m_radiusString.Count <= i)
	            	{
	            		m_radiusString.Add("0");
	            		m_radius.Add(0f);
	            	}
	            	m_radiusString[i] = EditorGUILayout.TextField("Radius of " + i, m_radiusString[i]);
	            	m_radius[i] = float.Parse(m_radiusString[i]);
	            }

	            //检查半径是不是从小到大
	            // for(int i = 1; i < numObjects; i++)
	            // {
	            // 	if(m_radius[i - 1] >= 0 && m_radius[i] >= 0 && m_radius[i] >= m_radius[i - 1])
	            // 	{
	            // 		m_radiusCheck = true;
	            		
	            // 	}
	            // 	else
	            // 	{
	            // 		m_radiusCheck = false;
	            // 		break;
	            // 	}
	            // }

	            if (m_radiusCheck)
                {
                	GUILayout.Label("请选择圆盘的prefab（暂时只能选一个）");
                	GameObject[] selectedGameObjects = Selection.gameObjects;
                	if (selectedGameObjects.Length > 0)
		            {
		                GUILayout.Label("已选择: " + GetSelectedObjectNames(selectedGameObjects));
		                if(GUILayout.Button("生成"))
		                {

							while(m_Center.transform.childCount > 0)
							{  
								Undo.DestroyObjectImmediate(m_Center.transform.GetChild(0).gameObject);
								//DestroyImmediate(m_Center.transform.GetChild(0).gameObject);  
							}

		                	Vector3 pos = m_Center.transform.position;
				            for(int i = 0; i < numObjects; i++)
				            {

				                GameObject NewDisc = Instantiate(selectedGameObjects[0], pos, m_Center.transform.rotation) as GameObject;
				                Undo.RegisterCreatedObjectUndo(NewDisc,"NewDisc");
				                //had to use mesh collider to make sure the raycast work
				                //float r = m_radius[i] / NewDisc.GetComponent<Collider>().bounds.max.x;
				                float r = m_radius[i];
				                NewDisc.transform.localScale = new Vector3(r, 0.1f, r);
				                NewDisc.transform.parent = m_Center.transform;
				                //两个disc的y轴距离
				                pos.y -= 0.1f;
				            }
		                }
		            }


                	
		            //Debug.Log("??");
                    //SpawnObjects(selectedGameObjects);
                    //Repaint();
                }


	        }
    	}

    	
    }

    string GetSelectedObjectNames(GameObject[] selectedGameObjects)
    {
        string nameList = "";

        for (int index = 0; index < selectedGameObjects.Length; ++index)
        {
            nameList += selectedGameObjects[index].name;

            if (index < selectedGameObjects.Length - 1)
            {
                nameList += ",";
            }
            else
            {
                nameList += ".";
            }
        }

        return nameList;
    }
}
