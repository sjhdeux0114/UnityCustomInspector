#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class DefineManagerWindow : EditorWindow
{
    // 여기에 자주 사용하는 Define 목록을 적어주세요
    private readonly string[] managedDefines = new string[]
    {
        "USE_TEST_MODE",
        "OIDD_NONE",
        "SIM_TEST",
        "CASH_VER"
    };

    [MenuItem("Tools/Define Manager")]
    public static void ShowWindow()
    {
        GetWindow<DefineManagerWindow>("Define Manager");
    }

    private void OnGUI()
    {
        // --- 1. Player Settings 제어 영역 ---
        GUILayout.Label("Player Settings", EditorStyles.boldLabel);

        // 스크린샷에 있던 'Use Player Log' 체크박스 제어
        bool useLog = EditorGUILayout.Toggle("Use Player Log", PlayerSettings.usePlayerLog);
        if (useLog != PlayerSettings.usePlayerLog)
        {
            PlayerSettings.usePlayerLog = useLog;
            Debug.Log($"[DefineManager] Use Player Log set to: {useLog}");
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // 구분선
        EditorGUILayout.Space();

        // --- 2. Scripting Define Symbols 제어 영역 ---
        GUILayout.Label("Scripting Define Symbols", EditorStyles.boldLabel);

        // 현재 선택된 빌드 타겟 그룹 가져오기 (PC, Android, iOS 등)
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);

        // 현재 설정된 Define 목록 가져오기
        string definesString = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
        List<string> currentDefines = definesString.Split(';', System.StringSplitOptions.RemoveEmptyEntries).ToList();

        bool isChanged = false;

        foreach (string define in managedDefines)
        {
            bool hasDefine = currentDefines.Contains(define);
            bool toggle = EditorGUILayout.Toggle(define, hasDefine);

            if (toggle != hasDefine)
            {
                if (toggle) currentDefines.Add(define);
                else currentDefines.Remove(define);
                isChanged = true;
            }
        }

        if (isChanged)
        {
            // 변경사항 적용
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(";", currentDefines.Distinct()));
            Debug.Log("Defines updated!");
        }
    }
}
#endif