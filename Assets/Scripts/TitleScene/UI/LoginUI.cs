using Firebase;
using Firebase.Auth;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UI
{
    // Firebase 인증을 관리하는 객체
    private FirebaseAuth firebaseAuth;

    private bool isTouchable;

    [SerializeField]
    private Text panelTitleText;

    [SerializeField]
    private Text statusText;

    [SerializeField]
    private InputField emailInputField;

    [SerializeField]
    private InputField pwInputField;

    [SerializeField]
    private GameObject loginButtonGroup;

    [SerializeField]
    private GameObject signUpButtonGroup;

    private Coroutine registerCoroutine;
    private Coroutine loginCoroutine;

    protected override void Awake()
    {
        base.Awake();

        firebaseAuth = FirebaseAuth.DefaultInstance;
    }

    private void Start()
    {
        Invoke("Show", 2.0f);
    }

    private void Update()
    {
        if (isTouchable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneController.Instance.MoveToScene("GameScene");
                SoundManager.Instance.PlaySFX("GameStart");

                isTouchable = false;
            }
        }
    }

    public void OnClickLoginButton()
    {
        if (loginCoroutine == null)
        {
            loginCoroutine = StartCoroutine(SignIn());
        }
    }

    public void OnClickToSignUpButton()
    {
        statusText.text = "";

        loginButtonGroup.SetActive(false);
        signUpButtonGroup.SetActive(true);
    }

    public void OnClickSignUpButton()
    {
        if (registerCoroutine == null)
        {
            registerCoroutine = StartCoroutine(SignUp());
        }
    }

    public IEnumerator SignUp()
    {
        string email = emailInputField.text;
        string password = pwInputField.text;

        // Firebase에 새로운 계정을 만들기를 시도한다.
        var registerTask = firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);

        statusText.text = "잠시만 기다려주세요...";
        statusText.color = Color.green;

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            switch (authError)
            {
                case AuthError.MissingEmail:
                case AuthError.InvalidEmail:
                    statusText.text = "이메일이 올바르지 않습니다.";
                    break;
                case AuthError.EmailAlreadyInUse:
                    statusText.text = "이미 등록된 이메일입니다.";
                    break;
                case AuthError.MissingPassword:
                case AuthError.WrongPassword:
                case AuthError.WeakPassword:
                    statusText.text = "비밀번호가 너무 짧습니다.";
                    break;
            }

            statusText.color = Color.red;
        }
        else
        {
            DataManager.Instance.RegisterUser(email);

            statusText.text = "회원가입이 완료되었습니다.";

            loginButtonGroup.SetActive(true);
            signUpButtonGroup.SetActive(false);
        }

        registerCoroutine = null;
    }

    public IEnumerator SignIn()
    {
        string email = emailInputField.text;
        string password = pwInputField.text;

        // Firebase에 로그인을 시도한다.
        var loginTask = firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

        statusText.text = "잠시만 기다려주세요...";
        statusText.color = Color.green;

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            switch (authError)
            {
                case AuthError.MissingEmail:
                case AuthError.InvalidEmail:
                    statusText.text = "이메일이 올바르지 않습니다.";
                    break;
                case AuthError.UserNotFound:
                    statusText.text = "존재하지 않는 계정입니다.";
                    break;
                case AuthError.MissingPassword:
                case AuthError.WrongPassword:
                    statusText.text = "비밀번호가 올바르지 않습니다.";
                    break;
            }

            statusText.color = Color.red;
        }
        else
        {
            DataManager.Instance.LoadUserData(email);
            Hide();

            isTouchable = true;
        }

        loginCoroutine = null;
    }

    private void Show()
    {
        animator.Play("Show", -1, 0.0f);
    }

    private void Hide()
    {
        animator.Play("Hide", -1, 0.0f);
    }

    // 버튼의 Event Trigger 컴포넌트에서 사용될 함수들이다.
    public void PlayButtonSFX()
    {
        SoundManager.Instance.PlaySFX("Button");
    }

    public void PlayPanelSFX()
    {
        SoundManager.Instance.PlaySFX("Panel");
    }
}
