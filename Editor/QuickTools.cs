using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class QuickTools : EditorWindow
{
    // Section Foldouts
    bool showTransforms = true;
    bool showAdvanced = true;
    bool showMaterials = true; // New
    bool showView = true;

    // Shader Selection State
    Shader targetShader;

    [MenuItem("Window/My Quick Tools")]
    public static void ShowWindow()
    {
        GetWindow<QuickTools>("Quick Tools");
    }

    void OnGUI()
    {
        // --- Object Transforms ---
        showTransforms = EditorGUILayout.BeginFoldoutHeaderGroup(showTransforms, "Object Transforms");
        if (showTransforms)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Zero (0,0,0)")) ResetPosition();
            if (GUILayout.Button("Align Parent")) AlignWithParent();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Straighten Vertical")) StraightenVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // --- NEW: Material & Shaders ---
        showMaterials = EditorGUILayout.BeginFoldoutHeaderGroup(showMaterials, "Material & Shaders");
        if (showMaterials)
        {
            targetShader = (Shader)EditorGUILayout.ObjectField("Target Shader", targetShader, typeof(Shader), false);
            
            if (GUILayout.Button("Apply Shader to Selected"))
            {
                ApplyShaderToSelected();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // --- Advanced Alignment ---
        showAdvanced = EditorGUILayout.BeginFoldoutHeaderGroup(showAdvanced, "Advanced Alignment");
        if (showAdvanced)
        {
            if (GUILayout.Button("Match Rotation to Active")) MatchRotation();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // --- View Tools ---
        showView = EditorGUILayout.BeginFoldoutHeaderGroup(showView, "View Tools");
        if (showView)
        {
            if (GUILayout.Button("Move to View (Alt+Shift+F)")) AlignWithView();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    // --- Shader Logic ---
    private void ApplyShaderToSelected()
    {
        if (targetShader == null)
        {
            Debug.LogError("No shader selected in Quick Tools!");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Record undo for each material modified
                Undo.RecordObjects(renderer.sharedMaterials, "Change Shader");
                foreach (Material mat in renderer.sharedMaterials)
                {
                    if (mat != null) mat.shader = targetShader;
                }
            }
        }
    }

    // --- Transform Methods ---
    private void ResetPosition()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Zero Position");
            obj.transform.localPosition = Vector3.zero;
        }
    }

    private void AlignWithParent()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.transform.parent != null)
            {
                Undo.RecordObject(obj.transform, "Align with Parent");
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void StraightenVertical()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Straighten Vertical");
            float currentY = obj.transform.localEulerAngles.y;
            obj.transform.localEulerAngles = new Vector3(0, currentY, 0);
        }
    }

    private void MatchRotation()
    {
        GameObject target = Selection.activeGameObject;
        if (target == null) return;
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj == target) continue;
            Undo.RecordObject(obj.transform, "Match Rotation");
            obj.transform.rotation = target.transform.rotation;
        }
    }

    private void AlignWithView()
    {
        EditorApplication.ExecuteMenuItem("GameObject/Align With View");
    }
}
