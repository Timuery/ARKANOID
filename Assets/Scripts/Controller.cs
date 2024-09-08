using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData
{
    public int HIGHSCORE;
    public int BLOCKDESTROYED;
    public float ALLTIMEINSECONDS;
    public int MAXLEVEL;
    public int MAXCOMBO;
    public int POUNCES;

}
public class Controller : MonoBehaviour
{

    public int volumeLevel = 2;
    public List<Sprite> spritelist = new List<Sprite>();
    public Button volumeButton;

    [Space, Space, Space, Space]
    [SerializeField] private GameObject gameMenu;
    [SerializeField] private GameObject standartMenu;
    [SerializeField] private GameObject youLose;

    
    
    [HideInInspector] public bool gameIsStarted = false;
    [HideInInspector] public bool gameIsPause = false;

    [Space, Space] public GameObject Paddle; // ARKANOID (управляем)
    public Transform PaddleSpawn;

    [Space, SerializeField] private int hP;
    public List<GameObject> hPobjects = new List<GameObject>();
    public int score;
    private int countCombo;

    [Space] public int levelNum = 0;
    public int countLevels;
    private int takelevel;
    [SerializeField] private List<GameObject> level = new List<GameObject>();

    public Transform positionToSpawnLevel;


    public TextMeshProUGUI textMegaScore;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textCombo;
    public TextMeshProUGUI textlevel;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI textloseText;
    public TextMeshProUGUI textTakeLevel;
    public TextMeshProUGUI textCountLevels;
    public Scrollbar scrollbar;
    private float timer;

    [Space, Space] public GameObject statsPanel;
    public List<TextMeshProUGUI> stats = new List<TextMeshProUGUI>();
    public float Timer { get => timer; set => timer = value; }




    [HideInInspector] public PlayerData data;

    public List<Sprite> blockHP = new List<Sprite>();


    public void OpenStats()
    {
        statsPanel.SetActive(true);
        StatsUpdate();
    }
    public void HideStats()
    {
        statsPanel.SetActive(false);
    }
    public void Start()
    {
        standartMenu.SetActive(true);
        //IfNullPrefs();
        data = LoadPlayerData();
        textCountLevels.text = $"/{countLevels}";
        textTakeLevel.text = $"{0}";
        scrollbar.size = 1.0f / countLevels;
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
    }


    

    //Поик и установка шара
    public void FindPaddleAndBall(){ Destroyed(); Instantiate(Paddle, PaddleSpawn); }
    public void Destroyed()
    {
        if (GameObject.FindGameObjectsWithTag("ball").Length > 0)
        {
            foreach (var b in GameObject.FindGameObjectsWithTag("ball"))
            {
                Destroy(b);
            }
        }
        Destroy(GameObject.FindGameObjectWithTag("ARKANOID"));
    }

    public void DestroyedLevel()
    {
        GameObject[] allLevels = GameObject.FindGameObjectsWithTag("LEVEL");
        foreach (var level in allLevels)
        {
            Destroy(level);
        }
    }
    public void FullHp()
    {
        foreach (var item in hPobjects)
        {
            item.SetActive(true);
        }
        hP = hPobjects.Count;
    }
    public void NullefireData()
    {
        score = 0;
        countCombo = 0;
        ChangeCombo();
        ChangeScore();
    }
    public void SavePlayerData(PlayerData playerData)
    {
        PlayerPrefs.SetInt("HIGHSCORE", playerData.HIGHSCORE);
        PlayerPrefs.SetInt("BLOCKDESTROYED", playerData.BLOCKDESTROYED);
        PlayerPrefs.SetFloat("ALLTIMEINSECONDS", playerData.ALLTIMEINSECONDS);
        PlayerPrefs.SetInt("MAXLEVEL", playerData.MAXLEVEL);
        PlayerPrefs.SetInt("MAXCOMBO", playerData.MAXCOMBO);
        PlayerPrefs.SetInt("POUNCES", playerData.POUNCES);
        PlayerPrefs.Save();
    }
    public PlayerData LoadPlayerData()
    {
        PlayerData playerData = new PlayerData();
        
        playerData.HIGHSCORE = PlayerPrefs.GetInt("HIGHSCORE", 0);
        playerData.BLOCKDESTROYED = PlayerPrefs.GetInt("BLOCKDESTROYED", 0);
        playerData.ALLTIMEINSECONDS = PlayerPrefs.GetFloat("ALLTIMEINSECONDS", 0);
        playerData.MAXLEVEL = PlayerPrefs.GetInt("MAXLEVEL", 0);
        playerData.MAXCOMBO = PlayerPrefs.GetInt("MAXCOMBO", 0);
        playerData.POUNCES = PlayerPrefs.GetInt("POUNCES", 0);

        return playerData;
    }
    public void StatsUpdate()
    {
        stats[0].text = $"{data.HIGHSCORE:D8}";
        stats[1].text = $"{data.BLOCKDESTROYED:D8}";
        TimeSpan timespan = TimeSpan.FromSeconds(data.ALLTIMEINSECONDS);
        string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
            timespan.Hours,
            timespan.Minutes,
            timespan.Seconds,
            timespan.Milliseconds);
        stats[2].text = $"{formattedTime}";
        stats[3].text = $"{data.MAXLEVEL:D2}";
        stats[4].text = $"{data.MAXCOMBO:D3}";
        stats[5].text = $"{data.POUNCES:D8}";
    }
    /// <summary>
    /// Изменение лучшего счёта в UI
    /// </summary>
    /// 

    private void ChangeMegaScore(){if (score > data.HIGHSCORE) data.HIGHSCORE = score; StartMegaScore(); }

    /// <summary>
    /// Изначально значени MegaScore
    /// </summary>
    private void StartMegaScore() {textMegaScore.text = $"{data.HIGHSCORE:D8}"; }
        
    /// <summary>
    /// Изменение текущего счёта в UI
    /// </summary>
    private void ChangeScore() { textScore.text = $"{score:D8}"; ChangeMegaScore(); }

    /// <summary>
    /// Изменение количества комбо в UI
    /// </summary>
    private void ChangeCombo() { textCombo.text = $"{countCombo:D3}"; }
    /// <summary>
    /// Смена уровня на другой
    /// </summary>
    public void ChangeLevel()
    {

        // Удаление уровней
        if(levelNum > data.MAXLEVEL)
        {
            data.MAXLEVEL = levelNum;
        }
        if (levelNum == takelevel)
        {
            EndGame(true);
            return;
        }
        DestroyedLevel();
        StartLevel();
        FindPaddleAndBall();

        textlevel.text = $"{levelNum}/{takelevel}";
    }

    public void OnScrollbarValueChanged(float value)
    {
        // Определяем выбранный уровень, используя значение scrollbar
        takelevel = Mathf.RoundToInt(value * (countLevels - 1)) + 1;
        textTakeLevel.text = $"{takelevel}";
    }


    /// <summary>
    /// Запуск нового уровня
    /// </summary>
    private void StartLevel()
    {
        
        positionToSpawnLevel.parent = Instantiate(level[levelNum].gameObject, positionToSpawnLevel).transform;
        
        levelNum += 1;
    }

    /// <summary>
    /// Подсчёт времени + запись в textMeshPro
    /// </summary>
    private void TimeSpaning()
    {
        if (!gameIsPause)
        {
            Timer += Time.deltaTime;

            TimeSpan timespan = TimeSpan.FromSeconds(Timer);

            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
                timespan.Hours,
                timespan.Minutes,
                timespan.Seconds,
                timespan.Milliseconds);
            textTimer.text = formattedTime;
            data.ALLTIMEINSECONDS += Time.deltaTime;
        }
    }
    /// <summary>
    /// Онулирование времени
    /// </summary>
    private void StopTimer()
    {
        if (!gameIsStarted && textTimer.text != "00:00:00:000")
        {
            textTimer.text = "00:00:00:000";
            Timer = 0f;
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
    /// Заморозка времени
    /// </summary>
    public void ZeroSpeed()
    {
        Time.timeScale = 0f;
    }
    /// <summary>
    /// Своеобазное настройка времени используя <paramref name="speed"/>
    /// </summary>
    /// <param name="speed"></param>
    public void SpeedGame(float speed)
    {
        Time.timeScale = speed;
    }
    /// <summary>
    /// Возврат скорости до стандартной "100%"
    /// </summary>
    public void FullGameSpeed()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Смена здоровья используя параметр <paramref name="miplu"/>
    /// Этот параметр является значением int, он может быть отрицательным и положительным значением -1/1
    /// </summary>
    /// <param name="miplu"></param>
    public void hpcontroller(int miplu)
    {
        hP += miplu;
        if (hP > hPobjects.Count)
        {
            hP = 4;
        }
        if (hP <= 0)
        {
            //проигрышь
            EndGame(false);
        }

        foreach (var item in hPobjects)
        {
            item.SetActive(false);
        }
        for (int i=0;  i<hP; i++)
        {
            hPobjects[i].SetActive(true);
        }
    }
    /// <summary>
    /// Добавление значения к очкам используя <see cref="countCombo"/>, параметр <paramref name="CountCombo"/> является значением
    /// для подсчёта количества комбо, а знечение уже определяется по формуле получая значение <see cref="score"/>
    /// <paramref name="CountCombo"/> всегда равен <see cref="countCombo"/>
    /// </summary>
    /// <param name="CountCombo"></param>
    public void  AddScore()
    {
        score += 200 + (100 * (countCombo - 1));
        ChangeScore();
    }
    /// <summary>
    /// Добавляет значение комбо
    /// </summary>
    public void AddCombo()
    {
        countCombo += 1;
        if (countCombo > data.MAXCOMBO)
        {
            data.MAXCOMBO = countCombo;
        }
        ChangeCombo();
    }
    /// <summary>
    /// Онулирует значение комбо
    /// </summary>
    public void ZeroCombo()
    {
        countCombo = 0;
        ChangeCombo();
    }
    /// <summary>
    /// Запуск игры, действия происходящие во время запуска игры
    /// </summary>
    public void StartGame()
    {
        if (takelevel != 0)
        {
            SavePlayerData(data);
            StatsUpdate();
            FullHp();
            NullefireData();
            StartMegaScore();

            gameIsPause = true;
            gameIsStarted = false;
            StopTimer();

            gameIsStarted = true;
            gameIsPause = false;

            gameMenu.SetActive(true);
            standartMenu.SetActive(false);
            youLose.SetActive(false);
            StopTimer();
            levelNum = 0;
            ChangeLevel();
            
        }
        
    }

    public void EndGame(bool win)
    {
        youLose.SetActive(true);
        ZeroSpeed();
        Destroyed();
        DestroyedLevel();
        if (score > data.HIGHSCORE)
        {
            loseText.text = "Рекорд: " + textScore.text;
        }
        else
        {
            loseText.text = "Очки: " + textScore.text;
        }
        
        if (!win)
        {
            textloseText.text = "ВЫ ПРОИГРАЛИ";
        }
        else
        {
            textloseText.text = "ПОБЕДА!";
        }
        
        
        gameIsPause = true;
        gameIsStarted = false;
    }

    public void Exit()
    {
        Application.Quit();

    }

    public void CountBlocks()
    {
        if (GameObject.FindGameObjectsWithTag("block").Length == 1)
        {
            ChangeLevel();
        }
    }

    public void GoToMenu()
    {
        SavePlayerData(data);
        StatsUpdate();
        Destroyed();
        DestroyedLevel();
        StopTimer();
        gameIsPause = true;
        gameIsStarted = false;
        standartMenu.SetActive(true);
        youLose.SetActive(false);
        gameMenu.SetActive(false); 
    }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        TimeSpaning();
        if (gameIsPause && Time.timeScale > 0f)
        {
            ZeroSpeed();
        }
        else if (!gameIsPause && Time.timeScale == 0f)
        {
            FullGameSpeed();
        }
    }
    private void OnApplicationQuit()
    {
        SavePlayerData(data);
    }
}
