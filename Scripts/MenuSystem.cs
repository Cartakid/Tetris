﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour {
    public Text LevelText;
    public Text highScoreText;
    public Text highScoreText2;
    public Text highScoreText3;
    public Text lastScore;

    private void Start(){
        if(LevelText!=null)
            LevelText.text="0";

        if(highScoreText!=null)
            highScoreText.text = PlayerPrefs.GetInt("highscore").ToString();

        if (highScoreText2 != null)
            highScoreText2.text = PlayerPrefs.GetInt("highscore2").ToString();

        if (highScoreText3 != null) 
            highScoreText3.text = PlayerPrefs.GetInt("highscore3").ToString();

        if (lastScore != null) 
            lastScore.text = PlayerPrefs.GetInt("lastscore").ToString();
    }

    public void PlayGame (){
        if (Game.startingLevel == 0)
            Game.startingAtLevelZero = true;
        else
            Game.startingAtLevelZero = false;
        
		Application.LoadLevel("Level");
	}

    public void QuitGame(){
        Application.Quit();
    }

    public void ChangedValue(float value){
        Game.startingLevel = (int)value;
        LevelText.text = value.ToString();
    }

    public void LaunchGameMenu(){
        Application.LoadLevel("GameMenu");
    }
}