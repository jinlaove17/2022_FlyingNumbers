using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : UI
{
    [SerializeField]
    private Text scoreText;

    private float targetScore;
    private float currentScore;

    private Coroutine scoreTextCoroutine;

    public float TargetScore
    {
        get
        {
            return targetScore;
        }
    }

    public override void Init()
    {
        targetScore = currentScore = 0;
        scoreText.text = "0";
    }

    public void IncreaseScore()
    {
        if (scoreTextCoroutine == null)
        {
            scoreTextCoroutine = StartCoroutine(IncreaseScoreForDuration(0.5f, 25));
        }
        else
        {
            StopCoroutine(scoreTextCoroutine);

            currentScore = targetScore;
            scoreText.text = ((int)currentScore).ToString();
            scoreTextCoroutine = StartCoroutine(IncreaseScoreForDuration(0.5f, 25));
        }
    }

    private IEnumerator IncreaseScoreForDuration(float duration, int increment)
    {
        targetScore = currentScore + increment;
        GameManager.Instance.ScoreLevel = (int)(targetScore / 1000 + 1);

        float offset = (targetScore - currentScore) / duration;

        while (currentScore < targetScore)
        {
            currentScore += offset * Time.deltaTime;
            scoreText.text = ((int)currentScore).ToString();

            yield return null;
        }

        currentScore = targetScore;
        scoreText.text = ((int)currentScore).ToString();
        scoreTextCoroutine = null;
    }
}
