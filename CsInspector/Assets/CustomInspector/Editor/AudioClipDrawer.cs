#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioClip))]
public class AudioClipDrawer : PropertyDrawer
{
    private const float Padding = 2f;
    private const float ButtonWidth = 52f;
    private const float ButtonHeight = 18f;
    private const float Gap = 4f;
    private const float TimeWidth = 48f;
    private const float VolumeLabelWidth = 24f;
    private const float MinVolumeWidth = 70f;
    private const float MinRowWidth = 220f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight;
        Rect fieldRect = new Rect(position.x, position.y, position.width, lineHeight);
        EditorGUI.PropertyField(fieldRect, property, label);

        AudioClip clip = property.objectReferenceValue as AudioClip;
        if (clip != null)
        {
            DrawPreviewControls(position, lineHeight, clip);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.objectReferenceValue != null)
        {
            height += ButtonHeight + Padding;
        }

        return height;
    }

    private static void DrawPreviewControls(Rect position, float lineHeight, AudioClip clip)
    {
        float rowX = position.x + EditorGUIUtility.labelWidth;
        float rowWidth = position.width - EditorGUIUtility.labelWidth;

        if (rowWidth < MinRowWidth)
        {
            rowX = position.x;
            rowWidth = position.width;
        }

        Rect rowRect = new Rect(rowX, position.y + lineHeight + Padding, rowWidth, ButtonHeight);
        Rect playRect = new Rect(rowRect.x, rowRect.y, ButtonWidth, ButtonHeight);
        Rect stopRect = new Rect(playRect.xMax + Gap, rowRect.y, ButtonWidth, ButtonHeight);
        Rect timeRect = new Rect(stopRect.xMax + Gap, rowRect.y, TimeWidth, ButtonHeight);
        Rect volumeLabelRect = new Rect(timeRect.xMax + Gap, rowRect.y, VolumeLabelWidth, ButtonHeight);

        float volumeX = volumeLabelRect.xMax + Gap;
        float volumeWidth = Mathf.Max(MinVolumeWidth, rowRect.xMax - volumeX);
        Rect volumeRect = new Rect(volumeX, rowRect.y, volumeWidth, ButtonHeight);

        Color oldColor = GUI.backgroundColor;

        GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);
        if (GUI.Button(playRect, "Play"))
        {
            AudioPreviewer.Play(clip, AudioPreviewer.Volume);
        }

        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
        if (GUI.Button(stopRect, "Stop"))
        {
            AudioPreviewer.Stop();
        }

        GUI.backgroundColor = oldColor;

        GUI.Label(timeRect, $"{clip.length:F1}s", EditorStyles.miniLabel);
        GUI.Label(volumeLabelRect, "Vol", EditorStyles.miniLabel);

        EditorGUI.BeginChangeCheck();
        float volume = EditorGUI.Slider(volumeRect, GUIContent.none, AudioPreviewer.Volume, 0f, 1f);
        if (EditorGUI.EndChangeCheck())
        {
            AudioPreviewer.Volume = volume;
        }
    }
}
#endif
