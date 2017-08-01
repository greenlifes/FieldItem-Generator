using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CodeModel {

	public string title;
	public ModelPosition m_modelPosition;
	public List<ConditionList> m_conditionList;
	public string content;
	public string additionContent;

	public enum ModelPosition{
		Update,
		Damage
	}

	[System.Serializable]
	public class ConditionList{
		public int conditionContent;
		public int conditionValue;
	}

	public CodeModel(){
		title = "New Model";
		content = "Code incide Update/Damage";
		additionContent = "Function definition";

		m_modelPosition = ModelPosition.Damage;
		m_conditionList = new List<ConditionList> ();
		m_conditionList.Add (new ConditionList ());


	}
}
