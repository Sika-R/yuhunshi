using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


[AddComponentMenu("Route/Route Controller")]
public class RouteController : MonoBehaviour
{
	GameObject m_RouteRoot;
	Transform[] m_Transforms;
    // Start is called before the first frame update

	void Start()
	{
		// Transform[] trans = GetTransforms();
		// for(int i = 0; i < trans.Length; i++)
		// {
		// 	trans[i].gameObject.SetActive(false);
		// }

	}
	void OnDrawGizmos()
	{
		Transform[] trans = GetTransforms();
		if (trans.Length < 2)
			return;

		Vector3 prevPos = trans[0].position;
		for (int i = 1; i < trans.Length; i++)
		{
			Vector3 currPos = trans[i].position;
			Gizmos.color = new Color(1, 0, 0, 1);
			Gizmos.DrawLine(prevPos, currPos);
			prevPos = currPos;
		}
	}

	public void GenerateFloor(GameObject floor)
	{
		float length = floor.GetComponent<Renderer>().bounds.size.z;
		Transform[] trans = GetTransforms();
		if (trans.Length < 2)
			return;

		for (int i = 0; i < trans.Length - 1; i++)
		{
			Vector3 pos = (trans[i].position + trans[i + 1].position) / 2;
			pos.y = transform.position.y + 0.05f;
			float len = (trans[i + 1].position - trans[i].position).magnitude + floor.GetComponent<Renderer>().bounds.size.x;
			GameObject newFloor = Instantiate(floor, pos, Quaternion.LookRotation(trans[i].position - trans[i + 1].position)) as GameObject;
			Vector3 newScale = newFloor.transform.localScale;
			newScale.z *= len / length;
			newFloor.transform.localScale = newScale;
			newFloor.transform.SetParent(gameObject.transform);
			Undo.RegisterCreatedObjectUndo(newFloor,"NewFloor");
		}
	}

    Transform[] GetTransforms()
	{
		List<Component> components = new List<Component>(GetComponentsInChildren(typeof(Transform)));
		List<Transform> transforms = components.ConvertAll(c => (Transform)c);

		transforms.Remove(transform);
		transforms.Sort(delegate(Transform a, Transform b)
		{
			return a.name.CompareTo(b.name);
		});

		return transforms.ToArray();

	}
}
