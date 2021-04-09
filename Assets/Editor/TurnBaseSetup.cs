using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TurnBaseSetup : EditorWindow
{

	GameObject m_Floor;
	GameObject m_RouteRoot;
	GameObject m_FloorPrefab;


	[MenuItem("Generate/GenerateRoute")]
    static void SpawnRouteWindowInit()
    {
		EditorWindow window = GetWindow(typeof(TurnBaseSetup));
        window.Show();    	
    }
    // Start is called before the first frame update
    void OnGUI()
    {
   //  	if(!m_Floor)
   //  	{
			// GUILayout.Label("请选择地面");
			// m_Floor = Selection.activeObject as GameObject;
		 //    GUILayout.Space(10);
   //  	}
   //  	if(m_Floor)
   //  	{
   //  		GUILayout.Label("已选择地面: " + m_Floor.name);

   //  		if(GUILayout.Button("重新选择"))
			// {
			// 	m_Floor = Selection.activeObject as GameObject;
			// }

		if(!m_RouteRoot)
		{
			GUILayout.Label("请选择路径根节点");
			m_RouteRoot = Selection.activeObject as GameObject;
	    	GUILayout.Space(10);
		}
		if(m_RouteRoot)
		{
			GUILayout.Label("已选择路径根节点: " + m_RouteRoot.name);

    		if(GUILayout.Button("重新选择"))
			{
				m_RouteRoot = Selection.activeObject as GameObject;
			}

			GUILayout.Label("该节点下共有" + m_RouteRoot.transform.childCount + "条路径");

			if(!m_FloorPrefab)
			{
				GUILayout.Label("请选择地板样式");
				m_FloorPrefab = Selection.activeObject as GameObject;
	    		GUILayout.Space(10);
			}

			if(m_FloorPrefab)
			{
				GUILayout.Label("已选择地板样式: " + m_FloorPrefab.name);

	    		if(GUILayout.Button("重新选择"))
				{
					m_FloorPrefab = Selection.activeObject as GameObject;
				}

				if(GUILayout.Button("生成路径"))
				{
					for(int i = 0; i < m_RouteRoot.transform.childCount; i++)
					{
						GameObject route = m_RouteRoot.transform.GetChild(i).gameObject;
						RouteController controller = route.GetComponent<RouteController>();
						controller.GenerateFloor(m_FloorPrefab);
					}
				}


				
			}

			
		}

			


    	// }
    }



    

   
}
