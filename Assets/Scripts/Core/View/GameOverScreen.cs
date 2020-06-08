using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public TMPro.TextMeshProUGUI deathMessageText;
    public TMPro.TextMeshProUGUI countdownTimerText;

    [Tooltip("Additional delay before countdown timer begins.")]
    public float extraDelay = 0;
    [Tooltip("GameObject which will be toggled on GameOver.  Use to add a background to the GameOverScreen")]
    public GameObject optionalFrame;

    public void Awake()
    {
        GameManager.OnPlayerKilled += TriggerGameOver;
        SetGameOverScreenEnabled(false);
    }

    public void OnDestroy() => GameManager.OnPlayerKilled -= TriggerGameOver;


    private void TriggerGameOver(string message)
    {
        countdownTimerText.text = "";
        deathMessageText.text = message;
        StartCoroutine(DoCount());
    }

    private IEnumerator DoCount()
    {
        SetGameOverScreenEnabled(true);
        yield return new WaitForSeconds(extraDelay);
        for (int i = 3; i > 0; i--)
        {
            countdownTimerText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        SetGameOverScreenEnabled(false);
        GameManager.Instance.ResetLevel();
    }

    private void SetGameOverScreenEnabled(bool v)
    {
        countdownTimerText.enabled = v;
        deathMessageText.enabled = v;
        if (optionalFrame != null)
            optionalFrame.SetActive(v);
    }

}
