using UnityEngine;
using System.Collections;

public class BackGroundManager : UnitySingleton<BackGroundManager> {
	public Camera mainCamera;
	[HideInInspector]
	public float cameraWidth;
	[HideInInspector]
	public float cameraHalfWidth;
	// Use this for initialization
	void Awake () {
		base.Awake ();
		cameraWidth =  (mainCamera.ViewportToWorldPoint (Vector3.right) - mainCamera.ViewportToWorldPoint (Vector3.zero)).x;
		cameraHalfWidth = cameraWidth / 2f;
	}
	public GameObject CloneBackGround(GameObject source)
	{
		GameObject father = new GameObject ();
		GameObject child;
		SpriteRenderer rendererTemp;
		father.transform.position = source.transform.position;
		father.transform.localScale = source.transform.localScale;
		father.transform.rotation = source.transform.rotation;
		rendererTemp = source.GetComponent<SpriteRenderer> ();
		if (rendererTemp != null) {
			SpriteRenderer sr = father.AddComponent<SpriteRenderer> ();
			sr.sprite = rendererTemp.sprite;
			sr.color = rendererTemp.color;
		}
		SpriteRenderer[] rendererArray =  source.GetComponentsInChildren<SpriteRenderer>(); 
		for (int i = 0; i < rendererArray.Length; i++) {
			if (rendererArray [i].gameObject == source)
				continue;
			child = new GameObject ();
			SpriteRenderer sr = child.AddComponent<SpriteRenderer> ();
			sr.sprite = rendererArray [i].sprite;
			sr.color = rendererArray [i].color;
			child.transform.SetParent (father.transform);
			child.transform.position = rendererArray [i].transform.position;
			child.transform.localScale = rendererArray [i].transform.localScale;
			child.transform.rotation = rendererArray [i].transform.rotation;

		}
		father.transform.SetParent (transform);
		return father;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
