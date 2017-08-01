using System.Collections;
using System.Collections.Generic;
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
	private Timing damageActive = Timing.None;
	private Timing AutoTarget = Timing.None;
	private TriggerCondition triggerCondition = TriggerCondition.noTrigger;

	private SerializedObject m_seObject;
	private SerializedProperty m_seProperty;

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
	}
	void OnGUI(){
		
		GUILayout.Label ("Basic Information : ");
		fiTitle = EditorGUILayout.TextField ("ItemTitle", fiTitle);

		EditorGUILayout.PropertyField (m_seProperty, true);

		displacement = (Displacement)EditorGUILayout.EnumPopup ("Displacement : ", displacement);

		damageActive = (Timing)EditorGUILayout.EnumPopup ("Damage Active : ", damageActive);

		triggerCondition = (TriggerCondition)EditorGUILayout.EnumPopup ("Trigger at : ", triggerCondition);








	}
}
