using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CodeModel {

	public string title;
	public ModelPosition m_modelPosition;
	public int m_condition;
	public string content;
	public string additionContent;

	public enum ModelPosition{
		Update,
		Damage
	}

	public CodeModel(){
		title = "New Model";
		content = "Code incide Update/Damage";
		additionContent = "Function definition";

		m_modelPosition = ModelPosition.Damage;
		m_condition = 0;


	}
}
