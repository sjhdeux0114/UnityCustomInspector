using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UniversalInspectorExample : MonoBehaviour
{
    // =========================================================
    // 1. 레이아웃 & 디자인 (Layout & Design)
    // =========================================================
    [TabGroup("Design")]
    [Title("기본 속성", true)] // 탭 + 제목 + 구분선
    [Suffix("HP")] public float health = 100f;

    [TabGroup("Design")]
    [HorizontalGroup] // 가로 배치
    public float x;
    [TabGroup("Design")]
    [HorizontalGroup]
    public float y;

    [TabGroup("Design")]
    [BoxGroup("Movement Settings")] // 박스로 묶기
    [Suffix("m/s")] public float moveSpeed = 5f;

    [TabGroup("Design")]
    [BoxGroup("Movement Settings")]
    [Suffix("deg")] public float turnSpeed = 90f;


    // =========================================================
    // 2. 스마트 워크플로우 (Productivity)
    // =========================================================
    [TabGroup("Workflow")]
    [Title("자동화 기능")]

    [FindChild("TitleText")] // 내 자식 중 "TitleText"라는 이름의 오브젝트를 찾아 자동 연결
    public Text titleText;

    [FindChild] // 이름 안 적으면 변수명("closeButton")으로 찾음
    public Button closeButton;

    [ColorPreset("Red", 1, 0, 0, "Blue", 0, 0, 1)] // 색상 프리셋 버튼
    public Color mainColor;

    [AssetList] // 버튼 누르고 폴더 선택 시, 해당 폴더의 Sprite를 몽땅 불러옴
    public Sprite[] animationFrames;

    [FolderPath] // 탐색기 열어서 경로 선택
    public string saveDirectory;

    [OnValueChanged("OnAlphaChanged")] // 값이 바뀌면 OnAlphaChanged 함수 실행
    [Range(0, 1)]
    public float alpha = 1f;

    private void OnAlphaChanged()
    {
        Debug.Log($"Alpha changed to: {alpha}");
        // 실제로는 여기서 UI 투명도를 즉시 조절하면 됩니다.
    }


    // =========================================================
    // 3. 선택 보조 (No Strings)
    // =========================================================
    [TabGroup("Selection")]
    [Title("오타 방지 선택 도구")]

    [Tag] public string playerTag;          // 태그 목록 드롭다운
    [Layer] public int groundLayer;         // 레이어 목록 (Int 저장)
    [SortingLayer] public int sortingId;    // 소팅 레이어 (ID 저장)
    [SceneName] public string lobbyScene;   // 씬 이름 목록
    [InputAxis] public string jumpButton;   // Input Manager 축 이름

    public Animator characterAnim;
    [AnimatorParam("characterAnim")] // characterAnim의 파라미터 목록 표시
    public string runParameter;

    [ValueDropdown("GetMonsterNames")] // 함수에서 리스트 가져오기
    public string spawnMonster;

    private List<string> GetMonsterNames() // 드롭다운용 데이터 제공 함수
    {
        return new List<string>() { "Slime", "Orc", "Dragon", "Goblin" };
    }


    // =========================================================
    // 4. 데이터 검증 & 시각화 (Validation & Visualization)
    // =========================================================
    [TabGroup("Validation")]
    [Title("상태 모니터링")]

    [ProgressBar(100,1,0,0)] // 0~100 기준 진행바
    public float currentMana = 50f;
    
    [TabGroup("Validation")]
    public float MAX_HP = 1000;
    [TabGroup("Validation")]
    [ProgressBar("MAX_HP", 1, 0, 0)] // 0~100 기준 진행바
    public float currentHP = 50f;

    [MinMaxSlider(0, 20)] // 0~20 사이의 범위 지정 (Vector2.x = min, y = max)
    public Vector2 spawnTimeRange;

    [Required("이건 꼭 연결해야 합니다!")] // 비어있으면 빨간색 경고
    public GameObject corePrefab;

    [ShowIf("isAttacking")] // isAttacking이 true일 때만 보임
    public float attackDamage;

    public bool isAttacking;

    [ReadOnly] // 수정 불가, 보기만 가능
    public int frameCount;

    [Viewer(150, 150)] // 이미지/프리팹 미리보기 (크기 150x150)
    public GameObject previewModel;

    [ExtendedTooltip("상세 설명", "이 변수는 게임의 밸런스를 조절합니다.\n주의해서 다루세요.", ExtendedTooltipAttribute.MessageType.Info)]
    public float balanceFactor;


    // =========================================================
    // 5. 버튼 & 액션 (Actions)
    // =========================================================

    [InspectorButton("공격 모드 토글")]
    public void ToggleAttack()
    {
        isAttacking = !isAttacking;
        Debug.Log($"Attack Mode: {isAttacking}");
    }

    [InspectorButton("프레임 리셋", 10)] // 10만큼 위쪽 여백
    public void ResetFrames()
    {
        frameCount = 0;
        Debug.Log("Frames reset!");
    }

    private void Update()
    {
        frameCount++;
    }
}