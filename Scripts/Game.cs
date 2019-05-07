using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    //ancho del marco del juego
    public static int gridWidth = 10;
    //altura del marco del juego
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingAtLevelZero; //cuando se inicia en el juego cero
    public static int startingLevel; //nivel de inicio

    public Canvas hud_canvas; //canvas del juego 
    public Canvas pause_canvas; //canvas cuando se pausa el juego 

    public int ScoreOneLine = 40; //puntaje cuando se completa 1 linea
    public int ScoreTwoLines = 100;//puntaje cuando se completan 2 linea
    public int ScoreThreeLines = 300; //puntaje cuando se completan 3 linea
    public int ScoreFourLines = 1200; //puntaje cuando se completan 4 linea

    public int currentLevel = 0;
    private int numLinesCleared = 0;

    public static float fallSpeed = 1.0f;
    public static bool isPaused = false;

    private AudioSource audioSource;
    public AudioClip clearedLineSound;
    public AudioClip music1;
    public AudioClip music2;
    public AudioClip music3;

    public Text hud_score;
    public Text hud_level;
    public Text hud_lines;

    //variables fondo
    public GameObject background1, background2, background3;


    private int numberOfRowsThisTurn = 0;
    public static int currentScore = 0;

    private GameObject previewTetromino;
    private GameObject nextTetromino;
    private GameObject savedTetromino;

    private bool gameStarted = false;
    private int startingHighScore;
    private int startingHighScore2;
    private int startingHighScore3;

    private Vector2 previewTetrominoPosition = new Vector2(-6.5f, 16); //visualizar del siguiente mino
    private Vector2 savedTetrominoPosition = new Vector2(-6.5f, 10);  //guardar mino

    public int maxSwaps = 2; //cantidad máima de cambios
    private int currentSwaps = 0; //contador de cambios

    void Start()
    {
        currentScore = 0;
        hud_score.text = "0";
        currentLevel = startingLevel;
        hud_level.text = currentLevel.ToString();
        hud_lines.text = "0";

        SpawnNextTetromino();
        startingHighScore = PlayerPrefs.GetInt("highscore");
        startingHighScore2 = PlayerPrefs.GetInt("highscore2");
        startingHighScore3 = PlayerPrefs.GetInt("highscore3");
        UpdateMusic();
    }

    //Actualizar el puntaje, nivel, velocidad de la pieza y teclas 
    void Update()
    {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateBackground();
        UpdateSpeed();
        CheckUserInput();
    }

    void CheckUserInput()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (Time.timeScale == 1)
                PauseGame();
            else
                ResumeGame();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActive");
            SaveTetromino(tempNextTetromino.transform);
        }
    }

    //Pausar el juego
    void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        hud_canvas.enabled = false;
        pause_canvas.enabled = true;
        audioSource.Pause();
    }

    //Resumir el juego
    void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        hud_canvas.enabled = true;
        pause_canvas.enabled = false;
        audioSource.Play();
    }

    //Cambiar la musica por nivel
    void UpdateMusic()
    {         if (currentLevel == 0 || currentLevel == 3 || currentLevel == 6 || currentLevel == 9)
        {             audioSource = gameObject.AddComponent<AudioSource>();             audioSource.clip = music1;             audioSource.Play();         }
        else if (currentLevel == 1 || currentLevel == 4 || currentLevel == 7)
        {             audioSource = gameObject.AddComponent<AudioSource>();             audioSource.clip = music2;             audioSource.Play();         }
        else
        {             audioSource = gameObject.AddComponent<AudioSource>();             audioSource.clip = music3;             audioSource.Play();         }
    } 
    //Cambiar el fondo por nivel
    void UpdateBackground()
    {
        if (currentLevel == 0 || currentLevel == 3 || currentLevel == 6 || currentLevel == 9)
        {
            background1.SetActive(true);
            background2.SetActive(false);
            background3.SetActive(false);
        }
        else if (currentLevel == 1 || currentLevel == 4 || currentLevel == 7)
        {
            background1.SetActive(false);
            background2.SetActive(true);
            background3.SetActive(false);
        }
        else
        {
            background1.SetActive(false);
            background2.SetActive(false);
            background3.SetActive(true);
        }
    }

    //Aumentar el nivel debido a las lineas completadas 
    void UpdateLevel()
    {
        if ((startingAtLevelZero == true) || (startingAtLevelZero == false && numLinesCleared / 10 > startingLevel))
            currentLevel = numLinesCleared / 10;
    }

    void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_lines.text = numLinesCleared.ToString();
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if (numberOfRowsThisTurn == 1)
            {
                ClearOneLine();
            }
            else if (numberOfRowsThisTurn == 2)
            {
                ClearTwoLines();
            }
            else if (numberOfRowsThisTurn == 3)
            {
                ClearThreeLines();
            }
            else if (numberOfRowsThisTurn == 4)
            {
                ClearFourLines();
            }
            numberOfRowsThisTurn = 0;
            PlayLineClearedSound();
        }
    }

    public void ClearOneLine()
    {
        currentScore += ScoreOneLine + (currentLevel * 20);
        numLinesCleared++;
    }

    public void ClearTwoLines()
    {
        currentScore += ScoreTwoLines + (currentLevel * 25);
        numLinesCleared += 2;
    }

    public void ClearThreeLines()
    {
        currentScore += ScoreThreeLines + (currentLevel * 30);
        numLinesCleared += 3;
    }

    public void ClearFourLines()
    {
        currentScore += ScoreFourLines + (currentLevel * 40);
        numLinesCleared += 4;
    }

    public void PlayLineClearedSound()
    {
        audioSource.PlayOneShot(clearedLineSound);
    }

    public void UpdateHighScore()
    {
        if (currentScore > startingHighScore)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", startingHighScore);
            PlayerPrefs.SetInt("highscore", currentScore);

        }
        else if (currentScore > startingHighScore2)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);

        }
        else if (currentScore > startingHighScore3)
        {
            PlayerPrefs.SetInt("highscore3", currentScore);
        }
        PlayerPrefs.SetInt("lastscore", currentScore);
    }

    bool CheckIsValidPosition(GameObject tetromino)
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (!CheckIsInsideGrid(pos))
                return false;

            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino.transform)
                return false;
        }
        return true;
    }

    public bool CheckIsAboveGrid(Tetromino tetromino)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);
                if (pos.y > gridHeight - 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsFullRowAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        numberOfRowsThisTurn++;
        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }
    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            if (IsFullRowAt(y))
            {
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                --y;
            }
        }
    }

    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
            nextTetromino.tag = "currentActive";
        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.tag = "currentActive";
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromino>().enabled = false;
        }
        currentSwaps = 0;
    }

    public void SaveTetromino(Transform t)
    {
        currentSwaps++;
        if (currentSwaps > maxSwaps)
            return;

        if (savedTetromino != null)
        {
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSaved");
            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            if (!CheckIsValidPosition(tempSavedTetromino))
            {
                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;
                return;
            }
            savedTetromino = (GameObject)Instantiate(t.gameObject);
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSaved";

            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetromino>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActive";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);

        }
        else
        {
            savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActive"));
            savedTetromino.GetComponent<Tetromino>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSaved";

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActive"));
            SpawnNextTetromino();
        }
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public void GameOver()
    {
        UpdateHighScore();
        Application.LoadLevel("GameOver");
    }

    string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "Prefabs/Tetromino_T";
        switch (randomTetromino)
        {

            case 1:
                randomTetrominoName = "Prefabs/Tetromino_T";
                break;
            case 2:
                randomTetrominoName = "Prefabs/Tetromino_Long";
                break;
            case 3:
                randomTetrominoName = "Prefabs/Tetromino_Square";
                break;
            case 4:
                randomTetrominoName = "Prefabs/Tetromino_J";
                break;
            case 5:
                randomTetrominoName = "Prefabs/Tetromino_L";
                break;
            case 6:
                randomTetrominoName = "Prefabs/Tetromino_S";
                break;
            case 7:
                randomTetrominoName = "Prefabs/Tetromino_Z";
                break;
        }
        return randomTetrominoName;
    }
}