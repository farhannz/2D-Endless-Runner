using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private int currentScore = 0;
    
    [Header("Score Highlight")]
    public int scoreHighlightRange;
    public CharacterSoundController sound;
    private int lastScoreHighlight = 0;
    [Header("Player")]
    public CharacterMoveController player;
    // Start is called before the first frame update
    void Start()
    {
        currentScore = 0;
        lastScoreHighlight = 0;
    }

    public float GetCurrentScore(){
        return currentScore;
    }

    public void IncreaseScore(int increase){
        currentScore += increase;
        if (currentScore - lastScoreHighlight > scoreHighlightRange)
        {
            sound.PlayScoreHighlight();
            lastScoreHighlight += scoreHighlightRange;
            // CharacterMoveController.
            player.IncreaseMaxSpeed();
        }
    }
    public void FinishScoring(){
        if(currentScore > ScoreData.highscore){
            ScoreData.highscore = currentScore;
        }
    }
}
