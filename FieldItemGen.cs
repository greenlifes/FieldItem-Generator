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

		//categorize
		foreach (CodeModel m in codeModelList) {
			if (m.m_modelPosition == CodeModel.ModelPosition.Update) {
				codeModelUpdate [m.m_condition].Add (m);
			} else if (m.m_modelPosition == CodeModel.ModelPosition.Damage) {
				codeModelDamageContent [m.m_condition].Add (m);
			}
		}

		InitialNameLists ();

		//Init Array
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
		//
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
		//
		displacement = (Displacement)EditorGUILayout.EnumPopup ("Displacement : ", displacement);
		if (nameList_update [(int)displacement].Length > 0) {
			displacementSelect = EditorGUILayout.Popup ("\tFunction : ", displacementSelect,  nameList_update [(int)displacement]);
		} else {
			GUILayout.Label ("\tNo Function");
			displacementSelect = -1;
		}
		//
		damageActive = (Timing)EditorGUILayout.EnumPopup ("Damage Active : ", damageActive);
		//
		triggerCondition = (TriggerCondition)EditorGUILayout.EnumPopup ("Trigger at : ", triggerCondition);
		//
		if ((int)displacement >= 2)
			targeting = 1;
		targeting = EditorGUILayout.Popup ("Damage Target : ", targeting, new string[]{"InAreaMember", "InAreaTarget"});
		if (targeting == 0) {
			targeting_sub = EditorGUILayout.Popup ("\tat ", targeting_sub, new string[]{"Enter", "Stay"});
		}
		//
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Destroy in ");
		destroy_duration = EditorGUILayout.FloatField (destroy_duration);
		GUILayout.Label ("sec After ");
		destroy = (Timing)EditorGUILayout.EnumPopup (destroy);
		EditorGUILayout.EndHorizontal ();
		//

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
		//
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
		CodeWriter class_var = new CodeWriter();
		class_var.addHead (new string[]{
			"private bool isActive = false;",
			"private bool isTrigger = false;"
		});
		foreach(ActiveComponent AC in components){
			class_var += "public GameObject "+ AC.gameObject.name +";";
		}
		if ((int)displacement != 0) {
			CodeModel cm = codeModelUpdate [(int)displacement] [displacementSelect];
			foreach (CodeModel.Variable vari in cm.variableList) {
				string pubpri = vari.isPublic ? "public" : "private";
				class_var += pubpri + " " + vari.type + " " + vari.varName + ";";
			}
		}
		for (int i = 0; i < damageSelect.Length; i++) {
			if (damageSelect [i] != -1) {
				CodeModel cm = codeModelDamageContent [i] [damageSelect [i]];
				foreach (CodeModel.Variable vari in cm.variableList) {
					string pubpri = vari.isPublic ? "public" : "private";
					class_var += pubpri + " " + vari.type + " " + vari.varName + ";";
				}
			}
		}

		if (damageActive == Timing.None) {
			class_var += "private bool damageActive = true;";
		} else {
			class_var += "private bool damageActive = false;";
		}

		if (triggerCondition == TriggerCondition.activeDelay) {
			class_var += "public float trigger_activeDelay;";
			class_var += "private float trigger_activeDelayTime;";
		}

		if (targeting == 1) {
			class_var += "private Transform InAreaTarget_target;";
		}
		if (targeting == 1 || targeting_sub == 1) {
			class_var += "public float targetingAttackInterval;";
			class_var += "private float targetingAttackTime;";
		}
		if (destroy != Timing.None) {
			class_var += "public float duration = " + destroy_duration.ToString() + "f;";
		}

		class_var.output (SB);

		//- Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - - Update - 
		CodeWriter cw_update = new CodeWriter("void Update () {", "}");
		CodeWriter cw_update_isActive = cw_update * new CodeWriter ("if(isActive){", "}");
		CodeWriter cw_update_isActive_in = cw_update_isActive * new CodeWriter ();

		if ((int)displacement != 0) {
			CodeModel cm = codeModelUpdate [(int)displacement] [displacementSelect];
			cw_update_isActive_in += cm.content;
		}
		if (triggerCondition == TriggerCondition.activeDelay) {
			cw_update_isActive_in += new CodeWriter(
				"if(!isTrigger && Time.time - trigger_activeDelayTime >= trigger_activeDelay)",
				"\tTrigger();"
			);
		}
		cw_update.output (SB);

		//- Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - - Active - 
		CodeWriter cw_Active = new CodeWriter("void Active (Vector3 info, BombGenerator.BombType requester) {", "}");
		CodeWriter cw_Active_inside = cw_Active * new CodeWriter ("isActive = true;");

		foreach (ActiveComponent AC in components) {
			if (AC.active == ActiveComponent.activeTiming.Active) {
				cw_Active_inside += AC.gameObject.name + ".SetActive (true);";
			} else if (AC.deactive == ActiveComponent.activeTiming.Active) {
				cw_Active_inside += AC.gameObject.name + ".SetActive (false);";
			}
		}
		if (damageActive == Timing.Active) {
			cw_Active_inside += "damageActive = true;";
		}
		if (triggerCondition == TriggerCondition.activeDelay) {
			cw_Active_inside += "trigger_activeDelayTime = Time.time;";
		}
		if (destroy != Timing.Active) {
			cw_Active_inside += "Destroy (transform.parent.gameObject, duration);\n";
		}
		cw_Active.output (SB);

		//- Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - - Trigger - 
		CodeWriter cw_Trigger = new CodeWriter("void Trigger () {", "}");
		CodeWriter cw_Trigger_inside = cw_Trigger * new CodeWriter ("isTrigger = true;");

		foreach (ActiveComponent AC in components) {
			if (AC.active == ActiveComponent.activeTiming.Trigger) {
				cw_Trigger_inside += AC.gameObject.name + ".SetActive (true);";
			} else if (AC.deactive == ActiveComponent.activeTiming.Trigger) {
				cw_Trigger_inside += AC.gameObject.name + ".SetActive (false);";
			}
		}
		if (damageActive == Timing.Trigger) {
			cw_Trigger_inside += "damageActive = true;";
		}
		if (destroy != Timing.Trigger) {
			cw_Trigger_inside += "Destroy (transform.parent.gameObject, duration);";
		}
		cw_Trigger.output (SB);

		//- Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - - Damage - 
		CodeWriter cw_Damage = new CodeWriter("void Damage (NPCmovement target) {", "}");
		CodeWriter cw_Damage_dActive = cw_Damage * new CodeWriter("if(damageActive){", "}");
		CodeWriter cw_Damage_dActive_in = cw_Damage_dActive * new CodeWriter ();

		if (targeting == 1 || targeting_sub == 1) {
			cw_Damage_dActive_in += "targetingAttackTime = Time.time;";
		}

		for (int i = 0; i < damageSelect.Length; i++) {
			if (damageSelect [i] != -1) {
				CodeModel cm = codeModelDamageContent [i] [damageSelect [i]];
				cw_Damage_dActive_in += cm.content;
			}
		}
		cw_Damage.output (SB);

		//- OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - - OnTriggerEnter - 
		CodeWriter cw_OTE = new CodeWriter("void OnTriggerEnter(Collider other){", "}");
		CodeWriter cw_OTE_ifminion = cw_OTE * new CodeWriter ("if(other.tag == \"minions\"){\n", "}");
		CodeWriter cw_OTE_ifminion_in = cw_OTE_ifminion * new CodeWriter();

		if (triggerCondition == TriggerCondition.inCollison) {
			cw_OTE_ifminion_in += new CodeWriter("if(!isTrigger)","\tTrigger();");
		}
		if(targeting == 0 && targeting_sub == 0){
			cw_OTE_ifminion_in += "Damage(other.GetComponent<NPCmovement> ());";
		}
		cw_OTE.output (SB);

		//- OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - - OnTriggerStay - 
		CodeWriter cw_OTS = new CodeWriter("void OnTriggerStay(Collider other){", "}");
		CodeWriter cw_OTS_ifminion = cw_OTE * new CodeWriter ("if(other.tag == \"minions\"){\n", "}");
		CodeWriter cw_OTS_ifminion_in = cw_OTS_ifminion * new CodeWriter();

		if (targeting == 0 && targeting_sub == 1) {
			cw_OTS_ifminion_in += new CodeWriter("if(Time.time - targetingAttackTime >= targetingAttackInterval)","Damage(other.GetComponent<NPCmovement> ());");
		} else if (targeting == 1) {
			cw_OTS_ifminion_in += 
				"if(InAreaTarget_target == null || other.GetComponent<NPCmovement> ().IsDead){\n" +
				"InAreaTarget_target = other.transform;\n" +
				"}";
		}
		cw_OTS.output (SB);

		//- OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - - OnTriggerExit - 
		CodeWriter cw_OTEx = new CodeWriter("void OnTriggerExit(Collider other){", "}");
		CodeWriter cw_OTEx_ifminion = cw_OTEx * new CodeWriter ("if(other.tag == \"minions\"){\n", "}");
		CodeWriter cw_OTEx_ifminion_in = cw_OTEx_ifminion * new CodeWriter();

		if (targeting == 1) {
			cw_OTEx_ifminion_in += 
				"if(InAreaTarget_target == other.transform){\n" +
				"InAreaTarget_target = null;\n" +
				"}";
		}
		cw_OTEx.output (SB);

		//- Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - - Arrived - 
		CodeWriter cw_Arrived = new CodeWriter("void Arrived(){", "}");
		CodeWriter cw_Arrived_in = cw_Arrived * new CodeWriter ();
		if (triggerCondition == TriggerCondition.arrived) {
			cw_Arrived_in += "Trigger();";
		}
		cw_Arrived.output (SB);

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
