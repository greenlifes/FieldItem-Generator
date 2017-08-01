using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveComponent {

	public enum activeTiming{
		First,
		Active,
		Trigger
	}

	public GameObject gameObject;
	public activeTiming active;
	public activeTiming deactive;


}
