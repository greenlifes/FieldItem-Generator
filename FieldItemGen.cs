using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
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
	private List<ActiveComponent> components;
	private Displacement displacement = Displacement.None;
	private int displacementSelect = 0;
	private Timing damageActive = Timing.None;
	private TriggerCondition triggerCondition = TriggerCondition.noTrigger;
	private int[] damageSelect;
	private bool[] damageSelectOption;
	private int targeting;
	private int targeting_sub;
	private float destroy_duration;
	private Timing destroy;

//	private SerializedObject m_seObject;
//	private SerializedProperty m_seProperty;

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

		components = new List<ActiveComponent> ();

//		m_seObject = new SerializedObject (components);
//		m_seProperty = m_seObject.FindProperty ("componentList");

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


		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Component : ");
		if (GUILayout.Button ("+")) {
			components.Add (new ActiveComponent ());
		}
		if (GUILayout.Button ("-")) {
			if (components.Count > 0)
				components.RemoveAt (components.Count - 1);
		}
		EditorGUILayout.EndHorizontal ();
		foreach(ActiveComponent AC in components){
			AC.gameObject = (GameObject)EditorGUILayout.ObjectField ("\tGameObject:", AC.gameObject, typeof(GameObject), true);
			AC.active = (ActiveComponent.activeTiming)EditorGUILayout.EnumPopup ("\tActive:", AC.active);
			AC.deactive = (ActiveComponent.activeTiming)EditorGUILayout.EnumPopup ("\tDeActive:", AC.deactive);
		}

		displacement = (Displacement)EditorGUILayout.EnumPopup ("Displacement : ", displacement);
		if (nameList_update [(int)displacement].Length > 0) {
			displacementSelect = EditorGUILayout.Popup ("\tFunction : ", displacementSelect,  nameList_update [(int)displacement]);
		} else {
			GUILayout.Label ("\tNo Function");
			displacementSelect = -1;
		}

		damageActive = (Timing)EditorGUILayout.EnumPopup ("Damage Active : ", damageActive);

		triggerCondition = (TriggerCondition)EditorGUILayout.EnumPopup ("Trigger at : ", triggerCondition);

		if ((int)displacement >= 2)
			targeting = 1;
		targeting = EditorGUILayout.Popup ("Damage Target : ", targeting, new string[]{"InAreaMember", "InAreaTarget"});
		if (targeting == 0) {
			targeting_sub = EditorGUILayout.Popup ("\tat ", targeting_sub, new string[]{"Enter", "Stay"});
		}

		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Destroy in ");
		destroy_duration = EditorGUILayout.FloatField (destroy_duration);
		GUILayout.Label ("sec After ");
		destroy = (Timing)EditorGUILayout.EnumPopup (destroy);
		EditorGUILayout.EndHorizontal ();

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
		GameObject gen = Instantiate (Resources.Load<GameObject>("FIexample"));

		gen.name = fiTitle;

		foreach (ActiveComponent AC in components) {
			AC.gameObject.transform.parent = gen.transform.GetChild(0);
			if (AC.active == ActiveComponent.activeTiming.First)
				AC.gameObject.SetActive (true);
			else
				AC.gameObject.SetActive (false);
		}

		StringBuilder SB = new StringBuilder ();

		//namespace
		SB.Append(
			"using System.Collections;\n" +
			"using System.Collections.Generic;\n" +
			"using UnityEngine;\n"
		);
		//class name
		SB.Append("public class "+ fiTitle +" : MonoBehaviour {\n");

		//- class variable - - class variable - - class variable - - class variable - - class variable - - class variable - - class variable - - class variable - 
		SB.Append ("private bool isActive = false;\n");
		SB.Append ("private bool isTrigger = false;\n");
		foreach(ActiveComponent AC in components){
			SB.AppendFormat("public GameObject {0};\n", AC.gameObject.name);
		}
		if ((int)displacement != 0) {
			CodeModel cm = codeModelUpdate [(int)displacement] [displacementSelect];
			foreach (CodeModel.Variable vari in cm.variableList) {
				string pubpri = vari.isPublic ? "public" : "private";
				SB.AppendFormat ("{0} {1} {2};\n", pubpri, vari.type, vari.varName);
			}
		}
		for (int i = 0; i < damageSelect.Length; i++) {
			if (damageSelect [i] != -1) {
				CodeModel cm = codeModelDamageContent [i] [damageSelect [i]];
				foreach (CodeModel.Variable vari in cm.variableList) {
					string pubpri = vari.isPublic ? "public" : "private";
					SB.AppendFormat ("{0} {1} {2};\n", pubpri, vari.type, vari.varName);
				}
			}
		}

		if (damageActive == Timing.None) {
			SB.Append ("private bool damageActive = true;\n");
		} else {
			SB.Append ("private bool damageActive = false;\n");
		}

		if (triggerCondition == TriggerCondition.activeDelay) {
			SB.Append ("public float trigger_activeDelay;\n");
			SB.Append ("private float trigger_activeDelayTime;\n");
		}

		if (targeting == 1) {
			SB.Append ("private Transform InAreaTarget_target;\n");
		}
		if (targeting == 1 || targeting_sub == 1) {
			SB.Append ("public float targetingAttackInterval;\n" +
				"private float targetingAttackTime;\n");
		}
		if (destroy != Timing.None) {
			SB.AppendFormat ("public float duration = {0}f;\n", destroy_duration);
		}
		//- Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - 
		SB.Append("void Update () {\n");
		SB.Append("if(isActive){\n");
		if ((int)displacement != 0) {
			CodeModel cm = codeModelUpdate [(int)displacement] [displacementSelect];
			SB.Append(cm.content);
			SB.AppendLine ();
		}
		if (triggerCondition == TriggerCondition.activeDelay) {
			SB.Append ("if(!isTrigger && Time.time - trigger_activeDelayTime >= trigger_activeDelay)\n" +
				"\tTrigger();\n");
		}
		SB.Append("}\n");
		SB.Append("}\n");
		//- Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - 
		SB.Append("void Active (Vector3 info, BombGenerator.BombType requester) {\n");
		SB.Append ("isActive = true;\n");

		foreach (ActiveComponent AC in components) {
			if (AC.active == ActiveComponent.activeTiming.Active) {
				SB.AppendFormat ("{0}.SetActive (true);\n", AC.gameObject.name);
			} else if (AC.deactive == ActiveComponent.activeTiming.Active) {
				SB.AppendFormat ("{0}.SetActive (false);\n", AC.gameObject.name);
			}
		}
		if (damageActive == Timing.Active) {
			SB.Append ("damageActive = true;\n");
		}
		if (triggerCondition == TriggerCondition.activeDelay) {
			SB.Append ("trigger_activeDelayTime = Time.time;\n");
		}
		if (destroy != Timing.Active) {
			SB.Append ("Destroy (transform.parent.gameObject, duration);\n");
		}

		SB.Append("}\n");
		//- Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - 
		SB.Append("void Trigger () {\n");
		SB.Append ("isTrigger = true;\n");

		foreach (ActiveComponent AC in components) {
			if (AC.active == ActiveComponent.activeTiming.Trigger) {
				SB.AppendFormat ("{0}.SetActive (true);\n", AC.gameObject.name);
			} else if (AC.deactive == ActiveComponent.activeTiming.Trigger) {
				SB.AppendFormat ("{0}.SetActive (false);\n", AC.gameObject.name);
			}
		}
		if (damageActive == Timing.Trigger) {
			SB.Append ("damageActive = true;\n");
		}
		if (destroy != Timing.Trigger) {
			SB.Append ("Destroy (transform.parent.gameObject, duration);\n");
		}
		SB.Append("}\n");
		//- Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - 
		SB.Append("void Damage (NPCmovement target) {\n");
		SB.Append ("if(damageActive){\n");
		if (targeting == 1 || targeting_sub == 1) {
			SB.Append ("targetingAttackTime = Time.time;\n");
		}

		for (int i = 0; i < damageSelect.Length; i++) {
			if (damageSelect [i] != -1) {
				CodeModel cm = codeModelDamageContent [i] [damageSelect [i]];
				SB.Append(cm.content);
				SB.AppendLine ();
			}
		}

		SB.Append ("}\n");
		SB.Append("}\n");
		//- OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - 
		SB.Append("void OnTriggerEnter(Collider other){\n");
		SB.Append ("if(other.tag == \"minions\"){\n");

		if (triggerCondition == TriggerCondition.inCollison) {
			SB.Append ("if(!isTrigger)\n" +
						"Trigger();\n");
		}
		if(targeting == 0 && targeting_sub == 0){
			SB.Append("Damage(other.GetComponent<NPCmovement> ());\n");
		}

		SB.Append("}\n");
		SB.Append("}\n");
		//- OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - 
		SB.Append("void OnTriggerStay(Collider other){\n");
		SB.Append ("if(other.tag == \"minions\"){\n");

		if (targeting == 0 && targeting_sub == 1) {
			SB.Append ("if(Time.time - targetingAttackTime >= targetingAttackInterval)\n" +
				"Damage(other.GetComponent<NPCmovement> ());\n");
		} else if (targeting == 1) {
			SB.Append ("if(InAreaTarget_target == null || other.GetComponent<NPCmovement> ().IsDead){\n" +
				"InAreaTarget_target = other.transform;\n" +
				"}\n");
		}

		SB.Append("}\n");
		SB.Append("}\n");
		//- OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - 
		SB.Append("void OnTriggerExit(Collider other){\n");
		SB.Append ("if(other.tag == \"minions\"){\n");

		if (targeting == 1) {
			SB.Append ("if(InAreaTarget_target == other.transform){\n" +
				"InAreaTarget_target = null;\n" +
				"}\n");
		}

		SB.Append("}\n");
		SB.Append("}\n");
		//- Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - 
		SB.Append("void Arrived(){\n");

		if (triggerCondition == TriggerCondition.arrived) {
			SB.Append("Trigger();\n");
		}

		SB.Append("}\n");
		//- Extra - - Extra - - Extra - - Extra - - Extra - - Extra - - Extra - - Extra - - Extra - - Extra - - Extra - - Extra - - Extra - - Extra - 

		if ((int)displacement != 0) {
			CodeModel cm = codeModelUpdate [(int)displacement] [displacementSelect];
			SB.Append(cm.additionContent);
		}
		for (int i = 0; i < damageSelect.Length; i++) {
			if (damageSelect [i] != -1) {
				CodeModel cm = codeModelDamageContent [i] [damageSelect [i]];
				SB.Append(cm.additionContent);
			}
		}

		//class end
		SB.Append("}\n");

		FileStream file = File.Open (Application.dataPath + "/Editor/" + fiTitle + ".cs", FileMode.Create);
		StreamWriter SW = new StreamWriter (file);
		SW.Write (SB.ToString());
		SW.Flush ();
		file.Close();
	}
}
