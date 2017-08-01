using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class CodeModelBuilder : EditorWindow {

	[MenuItem("CustomEditor/FIcodeModel Builder")]
	public static void OpenWindow(){
		EditorWindow.GetWindow<CodeModelBuilder> ("CodeModel Builder");
	}

	public List<CodeModel> codeModelList;
	public string[] titleList;

	private int selected = 0;

	void OnEnable(){
		BinaryFormatter bf = new BinaryFormatter ();

		try{
			FileStream file = File.Open (Application.dataPath + "/Editor/CodeModel.dat", FileMode.Open);
			codeModelList = (List<CodeModel>)bf.Deserialize (file);
			file.Close ();
		}
		catch(System.IO.FileNotFoundException){
			codeModelList = new List<CodeModel>();
			codeModelList.Add (new CodeModel ());
		}

		ListUpdate ();
	}

	void OnGUI(){
		selected = EditorGUILayout.Popup ("Code Model : ", selected, titleList);

		if (codeModelList [selected] != null) {
			CodeModel model = codeModelList [selected];

			model.title = EditorGUILayout.TextField ("Title : ", model.title);
			model.m_modelPosition = (CodeModel.ModelPosition)EditorGUILayout.EnumPopup ("Update/Damage : ", model.m_modelPosition );
			if (model.m_modelPosition == CodeModel.ModelPosition.Update) {
				foreach (CodeModel.ConditionList cond in model.m_conditionList) {
					EditorGUILayout.BeginHorizontal ();
					FieldItemGen.Displacement temp = (FieldItemGen.Displacement)cond.conditionContent;
					temp = (FieldItemGen.Displacement)EditorGUILayout.EnumPopup ("condition : ", (FieldItemGen.Displacement)temp);
					cond.conditionContent = (int)temp;
					cond.conditionValue = EditorGUILayout.IntField ("value : ", cond.conditionValue);
					EditorGUILayout.EndHorizontal ();
				}
			} else {
				foreach (CodeModel.ConditionList cond in model.m_conditionList) {
					EditorGUILayout.BeginHorizontal ();
					FieldItemGen.DamageContent temp = (FieldItemGen.DamageContent)cond.conditionContent;
					temp = (FieldItemGen.DamageContent)EditorGUILayout.EnumPopup ("condition : ", (FieldItemGen.DamageContent)temp);
					cond.conditionContent = (int)temp;
					cond.conditionValue = EditorGUILayout.IntField ("value : ", cond.conditionValue);
					EditorGUILayout.EndHorizontal ();
				}
			}
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+")) {
				CodeModel.ConditionList newlist = new CodeModel.ConditionList ();
				model.m_conditionList.Add (newlist);
			}
			if (GUILayout.Button ("-")) {
				if (model.m_conditionList.Count > 1) {
					model.m_conditionList.RemoveAt (model.m_conditionList.Count - 1);
				}
			}
			EditorGUILayout.EndHorizontal ();
			GUILayout.Label ("Content : ");
			model.content = EditorGUILayout.TextArea (model.content);
			GUILayout.Label ("Additional Content : ");
			model.additionContent = EditorGUILayout.TextArea (model.additionContent);
		}

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+")) {
			codeModelList.Add (new CodeModel ());
			selected = codeModelList.Count - 1;
			ListUpdate ();
		}
		if (GUILayout.Button ("X")) {
			if (codeModelList.Count > 1) {
				codeModelList.RemoveAt (selected);
				selected = 0;
				ListUpdate ();
			}
		}
		EditorGUILayout.EndHorizontal ();

		if (GUILayout.Button ("SAVE")) {
			SaveFile ();
		}
	}

	void ListUpdate(){
		List<string> subList = new List<string> ();
		for(int i = 0 ; i < codeModelList.Count ; i++){
				subList.Add (i + " : " + codeModelList[i].title);
		}

		titleList = subList.ToArray ();
	}

	void SaveFile (){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.dataPath + "/Editor/CodeModel.dat", FileMode.Create);

		bf.Serialize (file, codeModelList);
		file.Close();
	}

}
