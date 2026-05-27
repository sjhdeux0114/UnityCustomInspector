using UnityEngine;

[CreateAssetMenu(fileName = "SpriteGroup", menuName = "SpriteGroup")]
public class SpriteGroup : ScriptableObject
{
    public string _Name;
    public Sprite[] Sprites;
    public float _fps;
    public AudioClip _snd;
}