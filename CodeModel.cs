using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CodeModel {

	public string title;
	public ModelPosition m_modelPosition;
	public int m_condition;
	public List<Variable> variableList;
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
		variableList = new List<Variable> ();
		m_modelPosition = ModelPosition.Damage;
		m_condition = 0;


	}
	[System.Serializable]
	public class Variable{
		public bool isPublic;
		public string type;
		public string varName;
	}
}
