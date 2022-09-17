using Firebase;
using Firebase.Auth;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UI
{
    // Firebase ������ �����ϴ� ��ü
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

        // Firebase�� ���ο� ������ ����⸦ �õ��Ѵ�.
        var registerTask = firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);

        statusText.text = "��ø� ��ٷ��ּ���...";
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
                    statusText.text = "�̸����� �ùٸ��� �ʽ��ϴ�.";
                    break;
                case AuthError.EmailAlreadyInUse:
                    statusText.text = "�̹� ��ϵ� �̸����Դϴ�.";
                    break;
                case AuthError.MissingPassword:
                case AuthError.WrongPassword:
                case AuthError.WeakPassword:
                    statusText.text = "��й�ȣ�� �ʹ� ª���ϴ�.";
                    break;
            }

            statusText.color = Color.red;
        }
        else
        {
            DataManager.Instance.RegisterUser(email);

            statusText.text = "ȸ�������� �Ϸ�Ǿ����ϴ�.";

            loginButtonGroup.SetActive(true);
            signUpButtonGroup.SetActive(false);
        }

        registerCoroutine = null;
    }

    public IEnumerator SignIn()
    {
        string email = emailInputField.text;
        string password = pwInputField.text;

        // Firebase�� �α����� �õ��Ѵ�.
        var loginTask = firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

        statusText.text = "��ø� ��ٷ��ּ���...";
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
                    statusText.text = "�̸����� �ùٸ��� �ʽ��ϴ�.";
                    break;
                case AuthError.UserNotFound:
                    statusText.text = "�������� �ʴ� �����Դϴ�.";
                    break;
                case AuthError.MissingPassword:
                case AuthError.WrongPassword:
                    statusText.text = "��й�ȣ�� �ùٸ��� �ʽ��ϴ�.";
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

    // ��ư�� Event Trigger ������Ʈ���� ���� �Լ����̴�.
    public void PlayButtonSFX()
    {
        SoundManager.Instance.PlaySFX("Button");
    }

    public void PlayPanelSFX()
    {
        SoundManager.Instance.PlaySFX("Panel");
    }
}
