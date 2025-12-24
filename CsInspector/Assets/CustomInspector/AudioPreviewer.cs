#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

public static class AudioPreviewer
{
    private static MethodInfo playMethod;
    private static MethodInfo stopMethod;
    private static bool isPreviewClip = false; // PlayPreviewClip 함수인지 여부

    static AudioPreviewer()
    {
        // UnityEditor 어셈블리에서 AudioUtil 클래스 찾기
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

        if (audioUtilClass != null)
        {
            // 1. 최신 버전 (PlayPreviewClip) 먼저 찾기
            playMethod = audioUtilClass.GetMethod("PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null);

            if (playMethod != null)
            {
                isPreviewClip = true;
            }
            else
            {
                // 2. 구 버전 (PlayClip) 찾기 - 매개변수 3개짜리
                playMethod = audioUtilClass.GetMethod("PlayClip",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                    null);

                // 3. 만약 위도 실패하면 매개변수 1개짜리 (PlayClip) 찾기
                if (playMethod == null)
                {
                    playMethod = audioUtilClass.GetMethod("PlayClip",
                        BindingFlags.Static | BindingFlags.Public,
                        null,
                        new Type[] { typeof(AudioClip) },
                        null);
                }
            }

            // 정지 함수 찾기 (StopAllPreviewClips 또는 StopAllClips)
            stopMethod = audioUtilClass.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public);
            if (stopMethod == null)
            {
                stopMethod = audioUtilClass.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public);
            }
        }
    }

    public static void Play(AudioClip clip)
    {
        if (playMethod != null && clip != null)
        {
            try
            {
                // 매개변수 개수에 맞춰서 호출
                int paramCount = playMethod.GetParameters().Length;

                if (paramCount == 3)
                {
                    // (AudioClip clip, int startSample, bool loop)
                    playMethod.Invoke(null, new object[] { clip, 0, false });
                }
                else if (paramCount == 1)
                {
                    // (AudioClip clip)
                    playMethod.Invoke(null, new object[] { clip });
                }
            }
            catch (Exception e)
            {
                Debug.LogError("오디오 재생 실패: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("AudioUtil.Play 메서드를 찾을 수 없습니다.");
        }
    }

    public static void Stop()
    {
        if (stopMethod != null)
        {
            try
            {
                stopMethod.Invoke(null, new object[] { });
            }
            catch { }
        }
    }
}
#endif