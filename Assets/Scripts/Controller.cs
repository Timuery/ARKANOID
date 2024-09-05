using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{

    public int volumeLevel = 2;
    public List<Sprite> spritelist = new List<Sprite>();
    public Button volumeButton;

    [Space, Space, Space, Space]
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject standartMenu;
    [SerializeField] private GameObject youLose;
    [SerializeField] private GameObject statisticScreen;

    [HideInInspector] public bool gameIsStarted = false;
    [HideInInspector] public bool gameIsPause = false;

    [Space, Space] public GameObject Paddle; // ARKANOID (���������)
    public Transform PaddleSpawn;

    [Space, SerializeField] private int hP;
    public List<GameObject> hPobjects = new List<GameObject>();
    public int score;
    private int countCombo;

    [Space] public int levelNum=0;
    public int countLevels;
    [SerializeField] private List<GameObject> level = new List<GameObject>();

    public Transform positionToSpawnLevel;


    public TextMeshProUGUI textMegaScore;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textCombo;
    public TextMeshProUGUI textlevel;
    public TextMeshProUGUI textTimer;
    private float timer;

    public void FindPaddleAndBall()
    {
        if (GameObject.FindGameObjectWithTag("ball") != null)
        {
            GameObject balls = GameObject.FindGameObjectWithTag("ball");
            Destroy(balls);
            Debug.Log("balls Destroyed");
            Destroy(GameObject.FindGameObjectWithTag("ARKANOID"));
        }
        Destroy(GameObject.FindGameObjectWithTag("ARKANOID"));
        Debug.Log("ARKANOID NOT FIND");
        Instantiate(Paddle,PaddleSpawn);
    }
    /// <summary>
    /// ��������� ������� ����� � UI
    /// </summary>
    /// 
   
    private void ChangeMegaScore()
    {
        textMegaScore.text = $"{score:D8}"; 
    }
    /// <summary>
    /// ��������� �������� ����� � UI
    /// </summary>
    private void ChangeScore()
    {
        textScore.text = $"{score:D8}";
    }
    /// <summary>
    /// ��������� ���������� ����� � UI
    /// </summary>
    private void ChangeCombo()
    {
        textCombo.text = $"{countCombo:D3}";
    }
    /// <summary>
    /// ����� ������ �� ������
    /// </summary>
    public void ChangeLevel()
    {
        StartLevel();
        //DestroyPaddle();
        FindPaddleAndBall();
        textlevel.text = $"{levelNum}/{countLevels}";
    }


    /// <summary>
    /// ������ ������ ������
    /// </summary>
    private void StartLevel()
    {
        
        positionToSpawnLevel.parent = Instantiate(level[levelNum].gameObject, positionToSpawnLevel).transform;
        levelNum += 1;
    }

    /// <summary>
    /// ������� ������� + ������ � textMeshPro
    /// </summary>
    private void TimeSpaning()
    {
        if (gameIsStarted && !gameIsPause)
        {
            timer += Time.deltaTime;

            TimeSpan timespan = TimeSpan.FromSeconds(timer);

            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
                timespan.Hours,
                timespan.Minutes,
                timespan.Seconds,
                timespan.Milliseconds);
            textTimer.text = formattedTime;
        }
    }
    /// <summary>
    /// ������������ �������
    /// </summary>
    private void StopTimer()
    {
        if (!gameIsStarted && textTimer.text != "00:00:00:000")
        {
            textTimer.text = "00:00:00:000";
            timer = 0f;
        }
    }
    public void Volume()
    {
        volumeLevel += 1;
        if (volumeLevel > 2)
        {
            volumeLevel = 0;
        }
        volumeButton.image.sprite = spritelist[volumeLevel];
    }
    /// <summary>
    /// ��������� �������
    /// </summary>
    public void ZeroSpeed()
    {
        Time.timeScale = 0f;
    }
    /// <summary>
    /// ����������� ��������� ������� ��������� <paramref name="speed"/>
    /// </summary>
    /// <param name="speed"></param>
    public void SpeedGame(float speed)
    {
        Time.timeScale = speed;
    }
    /// <summary>
    /// ������� �������� �� ����������� "100%"
    /// </summary>
    public void FullGameSpeed()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// ����� �������� ��������� �������� <paramref name="miplu"/>
    /// ���� �������� �������� ��������� int, �� ����� ���� ������������� � ������������� ��������� -1/1
    /// </summary>
    /// <param name="miplu"></param>
    public void hpcontroller(int miplu)
    {
        hP += miplu;
        if (hP > hPobjects.Count)
        {
            hP = 2;
        }
        if (hP < 0)
        {
            ZeroSpeed();
            gameIsPause = true;
        }
    }

    /// <summary>
    /// ���������� �������� � ����� ��������� <see cref="countCombo"/>, �������� <paramref name="CountCombo"/> �������� ���������
    /// ��� �������� ���������� �����, � �������� ��� ������������ �� ������� ������� �������� <see cref="score"/>
    /// <paramref name="CountCombo"/> ������ ����� <see cref="countCombo"/>
    /// </summary>
    /// <param name="CountCombo"></param>
    public void  AddScore()
    {
        score += 200 + (100 * (countCombo - 1));
        ChangeScore();
    }
    /// <summary>
    /// ��������� �������� �����
    /// </summary>
    public void AddCombo()
    {
        countCombo += 1;
        ChangeCombo();
    }
    /// <summary>
    /// ��������� �������� �����
    /// </summary>
    public void ZeroCombo()
    {
        countCombo = 0;
        ChangeCombo();
    }
    /// <summary>
    /// ������ ����, �������� ������������ �� ����� ������� ����
    /// </summary>
    public void StartGame()
    {
        Debug.Log("START") ;
        gameMenu.SetActive(true);
        standartMenu.SetActive(false);
        youLose.SetActive(false);

        levelNum = 0;
        ChangeLevel();
    }

    public void YouLose()
    {
        youLose.SetActive(true);
        ZeroSpeed();
        gameIsPause = true;
        gameIsStarted = false;
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void CountBlocks()
    {
        Debug.Log(GameObject.FindGameObjectsWithTag("block"));
        if (GameObject.FindGameObjectsWithTag("block").Length == 1)
        {
            ChangeLevel();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       // StandartMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
        TimeSpaning();
        StopTimer();
        if (gameIsPause && Time.timeScale > 0f)
        {
            ZeroSpeed();
        }
        else if (!gameIsPause && Time.timeScale == 0f)
        {
            FullGameSpeed();
        }
    }
}
