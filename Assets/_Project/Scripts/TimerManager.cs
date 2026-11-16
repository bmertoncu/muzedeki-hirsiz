
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;
    public float timeRemaining = 120f;

    private bool gameEnded = false;

    void Start()
    {
        Time.timeScale = 1f;
        timeRemaining = 120f;
        gameEnded = false;

        timerText.text = "02:00";
    }

    void Update()
    {
        if (gameEnded) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            timerText.text = string.Format("{0:00}:{1:00}",
                minutes, seconds);
        }
        else
        {
            timeRemaining = 0;
            gameEnded = true;
            timerText.text = "00:00";

            Debug.Log("Süre Bitti!");

            Time.timeScale = 0f;
        }
    }
}
