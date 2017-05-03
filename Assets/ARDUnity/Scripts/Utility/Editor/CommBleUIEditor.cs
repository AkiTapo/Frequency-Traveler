using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(CommBleUI))]
public class CommBleUIEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/CommBleUI", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/CommBleUI", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<CommBleUI>();
    }
	
	
	SerializedProperty popupCanvas;
	SerializedProperty settingCommSocket;
	SerializedProperty ok;
	SerializedProperty cancel;
	SerializedProperty commBLE;
	SerializedProperty deviceList;
	SerializedProperty deviceItem;
    SerializedProperty modify;
    SerializedProperty settingDeviceName;
    SerializedProperty inputDeviceName;
    SerializedProperty setNameOK;
    SerializedProperty setNameCancel;
	
	void OnEnable()
	{
		popupCanvas = serializedObject.FindProperty("popupCanvas");
		settingCommSocket = serializedObject.FindProperty("settingCommSocket");
		ok = serializedObject.FindProperty("ok");
		cancel = serializedObject.FindProperty("cancel");
		commBLE = serializedObject.FindProperty("commBLE");
		deviceList = serializedObject.FindProperty("deviceList");
		deviceItem = serializedObject.FindProperty("deviceItem");
        modify = serializedObject.FindProperty("modify");
        settingDeviceName = serializedObject.FindProperty("settingDeviceName");
        inputDeviceName = serializedObject.FindProperty("inputDeviceName");
        setNameOK = serializedObject.FindProperty("setNameOK");
        setNameCancel = serializedObject.FindProperty("setNameCancel");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//CommBleUI utility = (CommBleUI)target;
		
		EditorGUILayout.PropertyField(commBLE, new GUIContent("commBLE"));
		EditorGUILayout.PropertyField(popupCanvas, new GUIContent("popupCanvas"));
		EditorGUILayout.PropertyField(settingCommSocket, new GUIContent("settingCommSocket"));
		EditorGUILayout.PropertyField(deviceList, new GUIContent("deviceList"));
		EditorGUILayout.PropertyField(deviceItem, new GUIContent("deviceItem"));
		EditorGUILayout.PropertyField(ok, new GUIContent("ok"));
		EditorGUILayout.PropertyField(cancel, new GUIContent("cancel"));
        EditorGUILayout.PropertyField(modify, new GUIContent("modify"));
        EditorGUILayout.PropertyField(settingDeviceName, new GUIContent("settingDeviceName"));
        EditorGUILayout.PropertyField(inputDeviceName, new GUIContent("inputDeviceName"));
        EditorGUILayout.PropertyField(setNameOK, new GUIContent("setNameOK"));
        EditorGUILayout.PropertyField(setNameCancel, new GUIContent("setNameCancel"));

		this.serializedObject.ApplyModifiedProperties();
	}
}