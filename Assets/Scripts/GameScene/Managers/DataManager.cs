using UnityEngine;
using Firebase.Database;
using System;

[Serializable]
public class UserData
{
    public int easyHighScore;
    public int normalHighScore;
    public int hardHighScore;
}

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    // Firebase�� �����͸� ����/�ҷ����⸦ �ϱ� ���� ��ü
    private DatabaseReference databaseReference;

    private string userEmail;
    private UserData userData;

    public static DataManager Instance
    {
        get
        {
            return instance;
        }
    }

    public UserData UserData
    {
        get
        {
            return userData;
        }

        set
        {
            userData = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            userData = new UserData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterUser(string email)
    {
        string jsonData = JsonUtility.ToJson(userData);

        // Ű ���� .�� ���� �ȵǴ� �� ����
        userEmail = email.Replace(".", "");
        databaseReference.Child("Users").Child(userEmail).SetRawJsonValueAsync(jsonData);
    }

    public void LoadUserData(string email)
    {
        // Ű ���� .�� ���� �ȵǴ� �� ����
        userEmail = email.Replace(".", "");

        databaseReference.Child("Users").Child(userEmail).GetValueAsync().ContinueWith(
            (task) =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot dataSnapshot = task.Result;

                    // ���� Ÿ��Ʋ �������� UIManager�� ���� ������ ������ ���� ĳ���س��´�.
                    userData = JsonUtility.FromJson<UserData>(dataSnapshot.GetRawJsonValue());
                }
            });
    }

    public void ApplyUserData()
    {
        UIManager.Instance.LobbyUIs.HighScoreTexts[0].text = userData.easyHighScore.ToString();
        UIManager.Instance.LobbyUIs.HighScoreTexts[1].text = userData.normalHighScore.ToString();
        UIManager.Instance.LobbyUIs.HighScoreTexts[2].text = userData.hardHighScore.ToString();
    }

    public void SaveUserData()
    {
        string jsonData = JsonUtility.ToJson(userData);

        databaseReference.Child("Users").Child(userEmail).SetRawJsonValueAsync(jsonData);
    }
}
