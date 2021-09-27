using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    [Header("System")]
    public BoardManager board;
    public TimeManager time;
    public ScoreManager score;

    [Header("UI")]
    public UIGameOver GameOverUI;

    #region Singleton

    private static GameFlowManager _instance = null;

    public static GameFlowManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameFlowManager>();

                if (_instance == null)
                {
                    Debug.LogError("Fatal Error: GameFlowManager not Found");
                }
            }
            return _instance;
        }
    }

    #endregion

    public bool IsGameOver{get {return isGameOver;}}
    public bool isGameOver = false;


    public void GameOver()
    {
        isGameOver = true;
        ScoreManager.Instance.SetHighScore();
        GameOverUI.Show();

    }
    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
