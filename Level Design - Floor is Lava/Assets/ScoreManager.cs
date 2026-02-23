using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager sManager;
    public int playerScore = 0;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sManager = this;
    }

    public void IncreaseScore(int Increase)
    {
        playerScore += Increase;

    }

}
