using UnityEngine;
using UnityEngine.UI;

public class LoadingCavnasController : MonoBehaviour
{
    // UI 컴포넌트
    [SerializeField] private Animator animator;     // 로딩 애니메이션
    [SerializeField] private Button cancelBtn;      // 취소 버튼

    private NetworkRunnerController networkRunnerController;

    // 초기화
    private void Start()
    {
        // 네트워크 러너 컨트롤러 참조 및 이벤트 리스너 등록
        networkRunnerController = GlobalManagers.Instance.NetworkRunnerController;
        networkRunnerController.OnStartedRunnerConnection += OnStartedRunnerConnection;
        networkRunnerController.OnPlayerJoinedSuccessfully += OnPlayerJoinedSuccessfully;

        // 취소 버튼 이벤트 설정
        cancelBtn.onClick.AddListener(networkRunnerController.ShutDownRunner);
        this.gameObject.SetActive(false);
    }

    // 러너 연결 시작 시 호출
    private void OnStartedRunnerConnection()
    {
        this.gameObject.SetActive(true);
        const string CLIP_NAME = "In";
        // 페이드 인 애니메이션 재생
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(gameObject, animator, CLIP_NAME));
    }

    // 플레이어 접속 성공 시 호출
    private void OnPlayerJoinedSuccessfully()
    {
        const string CLIP_NAME = "Out";
        // 페이드 아웃 애니메이션 재생 후 비활성화
        StartCoroutine(Utils.PlayAnimAndSetStateWhenFinished(gameObject, animator, CLIP_NAME, false));
    }

    // 컴포넌트 제거 시 이벤트 리스너 해제
    private void OnDestroy()
    {
        networkRunnerController.OnStartedRunnerConnection -= OnStartedRunnerConnection;
        networkRunnerController.OnPlayerJoinedSuccessfully -= OnPlayerJoinedSuccessfully;
    }
}