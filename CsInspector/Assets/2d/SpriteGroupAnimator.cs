using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum SpriteGroupPlayMode
{
    Once,
    Backward,
    Loop,
    PingPong
}

public enum SpriteGroupEndAction
{
    None,          // 마지막 프레임 유지
    Transition,    // 다른 애니메이션 상태로 전환
    HideObject,    // 오브젝트 비활성화
    DestroyObject  // 오브젝트 파괴
}

[System.Serializable]
public class SpriteGroupEvent
{
    [Tooltip("이벤트를 발생시킬 프레임 번호 (0부터 시작)")]
    public int frameIndex;

    [Tooltip("발생시킬 Unity 이벤트")]
    public UnityEvent onTriggerEvent;
}

[System.Serializable]
public class SpriteGroupState : InspectorBase
{
    [ReadOnly]
    [Tooltip("상태 식별용 이름 (SpriteGroup 에셋의 _Name과 자동 동기화됩니다)")]
    public string stateName;

    [Required("SpriteGroup 에셋을 지정해야 합니다.")]
    public SpriteGroup spriteGroup;

    public SpriteGroupPlayMode playMode = SpriteGroupPlayMode.Once;
    public SpriteGroupEndAction endAction = SpriteGroupEndAction.None;

    [HideInInspector] public bool isTransitionAction;

    [ShowIf("isTransitionAction")]
    [ValueDropdown("GetAnimationNames")]
    [Tooltip("전환할 다음 애니메이션 상태의 이름")]
    public string nextStateName;

    [ShowIf("isTransitionAction")]
    [Tooltip("전환되기 전에 대기할 지연 시간 (초)")]
    public float delayBeforeTransition = 0f;

    [Tooltip("특정 프레임에서 실행할 이벤트 목록")]
    public List<SpriteGroupEvent> frameEvents = new List<SpriteGroupEvent>();
}

public class SpriteGroupAnimator : MonoBehaviour
{
    [Header("Renderers")]
    public SpriteRenderer spriteRenderer;
    public Image uiImage;

    [Header("Animations")]
    public List<SpriteGroupState> animations = new List<SpriteGroupState>();
    public string defaultAnimation;
    public bool playOnAwake = true;
    public bool bUnscaled = false;

    [Header("Runtime State (Debug)")]
    [ReadOnly] public string currentStateName;
    [ReadOnly] public int currentFrame;
    [ReadOnly] public bool isPlaying;

    private SpriteGroupState currentState;
    private float frameTimer;
    private bool isPingPongForward = true;
    private Coroutine transitionCoroutine;
    private HashSet<SpriteGroupEvent> triggeredEvents = new HashSet<SpriteGroupEvent>();

    private void Awake()
    {
        // 렌더러 자동 할당
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (uiImage == null) uiImage = GetComponent<Image>();
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        if (playOnAwake && !string.IsNullOrEmpty(defaultAnimation))
        {
            Play(defaultAnimation);
        }

    }

    private void Update()
    {
        if (!isPlaying || currentState == null || currentState.spriteGroup == null) return;

        Sprite[] sprites = currentState.spriteGroup.Sprites;
        if (sprites == null || sprites.Length == 0) return;

        float dt = bUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
        frameTimer += dt;

        float fps = currentState.spriteGroup._fps > 0 ? currentState.spriteGroup._fps : 30f;
        float frameDuration = 1f / fps;

        if (frameTimer >= frameDuration)
        {
            int framesToAdvance = Mathf.FloorToInt(frameTimer / frameDuration);
            frameTimer %= frameDuration;

            AdvanceFrames(framesToAdvance, sprites.Length);
        }
    }

    /// <summary>
    /// 지정된 이름의 애니메이션 상태를 재생합니다.
    /// </summary>
    public void Play(string animName)
    {
        SpriteGroupState targetState = FindState(animName);
        if (targetState == null)
        {
            Debug.LogWarning($"[SpriteGroupAnimator] '{animName}' 상태를 찾을 수 없습니다.");
            return;
        }

        // 진행 중인 지연 전환이 있으면 중단
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        currentState = targetState;
        currentStateName = string.IsNullOrEmpty(currentState.stateName) ? currentState.spriteGroup._Name : currentState.stateName;
        frameTimer = 0f;
        isPlaying = true;
        triggeredEvents.Clear();

        // 플레이 모드에 따른 시작 프레임 결정
        if (currentState.playMode == SpriteGroupPlayMode.Backward)
        {
            currentFrame = currentState.spriteGroup.Sprites.Length - 1;
        }
        else
        {
            currentFrame = 0;
        }

        isPingPongForward = true;

        // 프레임 즉시 적용
        ApplyFrame();

        // 사운드 재생
        if (currentState.spriteGroup._snd != null)
        {
            // 루프 모드인 경우 사운드 루프 설정 적용
            bool isLoopSound = (currentState.playMode == SpriteGroupPlayMode.Loop);
            {
                //play sound  : currentState.spriteGroup._snd
            //    SoundEventBus.PlayEffect(currentState.spriteGroup._snd, isLoopSound);
            }
        }
    }

    public void Stop()
    {
        isPlaying = false;
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Resume()
    {
        if (currentState != null)
        {
            isPlaying = true;
        }
    }

    private SpriteGroupState FindState(string name)
    {
        foreach (var state in animations)
        {
            if (state == null || state.spriteGroup == null) continue;

            string keyName = string.IsNullOrEmpty(state.stateName) ? state.spriteGroup._Name : state.stateName;
            if (keyName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                return state;
            }
        }
        return null;
    }

    private void AdvanceFrames(int count, int totalFrames)
    {
        for (int i = 0; i < count; i++)
        {
            switch (currentState.playMode)
            {
                case SpriteGroupPlayMode.Once:
                    if (currentFrame < totalFrames - 1)
                    {
                        currentFrame++;
                        CheckFrameEvents();
                    }
                    else
                    {
                        OnAnimationEnd();
                        return;
                    }
                    break;

                case SpriteGroupPlayMode.Backward:
                    if (currentFrame > 0)
                    {
                        currentFrame--;
                        CheckFrameEvents();
                    }
                    else
                    {
                        OnAnimationEnd();
                        return;
                    }
                    break;

                case SpriteGroupPlayMode.Loop:
                    currentFrame = (currentFrame + 1) % totalFrames;
                    CheckFrameEvents();
                    break;

                case SpriteGroupPlayMode.PingPong:
                    if (isPingPongForward)
                    {
                        if (currentFrame < totalFrames - 1)
                        {
                            currentFrame++;
                        }
                        else
                        {
                            isPingPongForward = false;
                            currentFrame = Mathf.Max(0, totalFrames - 2);
                        }
                    }
                    else
                    {
                        if (currentFrame > 0)
                        {
                            currentFrame--;
                        }
                        else
                        {
                            isPingPongForward = true;
                            currentFrame = Mathf.Min(totalFrames - 1, 1);
                        }
                    }
                    CheckFrameEvents();
                    break;
            }
        }

        ApplyFrame();
    }

    private void ApplyFrame()
    {
        if (currentState == null || currentState.spriteGroup == null) return;
        Sprite[] sprites = currentState.spriteGroup.Sprites;
        if (sprites == null || currentFrame < 0 || currentFrame >= sprites.Length) return;

        Sprite sprite = sprites[currentFrame];
        if (spriteRenderer != null) spriteRenderer.sprite = sprite;
        if (uiImage != null) uiImage.sprite = sprite;

        CheckFrameEvents();
    }

    private void CheckFrameEvents()
    {
        if (currentState == null || currentState.frameEvents == null) return;

        foreach (var evt in currentState.frameEvents)
        {
            if (evt == null) continue;

            if (currentFrame == evt.frameIndex)
            {
                if (!triggeredEvents.Contains(evt))
                {
                    triggeredEvents.Add(evt);
                    evt.onTriggerEvent?.Invoke();
                }
            }
            else
            {
                triggeredEvents.Remove(evt);
            }
        }
    }

    private void OnAnimationEnd()
    {
        isPlaying = false;

        switch (currentState.endAction)
        {
            case SpriteGroupEndAction.None:
                break;

            case SpriteGroupEndAction.Transition:
                if (!string.IsNullOrEmpty(currentState.nextStateName))
                {
                    if (currentState.delayBeforeTransition > 0f)
                    {
                        transitionCoroutine = StartCoroutine(DelayedTransition(currentState.nextStateName, currentState.delayBeforeTransition));
                    }
                    else
                    {
                        Play(currentState.nextStateName);
                    }
                }
                break;

            case SpriteGroupEndAction.HideObject:
                gameObject.SetActive(false);
                break;

            case SpriteGroupEndAction.DestroyObject:
                Destroy(gameObject);
                break;
        }
    }

    private IEnumerator DelayedTransition(string nextState, float delay)
    {
        yield return new WaitForSeconds(delay);
        transitionCoroutine = null;
        Play(nextState);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (animations != null)
        {
            foreach (var anim in animations)
            {
                if (anim != null)
                {
                    anim.isTransitionAction = (anim.endAction == SpriteGroupEndAction.Transition);
                    if (anim.spriteGroup != null)
                    {
                        if (anim.stateName != anim.spriteGroup._Name)
                        {
                            anim.stateName = anim.spriteGroup._Name;
                        }

                        if (anim.frameEvents != null)
                        {
                            int maxFrame = anim.spriteGroup.Sprites != null ? anim.spriteGroup.Sprites.Length - 1 : 0;
                            foreach (var evt in anim.frameEvents)
                            {
                                if (evt != null)
                                {
                                    evt.frameIndex = Mathf.Clamp(evt.frameIndex, 0, Mathf.Max(0, maxFrame));
                                }
                            }
                        }
                    }
                    else
                    {
                        anim.stateName = "";
                    }
                }
            }
        }
    }

    /// <summary>
    /// 인스펙터의 ValueDropdown에서 사용할 상태 이름 목록을 반환합니다.
    /// </summary>
    public List<string> GetAnimationNames()
    {
        List<string> list = new List<string>();
        foreach (var anim in animations)
        {
            if (anim == null || anim.spriteGroup == null) continue;
            string name = string.IsNullOrEmpty(anim.stateName) ? anim.spriteGroup._Name : anim.stateName;
            if (!string.IsNullOrEmpty(name) && !list.Contains(name))
            {
                list.Add(name);
            }
        }
        return list;
    }
#endif
}
