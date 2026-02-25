using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager sManager;
    public int playerScore;
    public TextMeshProUGUI scoreText;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sManager = this;
        playerScore = 0;
    }

    public void IncreaseScoreCouchCushion(int Increase)
    {
        playerScore += Increase;

    }

    public void IncreaseScoreBigCushion(int Increase)
    {
        playerScore += Increase;
    }

    public void IncreaseScoreBowl(int Increase)
    {
        playerScore += Increase;
    }

    public void IncreaseCushionWalk(int Increase)
    {
        playerScore += Increase;
    }

    public void Update()
    {
        scoreText.text = playerScore.ToString();
    }
}
