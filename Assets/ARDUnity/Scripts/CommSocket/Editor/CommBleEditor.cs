using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Ardunity;

[CustomEditor(typeof(CommBLE))]
public class CommBleEditor : ArdunityObjectEditor
{
    bool foldout = false;
#if (UNITY_STANDALONE_OSX && UNITY_EDITOR_OSX)
    string newName;
#endif
    
    SerializedProperty searchTimeout;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;
    SerializedProperty OnStartSearch;
    SerializedProperty OnStopSearch;

    void OnEnable()
	{
        searchTimeout = serializedObject.FindProperty("searchTimeout");
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnClose = serializedObject.FindProperty("OnClose");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnErrorClosed = serializedObject.FindProperty("OnErrorClosed");
        OnStartSearch = serializedObject.FindProperty("OnStartSearch");
        OnStopSearch = serializedObject.FindProperty("OnStopSearch");

#if (UNITY_STANDALONE_OSX && UNITY_EDITOR_OSX)
        CommBLE socket = (CommBLE)target;
        newName = socket.device.name;
#endif
    }
	
	public override void OnInspectorGUI()
	{
        this.serializedObject.Update();

#if UNITY_STANDALONE_OSX

#if UNITY_EDITOR_OSX
        CommBLE socket = (CommBLE)target;

        if(Application.isPlaying)
        {
            if(socket.isSupport)
            {
                GUILayout.Label(string.Format("Device: {0}", socket.device.name));
                if(socket.IsOpen)
                {
                    GUILayout.BeginHorizontal();
                    newName = GUILayout.TextField(newName);
                    if(GUILayout.Button("Change", GUILayout.Width(75f)))
                        socket.SetDeviceName(newName);
                    GUILayout.EndHorizontal();
                }
                else
                {
                    if(socket.isSearching)
                    {
                        EditorUtility.SetDirty(target);
                        
                        if(GUILayout.Button("Stop Search"))
                            socket.StopSearch();
                    }
                    else
                    {
                        if(GUILayout.Button("Start Search"))
                            socket.StartSearch();
                    }
                    
                    if(socket.foundDevices.Count > 0)
                    {
                        EditorGUILayout.Foldout(true, "Found Devices");
                        List<string> names = new List<string>();
                        int selection = -1;
                        for(int i=0; i<socket.foundDevices.Count; i++)
                        {
                            names.Add(socket.foundDevices[i].name);
                            if(socket.foundDevices[i].Equals(socket.device))
                                selection = i;
                        }
    
                        int newSelection = GUILayout.SelectionGrid(selection, names.ToArray(), 1);
                        if(selection != newSelection)
                            socket.device = new CommDevice(socket.foundDevices[newSelection]);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("This machine is not supported BLE", MessageType.Info);
            }
        }
#else
        EditorGUILayout.HelpBox("Inspector is not support on current OS.", MessageType.Warning);
#endif

#elif UNITY_ANDROID
        EditorGUILayout.HelpBox("Inspector is not support on current platform.", MessageType.Warning);
#else
        EditorGUILayout.HelpBox("This component is not supported on current platform.", MessageType.Warning);
#endif
      
        EditorGUILayout.PropertyField(searchTimeout, new GUIContent("searchTimeout"));

        foldout = EditorGUILayout.Foldout(foldout, "Events");
        if (foldout)
        {
            EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
            EditorGUILayout.PropertyField(OnClose, new GUIContent("OnClose"));
            EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
            EditorGUILayout.PropertyField(OnErrorClosed, new GUIContent("OnErrorClosed"));
            EditorGUILayout.PropertyField(OnStartSearch, new GUIContent("OnStartSearch"));
            EditorGUILayout.PropertyField(OnStopSearch, new GUIContent("OnStopSearch"));
        }
        
        this.serializedObject.ApplyModifiedProperties();
	}
    
    static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
    {
        string menuName = "Unity/Add CommSocket/CommBLE";
        
        if(Selection.activeGameObject != null)
            menu.AddItem(new GUIContent(menuName), false, func, typeof(CommBLE));
        else
            menu.AddDisabledItem(new GUIContent(menuName));
    }
}
