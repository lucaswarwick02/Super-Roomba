using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class GameState : MonoBehaviour
{
    public static GameState INSTANCE;

    public GameProgress gameProgress;
    public Settings settings;

    public int initialBattery1 = 10;
    public int initialBattery2 = 10;
    public int maxDirt = 3;
    public Level level;
    private bool notPlayed = true;
    private int passScore;

    public GameStateUI gameStateUI;
    public CompletionPanel completionPanel;

    [SerializeField] public AudioSource win;
    [SerializeField] public AudioSource lose;

    [SerializeField] public AudioSource roombaMoveSound;
    [SerializeField] public AudioSource roombaDiesSound;

    [SerializeField] public AudioSource batterySound;
    [SerializeField] public AudioSource dirtSound;
    [SerializeField] public AudioSource ringSound;
    [SerializeField] public AudioSource slipSound;
    [SerializeField] public AudioSource catPushSound;
    [SerializeField] public AudioSource bumpWallSound;
    [SerializeField] public AudioSource bumpRoombaSound;
    [SerializeField] public AudioSource getPoint;
    [SerializeField] public AudioSource losePoint;

    public Vector3Int startPos1 = new Vector3Int(0, 0, 0);
    public Vector3Int startPos2 = new Vector3Int(-100, 0, 0);

    private bool gameOver = false;
    private bool batteryCheck1 = false;
    private bool batteryCheck2 = false;

    private int _battery1;
    private Vector3Int check = new Vector3Int(-100, 0, 0);
    public int Battery1
    {
        get { return _battery1; }
        set { _battery1 = value; }
    }

    private int _battery2;
    public int Battery2
    {
        get { return _battery2; }
        set { _battery2 = value; }
    }

    public int dirtCollected;



    private int _dirt;
    public int Dirt
    {
        get { return _dirt; }
        set { _dirt = value; }
    }

    private int _rings;
    public int Rings
    {
        get { return _rings; }
        set { _rings = value; }
    }

    private void Awake()
    {
        INSTANCE = this;
        Battery1 = initialBattery1;
        Battery2 = initialBattery2;
    }

    private void Start()
    {
        completionPanel.hc.enabled = false;

        String titleString = level + " Results";
        completionPanel.resultsTitle.text = "Room " + titleString.Substring(6);
        completionPanel.level = level;
        // completionPanel.medal.gameObject.SetActive(false);
        completionPanel.tick1.gameObject.SetActive(false);
        completionPanel.tick2.gameObject.SetActive(false);
        dirtCollected = 0;
        float passScore1 = (float)maxDirt / 2;
        passScore = (int)Math.Ceiling((decimal)passScore1);
        Dirt = maxDirt;
        // Change scale of camera to match settings
        completionPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameOver) checkGameStatus();

        if (!PlayerMovement.INSTANCE.isMoving())
        {
            if (!batteryCheck1)
            {
                if (_battery1 <= 0)
                {
                    batteryCheck1 = true;
                    roombaDiesSound.Play();
                }
            }
        }
        if (startPos2 != check)
        {
            if (!PlayerMovement.INSTANCE.isMoving())
            {
                if (!batteryCheck2)
                {
                    if (_battery2 <= 0)
                    {
                        batteryCheck2 = true;
                        roombaDiesSound.Play();
                    }
                }
            }
        }
    }


    private void checkGameStatus()
    {
        // Stop function if the game is still runnings
        bool isGameOver = false;

        if (Battery1 <= 0 && Battery2 <= 0)
        {
            // Both Roombas have died! (Lose)
            isGameOver = true;
        }
        if (dirtCollected >= maxDirt)
        {
            // They have collected all the dirt! (Win)
            isGameOver = true;
        }

        if (PlayerMovement.INSTANCE.isMoving())
        {
            isGameOver = false;
        }

        if (!isGameOver) return;

        float score = (dirtCollected - Rings);

        // Assign score to level save data

        completionPanel.dirtCollected.text = dirtCollected.ToString();
        completionPanel.ringsCollected.text = Rings.ToString();

        if (passScore == 1)
        {
            completionPanel.passScore.text = "get 1 point to unlock next room";
        }
        else
        {
            completionPanel.passScore.text = "get " + passScore.ToString() + " points to unlock next room";
        }
        completionPanel.medalScore.text = "get " + maxDirt.ToString() + " points to earn clean sweep medal";

        completionPanel.points.text = score.ToString();

        if (score >= (int)passScore)
        {
            if(notPlayed){
                win.Play();
                notPlayed = false;
            }
            
            completionPanel.tick1.gameObject.SetActive(true);
            if (score >= maxDirt)
            {
                gameProgress.GetHouseSaveData(level.house)[level.room - 1].hasMedal = true;
                completionPanel.medal.gameObject.SetActive(true);
                completionPanel.tick2.gameObject.SetActive(true);
            }
            else
            {
                completionPanel.medal.gameObject.SetActive(false);
                completionPanel.tick2.gameObject.SetActive(false);
            }

            if (passScore == 1)
            {
                completionPanel.passScore.text = "get 1 point to unlock next room";
            }
            else
            {
                completionPanel.passScore.text = "get " + passScore.ToString() + " points to unlock next room";
            }

            // Unlock next level
            Level nextLevel = level.NextLevel();
            gameProgress.GetHouseSaveData(nextLevel.house)[nextLevel.room - 1].isUnlocked = true;

        }

        else
        {
            if(notPlayed){
                lose.Play();
                notPlayed = false;
            }
            completionPanel.removeNextLevelButton();

            completionPanel.medal.gameObject.SetActive(false);
            completionPanel.tick1.gameObject.SetActive(false);
            completionPanel.tick2.gameObject.SetActive(false);

            completionPanel.medalScore.text = "get " + maxDirt.ToString() + " points to earn clean sweep medal";
        }

        if (level.room == 5)
        {
            completionPanel.removeNextLevelButton();
            completionPanel.passScore.text = "get " + passScore.ToString() + " points to finish this house";

            if (passScore == 1)
            {
                completionPanel.passScore.text = "get 1 point to finish this house";
            }
            if(score >= passScore){
                completionPanel.hc.enabled= true;
                completionPanel.hc.text = "House finished!";
            }
            if(level.house == 3){
                completionPanel.hc.enabled= true;
                completionPanel.hc.text = "All houses finished!!!";
            }
        }
        else {
            completionPanel.hc.enabled = false;
        }


        if (gameProgress.GetHouseSaveData(level.house)[level.room - 1].highScore < score)
        {
            if (score >= maxDirt)
            {
                gameProgress.GetHouseSaveData(level.house)[level.room - 1].highScore = (int)score;
            }
        }

        gameStateUI.gameObject.SetActive(false);
        completionPanel.gameObject.SetActive(true);
    }
}

