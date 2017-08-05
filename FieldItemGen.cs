using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;

public class FieldItemGen : EditorWindow {

	[MenuItem("CustomEditor/FieldItem Generator")]
	public static void OpenWindow(){
		EditorWindow.GetWindow<FieldItemGen> ("FieldItem");
	}



	//Information member
	private string fiTitle;
	private int componentSize = 1;
	private ActiveCompList components;
	private Displacement displacement = Displacement.None;
	private int displacementSelect = 0;
	private Timing damageActive = Timing.None;
	private Timing AutoTarget = Timing.None;
	private TriggerCondition triggerCondition = TriggerCondition.noTrigger;
	private int[] damageSelect;
	private bool[] damageSelectOption;

	private SerializedObject m_seObject;
	private SerializedProperty m_seProperty;

	private List<CodeModel> codeModelList;

	private List<List<CodeModel>> codeModelUpdate;
	private List<List<CodeModel>> codeModelDamageContent;

	private List<string[]> nameList_update;
	private List<string[]> nameList_damageContent;
	public enum Displacement
	{
		None,
		Forward,
		Targeted_Forward,
		Parabolic
	}
	public enum Timing
	{
		None,
		Active,
		Trigger
	}
	public enum TriggerCondition
	{
		noTrigger,
		inCollison,
		activeDelay,
		arrived
	}
	public enum DamageContent
	{
		DirectDamage,
		SlowDown,
		Stun,
		Repel,
		DamageEnhance,
		Debuff
	}

	void OnEnable(){

		components = CreateInstance<ActiveCompList> ();

		m_seObject = new SerializedObject (components);
		m_seProperty = m_seObject.FindProperty ("componentList");

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

		InitialLists ();

		foreach (CodeModel m in codeModelList) {
			if (m.m_modelPosition == CodeModel.ModelPosition.Update) {
				codeModelUpdate [m.m_condition].Add (m);
			} else if (m.m_modelPosition == CodeModel.ModelPosition.Damage) {
				codeModelDamageContent [m.m_condition].Add (m);
			}
		}

		InitialNameLists ();

		damageSelect = new int[Enum.GetNames (typeof(DamageContent)).Length];
		for (int i = 0; i < damageSelect.Length; i++) {
			damageSelect[i] = -1;
		}

		damageSelectOption = new bool[damageSelect.Length];

	}

	void InitialLists(){
		int total = Enum.GetNames (typeof(Displacement)).Length;
		codeModelUpdate = new List<List<CodeModel>> ();
		for (int i = 0; i < total; i++) {
			codeModelUpdate.Add (new List<CodeModel> ());
		}

		total = Enum.GetNames (typeof(DamageContent)).Length;
		codeModelDamageContent = new List<List<CodeModel>> ();
		for (int i = 0; i < total; i++) {
			codeModelDamageContent.Add (new List<CodeModel> ());
		}
	}
	void InitialNameLists(){
		nameList_update = new List<string[]> ();
		nameList_damageContent = new List<string[]> ();
		List<string> temp;

		foreach (List<CodeModel> CML in codeModelUpdate) {
			temp = new List<string> ();
			foreach (CodeModel CM in CML) {
				temp.Add (CM.title);
			}
			nameList_update.Add (temp.ToArray());
		}

		foreach (List<CodeModel> CML in codeModelDamageContent) {
			temp = new List<string> ();
			foreach (CodeModel CM in CML) {
				temp.Add (CM.title);
			}
			nameList_damageContent.Add (temp.ToArray());
		}

	}
	void OnGUI(){
		
		GUILayout.Label ("Basic Information : ");
		fiTitle = EditorGUILayout.TextField ("ItemTitle", fiTitle);

		EditorGUILayout.PropertyField (m_seProperty, true);

		displacement = (Displacement)EditorGUILayout.EnumPopup ("Displacement : ", displacement);
		if (nameList_update [(int)displacement].Length > 0) {
			displacementSelect = EditorGUILayout.Popup ("\tFunction : ", displacementSelect,  nameList_update [(int)displacement]);
		} else {
			GUILayout.Label ("\tNo Function");
			displacementSelect = -1;
		}

		damageActive = (Timing)EditorGUILayout.EnumPopup ("Damage Active : ", damageActive);

		triggerCondition = (TriggerCondition)EditorGUILayout.EnumPopup ("Trigger at : ", triggerCondition);

		GUILayout.Label ("Damage Content : ");

		for (int i = 0; i < damageSelect.Length; i++) {
			damageSelectOption [i] = EditorGUILayout.Toggle (((DamageContent)i).ToString (), damageSelectOption [i]);
			if (damageSelectOption [i]) {
				if (nameList_damageContent [i].Length > 0) {
					damageSelect [i] = EditorGUILayout.Popup ("\tFunction : ", damageSelect [i], nameList_damageContent [i]);
				} else {
					GUILayout.Label ("\tNo Function");
					damageSelect [i] = -1;
				}
			} else {
				damageSelect [i] = -1;
			}
		}

		if (GUILayout.Button ("Generate!")) {
			Generate ();
		}





	}
	void Generate(){
	}
}
