﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Interaction.Minigame;
using Interaction;
using System;
using GameManager;

public class CryptoGramManager : MonoBehaviour
{
    //--------------------------------------------FOR QUEST------------------------------------------------//

    public static CryptoGramManager instance;

    [HideInInspector] public Dictionary<string, CryptoQuest> cryptoQuests = new Dictionary<string, CryptoQuest>();
    public string currentCollectQuestId;
    GameObject container;
    GameObject targetNPC;
    CryptoQuest __cryptoquest;
    CryptogramEventSO cryptoEvent;
    private Coroutine CryptoTimerRoutine;
    private float timeRemaining;
    private const float COLLECT_DURATION = 20f;

    private void Awake()
    {
        instance = this;
    }

    public void InitCryptoQuest(GameObject targetNPC, CryptogramEventSO cryptoEvent)
    {
        timeRemaining = COLLECT_DURATION;
        this.targetNPC = targetNPC;
        this.cryptoEvent = cryptoEvent;
        currentCollectQuestId = cryptoEvent.questId;

        //setup collect quest
        CryptoQuest cryptoQuest = new CryptoQuest();
        __cryptoquest = cryptoQuest;

        cryptoQuest.OnFinishQuest = () => {
            targetNPC.SendMessage("FinishQuestStep");
            ConservationManager.instance.StarContainer.Deactivate();
            if (CryptoTimerRoutine != null)
            {
                StopCoroutine(CryptoTimerRoutine);
                CryptoTimerRoutine = null;
            }

            // ẩn UI timer
            ConservationManager.instance.timerContainer.Deactivate();
           QuestManager.instance.questMap[cryptoEvent.questId].OnQuestFinish += OnMainQuestComplete;
        
        };
        cryptoQuests.Add(cryptoEvent.minigameId, cryptoQuest);
        if (ConservationManager.instance != null)
        {
            StartCoroutine(ConservationManager.instance.DeactivateConservationDialog());
        }
        Init();
        Invoke(nameof(StartCryptoTimer), 0.5f);


    }

    private void StartCryptoTimer()
    {
        // Tránh gọi nhiều lần
        if (CryptoTimerRoutine != null)
        {
            StopCoroutine(CryptoTimerRoutine);
        }

        CryptoTimerRoutine = StartCoroutine(CollectCountdown(timeRemaining));
    }

    private IEnumerator CollectCountdown(float duration)
    {
        float t = duration;
        ConservationManager.instance.timerContainer.Activate();


        while (t > 0f)
        {
            // tính phút và giây
            int minutes = (int)(t / 60);
            int seconds = (int)(t % 60);
            // format “MM:SS”
            ConservationManager.instance.timerText.text = $"{minutes:00}:{seconds:00}";

            t -= Time.deltaTime;
            timeRemaining = t;
            yield return null;
        }

        // khi hết giờ
        ConservationManager.instance.timerText.text = "00:00";
        OnCollectTimerExpired();
    }

    private void OnCollectTimerExpired()
    {
        // dừng coroutine nếu còn chạy
        if (CryptoTimerRoutine != null)
        {
            StopCoroutine(CryptoTimerRoutine);
            CryptoTimerRoutine = null;
        }

        // ẩn UI timer
        ConservationManager.instance.timerContainer.Deactivate();
        Game.Deactivate();
        cryptoQuests.Remove(cryptoEvent.minigameId);
        // reset quest về CAN_START
        GameManager.QuestManager.instance.UpdateQuestStep(
         QuestState.CAN_START,
          currentCollectQuestId
      );

        targetNPC.SendMessage("ChangeNPCState", NPCState.HAVE_QUEST);

        DialogConservation correctDialog = new DialogConservation();
        DialogResponse response = new DialogResponse();

        correctDialog.message = "Thời gian đã hết. Bạn đã không thể hoàn thành nhiệm vụ. Hãy thử lại vào lần tới";
        response.executedFunction = DialogExecuteFunction.OnQuestMinigameFail;

        response.message = "Đã hiểu";
        correctDialog.possibleResponses.Add(response);
        TalkInteraction.instance.StartCoroutine(TalkInteraction.instance.SmoothTransitionToTraceMiniGame());
        StartCoroutine(ConservationManager.instance.UpdateConservation(correctDialog));

        targetNPC.SendMessage("OnQuizTimerFail");
    }

    public void OnMainQuestComplete()
    {
       
    }

    public void Win(string minigameId)
    {
        CryptoQuest quest = cryptoQuests[minigameId];
        quest.Win();
   
    }

    public void Lose()
    {
        // dừng coroutine nếu còn chạy
        if (CryptoTimerRoutine != null)
        {
            StopCoroutine(CryptoTimerRoutine);
            CryptoTimerRoutine = null;
        }

        // ẩn UI timer
        ConservationManager.instance.timerContainer.Deactivate();
        Game.Deactivate();
        cryptoQuests.Remove(cryptoEvent.minigameId);
        // reset quest về CAN_START
        GameManager.QuestManager.instance.UpdateQuestStep(
         QuestState.CAN_START,
          currentCollectQuestId
      );

        targetNPC.SendMessage("ChangeNPCState", NPCState.HAVE_QUEST);

        DialogConservation correctDialog = new DialogConservation();
        DialogResponse response = new DialogResponse();

        correctDialog.message = "Bạn đã sai quá 3 lần. Hãy thử lại vào lần tới";
        response.executedFunction = DialogExecuteFunction.OnQuestMinigameFail;

        response.message = "Đã hiểu";
        correctDialog.possibleResponses.Add(response);
        TalkInteraction.instance.StartCoroutine(TalkInteraction.instance.SmoothTransitionToTraceMiniGame());
        StartCoroutine(ConservationManager.instance.UpdateConservation(correctDialog));

        targetNPC.SendMessage("OnQuizTimerFail");
    }

    //--------------------------------------------FOR MINIGAME------------------------------------------------//
    [System.Serializable]
    public class HiddenLetterConfig
    {
        public char letter;
        public int assignedNumber; // Số được gán cho chữ cái này
        public List<int> hideAtPositions; // Vị trí sẽ ẩn chữ cái (0-based index)
    }

    [System.Serializable]
    public class CryptoLevel
    {
        public string quote;
        public List<HiddenLetterConfig> hiddenLetters; // Cấu hình cho các chữ cái ẩn
    }

    [Header("UI References")]
    [SerializeField] private Transform quoteContainer;
    [SerializeField] private GameObject letterSlotPrefab; // Prefab cho mỗi ô chữ
    [SerializeField] private Transform keyboardContainer;
    [SerializeField] private GameObject keyButtonPrefab; // Prefab cho phím keyboard
    [SerializeField] private TextMeshProUGUI mistakesText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private UIAnimationController Game;
    [SerializeField] private UIAnimationController winPanel;
    [SerializeField] private UIAnimationController losePanel;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button retryButton;

    [Header("Game Settings")]
    [SerializeField] private List<CryptoLevel> levels = new List<CryptoLevel>();
    [SerializeField] private int maxMistakes = 3;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private Color completedKeyColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color activeKeyColor = Color.green;
    [SerializeField] private Color defaultKeyColor = Color.white;

    // Game State
    private int currentLevel = 0;
    private int mistakes = 0;
    private Dictionary<char, int> letterToNumber = new Dictionary<char, int>();
    private Dictionary<int, char> numberToLetter = new Dictionary<int, char>();
    private Dictionary<char, bool> letterCompleted = new Dictionary<char, bool>();
    private Dictionary<char, List<int>> letterPositions = new Dictionary<char, List<int>>(); // Track all positions of each letter
    private Dictionary<char, List<int>> hiddenPositions = new Dictionary<char, List<int>>(); // Track hidden positions for each letter
    private List<LetterSlot> letterSlots = new List<LetterSlot>();
    private Dictionary<char, KeyButton> keyButtons = new Dictionary<char, KeyButton>();
    private LetterSlot selectedSlot = null;
    private bool gameActive = false;
    private List<CryptoLevel> selectedLevels = new List<CryptoLevel>(); // Levels selected for this game session
    private const int MAX_LEVELS_PER_GAME = 1; // Maximum levels per game

    // Classes for UI elements
    private class LetterSlot
    {
        public GameObject gameObject;
        public TextMeshProUGUI letterText;
        public TextMeshProUGUI numberText;
        public Button button;
        public Image background;
        public char correctLetter;
        public int assignedNumber;
        public bool isHidden;
        public bool isFilled;
        public int positionIndex; // Position in the quote
        public RectTransform rectTransform;
    }

    private class KeyButton
    {
        public GameObject gameObject;
        public Button button;
        public TextMeshProUGUI text;
        public Image background;
        public char letter;
    }

    void Start()
    {
        SetupDefaultLevels();
        SetupUI();
    }

    void SetupDefaultLevels()
    {
        if (levels.Count == 0)
        {
            // Level 1: WHERE THERE IS LOVE THERE IS LIFE
            levels.Add(new CryptoLevel
            {
                quote = "WHERE THERE IS LOVE THERE IS LIFE",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig
                    {
                        letter = 'E',
                        assignedNumber = 19,
                        hideAtPositions = new List<int> { 0, 2, 3 } // Hide E at positions 0, 2, 3 (will show E at position 1)
                    },
                    new HiddenLetterConfig
                    {
                        letter = 'L',
                        assignedNumber = 17,
                        hideAtPositions = new List<int> { 0, 1 } // Hide both L's
                    }
                }
            });

            // Level 2: TURN YOUR WOUNDS INTO WISDOM
            levels.Add(new CryptoLevel
            {
                quote = "TURN YOUR WOUNDS INTO WISDOM",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig
                    {
                        letter = 'U',
                        assignedNumber = 6,
                        hideAtPositions = new List<int> { 0, 1 } // Hide first two U's
                    },
                    new HiddenLetterConfig
                    {
                        letter = 'N',
                        assignedNumber = 14,
                        hideAtPositions = new List<int> { 0, 1 } // Hide both N's
                    },
                    new HiddenLetterConfig
                    {
                        letter = 'I',
                        assignedNumber = 18,
                        hideAtPositions = new List<int> { 0, 1 } // Hide both I's
                    },
                    new HiddenLetterConfig
                    {
                        letter = 'M',
                        assignedNumber = 1,
                        hideAtPositions = new List<int> { 0 } // Hide M
                    }
                }
            });

            // Level 3
            levels.Add(new CryptoLevel
            {
                quote = "BE THE CHANGE YOU WISH TO SEE",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'E', assignedNumber = 5, hideAtPositions = new List<int> { 0, 2, 3 } },
                    new HiddenLetterConfig { letter = 'H', assignedNumber = 8, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 2, hideAtPositions = new List<int> { 0 } }
                }
            });

            // Level 4
            levels.Add(new CryptoLevel
            {
                quote = "DREAM BIG AND DARE TO FAIL",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 3, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'D', assignedNumber = 7, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'I', assignedNumber = 11, hideAtPositions = new List<int> { 0, 1 } }
                }
            });

            // Level 5
            levels.Add(new CryptoLevel
            {
                quote = "LIFE IS SHORT MAKE IT SWEET",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'I', assignedNumber = 9, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'S', assignedNumber = 15, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'E', assignedNumber = 4, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 6
            levels.Add(new CryptoLevel
            {
                quote = "BELIEVE YOU CAN AND YOU ARE HALFWAY THERE",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'E', assignedNumber = 12, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 16, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'Y', assignedNumber = 20, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 7
            levels.Add(new CryptoLevel
            {
                quote = "SUCCESS IS NOT FINAL FAILURE IS NOT FATAL",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'S', assignedNumber = 10, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'F', assignedNumber = 13, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 21, hideAtPositions = new List<int> { 0, 1, 2, 3 } }
                }
            });

            // Level 8
            levels.Add(new CryptoLevel
            {
                quote = "HAPPINESS IS A JOURNEY NOT A DESTINATION",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 22, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'N', assignedNumber = 23, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'O', assignedNumber = 24, hideAtPositions = new List<int> { 0, 1 } }
                }
            });

            // Level 9
            levels.Add(new CryptoLevel
            {
                quote = "THE BEST TIME TO PLANT A TREE WAS YESTERDAY",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'T', assignedNumber = 25, hideAtPositions = new List<int> { 0, 1, 2, 3, 4 } },
                    new HiddenLetterConfig { letter = 'E', assignedNumber = 26, hideAtPositions = new List<int> { 0, 1, 2, 3, 4 } },
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 27, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 10
            levels.Add(new CryptoLevel
            {
                quote = "EVERY MOMENT IS A FRESH BEGINNING",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'E', assignedNumber = 28, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'M', assignedNumber = 29, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'N', assignedNumber = 30, hideAtPositions = new List<int> { 0, 1, 2, 3 } }
                }
            });

            // Level 11
            levels.Add(new CryptoLevel
            {
                quote = "KINDNESS IS ALWAYS FASHIONABLE",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'N', assignedNumber = 31, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'S', assignedNumber = 32, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 33, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 12
            levels.Add(new CryptoLevel
            {
                quote = "COURAGE IS GRACE UNDER PRESSURE",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'R', assignedNumber = 34, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'E', assignedNumber = 35, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'U', assignedNumber = 36, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 13
            levels.Add(new CryptoLevel
            {
                quote = "STARS CANNOT SHINE WITHOUT DARKNESS",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'S', assignedNumber = 37, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'T', assignedNumber = 38, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'N', assignedNumber = 39, hideAtPositions = new List<int> { 0, 1, 2, 3 } }
                }
            });

            // Level 14
            levels.Add(new CryptoLevel
            {
                quote = "FOCUS ON THE GOOD AND THE GOOD GETS BETTER",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'O', assignedNumber = 40, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'G', assignedNumber = 41, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'T', assignedNumber = 42, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 15
            levels.Add(new CryptoLevel
            {
                quote = "SIMPLICITY IS THE ULTIMATE SOPHISTICATION",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'I', assignedNumber = 43, hideAtPositions = new List<int> { 0, 1, 2, 3, 4, 5 } },
                    new HiddenLetterConfig { letter = 'T', assignedNumber = 44, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'S', assignedNumber = 45, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 16
            levels.Add(new CryptoLevel
            {
                quote = "PATIENCE IS BITTER BUT ITS FRUIT IS SWEET",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'I', assignedNumber = 46, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'T', assignedNumber = 47, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'E', assignedNumber = 48, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 17
            levels.Add(new CryptoLevel
            {
                quote = "WISDOM BEGINS IN WONDER",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'W', assignedNumber = 49, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'I', assignedNumber = 50, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'N', assignedNumber = 51, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 18
            levels.Add(new CryptoLevel
            {
                quote = "LEARN FROM YESTERDAY LIVE FOR TODAY",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'L', assignedNumber = 52, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'E', assignedNumber = 53, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'R', assignedNumber = 54, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 19
            levels.Add(new CryptoLevel
            {
                quote = "GREAT THINGS NEVER COME FROM COMFORT ZONES",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'R', assignedNumber = 55, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'O', assignedNumber = 56, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'M', assignedNumber = 57, hideAtPositions = new List<int> { 0, 1 } }
                }
            });

            // Level 20
            levels.Add(new CryptoLevel
            {
                quote = "ACTION IS THE FOUNDATIONAL KEY TO SUCCESS",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 58, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'O', assignedNumber = 59, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'S', assignedNumber = 60, hideAtPositions = new List<int> { 0, 1 } }
                }
            });

            // Level 21
            levels.Add(new CryptoLevel
            {
                quote = "CREATIVITY TAKES COURAGE",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'C', assignedNumber = 61, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'R', assignedNumber = 62, hideAtPositions = new List<int> { 0, 1 } },
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 63, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });

            // Level 22
            levels.Add(new CryptoLevel
            {
                quote = "MISTAKES ARE PROOF THAT YOU ARE TRYING",
                hiddenLetters = new List<HiddenLetterConfig>
                {
                    new HiddenLetterConfig { letter = 'A', assignedNumber = 64, hideAtPositions = new List<int> { 0, 1, 2 } },
                    new HiddenLetterConfig { letter = 'R', assignedNumber = 65, hideAtPositions = new List<int> { 0, 1, 2, 3 } },
                    new HiddenLetterConfig { letter = 'T', assignedNumber = 66, hideAtPositions = new List<int> { 0, 1, 2 } }
                }
            });
        }
    }

    void SetupUI()
    {
      //  if (winPanel) winPanel.Deactivate();
      //  if (losePanel) losePanel.Deactivate();

        if (nextLevelButton) nextLevelButton.onClick.AddListener(NextLevel);
     //   if (retryButton) retryButton.onClick.AddListener(RetryLevel);

        CreateKeyboard();
    }

    public void Init()
    {
        currentLevel = 0;
        SelectRandomLevels();
        StartLevel();
        Game.Activate();
    }

    void SelectRandomLevels()
    {
        selectedLevels.Clear();

        if (levels.Count <= MAX_LEVELS_PER_GAME)
        {
            // If we have 5 or fewer levels, use all of them
            selectedLevels.AddRange(levels);
        }
        else
        {
            // Randomly select 5 levels
            List<CryptoLevel> tempLevels = new List<CryptoLevel>(levels);

            for (int i = 0; i < MAX_LEVELS_PER_GAME; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, tempLevels.Count);
                selectedLevels.Add(tempLevels[randomIndex]);
                tempLevels.RemoveAt(randomIndex);
            }
        }

        Debug.Log($"Selected {selectedLevels.Count} levels for this game session");
    }

    public void End()
    {
        gameActive = false;
        ClearLevel();

        // Hide all UI panels
        if (winPanel) winPanel.Deactivate();
        if (losePanel) losePanel.Deactivate();
    }

    void StartLevel()
    {
        if (currentLevel >= selectedLevels.Count)
        {
            ShowWinScreen();
            return;
        }

        gameActive = true;
        mistakes = 0;
        selectedSlot = null;

        ClearLevel();
        GenerateLevel(selectedLevels[currentLevel]);
        UpdateUI();
    }

    void ClearLevel()
    {
        // Clear letter slots
        foreach (var slot in letterSlots)
        {
            if (slot.gameObject) Destroy(slot.gameObject);
        }
        letterSlots.Clear();

        // Reset dictionaries
        letterToNumber.Clear();
        numberToLetter.Clear();
        letterCompleted.Clear();
        letterPositions.Clear();
        hiddenPositions.Clear();

        // Reset keyboard colors
        foreach (var kvp in keyButtons)
        {
            kvp.Value.background.color = defaultKeyColor;
        }
    }

    void GenerateLevel(CryptoLevel level)
    {
        string quote = level.quote.ToUpper();

        // First, track all positions of each letter in the quote
        int charIndex = 0;
        foreach (char c in quote)
        {
            if (c != ' ')
            {
                if (!letterPositions.ContainsKey(c))
                    letterPositions[c] = new List<int>();
                letterPositions[c].Add(charIndex);
                charIndex++;
            }
        }

        // Setup hidden letters configuration
        foreach (var config in level.hiddenLetters)
        {
            char letter = char.ToUpper(config.letter);
            letterToNumber[letter] = config.assignedNumber;
            numberToLetter[config.assignedNumber] = letter;
            letterCompleted[letter] = false;
            hiddenPositions[letter] = new List<int>();

            // Calculate actual positions to hide
            if (letterPositions.ContainsKey(letter))
            {
                var allPositions = letterPositions[letter];
                foreach (int hideIndex in config.hideAtPositions)
                {
                    if (hideIndex < allPositions.Count)
                    {
                        hiddenPositions[letter].Add(allPositions[hideIndex]);
                    }
                }
            }
        }

        // Create letter slots
        float xOffset = 0;
        float yOffset = 0;
        float slotWidth = 40;
        float slotHeight = 50;
        const float spacingY = 100f;
        float spacing = 5;
        float maxWidth = 800;

        int globalCharIndex = 0;

        foreach (char c in quote)
        {
            if (c == ' ')
            {
                xOffset += slotWidth / 2;
                if (xOffset > maxWidth)
                {
                    xOffset = 0;
                    yOffset -= slotHeight + spacingY;
                }
                continue;
            }

            GameObject slotObj = Instantiate(letterSlotPrefab, quoteContainer);
            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(xOffset - maxWidth / 2, yOffset);
            rt.sizeDelta = new Vector2(slotWidth, slotHeight);

            bool shouldHide = false;
            int assignedNum = 0;

            // Check if this position should be hidden
            if (letterToNumber.ContainsKey(c))
            {
                if (hiddenPositions.ContainsKey(c) && hiddenPositions[c].Contains(globalCharIndex))
                {
                    shouldHide = true;
                    assignedNum = letterToNumber[c];
                }
                else
                {
                    // This letter has a number but is shown (not hidden)
                    assignedNum = letterToNumber[c];
                }
            }

            LetterSlot slot = new LetterSlot
            {
                gameObject = slotObj,
                letterText = slotObj.transform.Find("Letter").GetComponent<TextMeshProUGUI>(),
                numberText = slotObj.transform.Find("Number").GetComponent<TextMeshProUGUI>(),
                button = slotObj.GetComponent<Button>(),
                background = slotObj.GetComponent<Image>(),
                correctLetter = c,
                isHidden = shouldHide,
                isFilled = false,
                positionIndex = globalCharIndex,
                assignedNumber = assignedNum,
                rectTransform = rt
            };

            if (shouldHide)
            {
                // Hidden letter - show empty slot with number
                slot.letterText.text = "";
                slot.numberText.text = assignedNum.ToString();
                slot.button.onClick.AddListener(() => SelectSlot(slot));
            }
            else if (assignedNum > 0)
            {
                // Visible letter with number hint
                slot.letterText.text = c.ToString();
                slot.numberText.text = assignedNum.ToString();
                slot.button.interactable = false;
                slot.background.color = new Color(0.9f, 0.9f, 0.9f);
            }
            else
            {
                // Normal visible letter without number
                slot.letterText.text = c.ToString();
                slot.numberText.text = "";
                slot.button.interactable = false;
                slot.background.color = new Color(0.9f, 0.9f, 0.9f);
            }

            letterSlots.Add(slot);
            globalCharIndex++;

            xOffset += slotWidth + spacing;
            if (xOffset > maxWidth)
            {
                xOffset = 0;
                yOffset -= slotHeight + spacingY;
            }
        }

        UpdateKeyboardColors();
    }

    void CreateKeyboard()
    {
        string[] keyboardLayout = new string[]
        {
            "QWERTYUIOP",
            "ASDFGHJKL",
            "ZXCVBNM"
        };

        float keyWidth = 60;
        float keyHeight = 60;
        float spacing = 5;
        float yOffset = 0;

        foreach (string row in keyboardLayout)
        {
            float xOffset = -(row.Length * (keyWidth + spacing)) / 2;

            foreach (char c in row)
            {
                GameObject keyObj = Instantiate(keyButtonPrefab, keyboardContainer);
                RectTransform rt = keyObj.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(xOffset + keyWidth / 2, yOffset);
                rt.sizeDelta = new Vector2(keyWidth, keyHeight);

                KeyButton keyBtn = new KeyButton
                {
                    gameObject = keyObj,
                    button = keyObj.GetComponent<Button>(),
                    text = keyObj.GetComponentInChildren<TextMeshProUGUI>(),
                    background = keyObj.GetComponent<Image>(),
                    letter = c
                };

                keyBtn.text.text = c.ToString();
                keyBtn.button.onClick.AddListener(() => OnKeyPressed(c));

                keyButtons[c] = keyBtn;

                xOffset += keyWidth + spacing;
            }

            yOffset -= keyHeight + spacing;
        }
    }

    void SelectSlot(LetterSlot slot)
    {
        if (!gameActive || !slot.isHidden || slot.isFilled) return;

        // Deselect previous slot
        if (selectedSlot != null)
        {
            LeanTween.scale(selectedSlot.rectTransform, Vector3.one, 0.2f);
            selectedSlot.background.color = Color.white;
        }

        // Select new slot
        selectedSlot = slot;
        LeanTween.scale(slot.rectTransform, Vector3.one * 1.1f, 0.2f)
            .setEaseOutBack();
        slot.background.color = new Color(1f, 1f, 0.5f);
    }

    void OnKeyPressed(char key)
    {
        if (!gameActive || selectedSlot == null) return;

        // Fill the selected slot
        selectedSlot.letterText.text = key.ToString();
        selectedSlot.isFilled = true;

        // Animate the letter appearing
        selectedSlot.letterText.transform.localScale = Vector3.zero;
        LeanTween.scale(selectedSlot.letterText.gameObject, Vector3.one, 0.3f)
            .setEaseOutBack();

        // Check if correct
        if (key == selectedSlot.correctLetter)
        {
            HandleCorrectGuess(selectedSlot);
        }
        else
        {
            HandleWrongGuess(selectedSlot);
        }

        // Deselect slot
        LeanTween.scale(selectedSlot.rectTransform, Vector3.one, 0.2f);
        selectedSlot.background.color = Color.white;
        selectedSlot = null;
    }

    void HandleCorrectGuess(LetterSlot slot)
    {
        // Flash green
        LeanTween.color(slot.background.rectTransform, correctColor, 0.2f)
            .setEaseOutQuad()
            .setOnComplete(() => {
                LeanTween.color(slot.background.rectTransform, Color.white, 0.3f);
            });

        // Check if all instances of this letter are filled
        CheckLetterCompletion(slot.correctLetter);
    }

    void HandleWrongGuess(LetterSlot slot)
    {
        mistakes++;

        // Flash red and shake
        LeanTween.color(slot.background.rectTransform, wrongColor, 0.2f)
            .setEaseOutQuad()
            .setOnComplete(() => {
                LeanTween.color(slot.background.rectTransform, Color.white, 0.3f);
            });

        // Shake animation
        LeanTween.moveLocalX(slot.gameObject, slot.rectTransform.anchoredPosition.x + 10, 0.1f)
            .setEaseShake()
            .setLoopPingPong(2);

        // Clear the wrong letter after animation
        LeanTween.delayedCall(0.5f, () => {
            slot.letterText.text = "";
            slot.isFilled = false;
        });

        UpdateUI();

        if (mistakes >= maxMistakes)
        {
            ShowLoseScreen();
        }
    }

    void CheckLetterCompletion(char letter)
    {
        bool allFilled = true;
        List<LetterSlot> hiddenSlotsForLetter = letterSlots.FindAll(s => s.correctLetter == letter && s.isHidden);

        foreach (var slot in hiddenSlotsForLetter)
        {
            if (!slot.isFilled || slot.letterText.text != letter.ToString())
            {
                allFilled = false;
                break;
            }
        }

        if (allFilled && hiddenSlotsForLetter.Count > 0)
        {
            letterCompleted[letter] = true;

            // Remove numbers with animation for ALL slots with this letter (hidden and visible)
            List<LetterSlot> allSlotsForLetter = letterSlots.FindAll(s => s.correctLetter == letter && s.assignedNumber > 0);
            foreach (var slot in allSlotsForLetter)
            {
                LeanTween.scale(slot.numberText.gameObject, Vector3.zero, 0.3f)
                    .setEaseInBack()
                    .setOnComplete(() => {
                        slot.numberText.text = "";
                    });
            }

            UpdateKeyboardColors();
            CheckLevelCompletion();
        }
    }

    void UpdateKeyboardColors()
    {
        foreach (var kvp in keyButtons)
        {
            char letter = kvp.Key;
            KeyButton keyBtn = kvp.Value;

            Color targetColor = defaultKeyColor;

            // Check if this letter is configured as hidden letter
            if (letterToNumber.ContainsKey(letter))
            {
                if (letterCompleted.ContainsKey(letter) && letterCompleted[letter])
                {
                    // Letter is completed - gray
                    targetColor = completedKeyColor;
                }
                else if (hiddenPositions.ContainsKey(letter) && hiddenPositions[letter].Count > 1)
                {
                    // Letter has hidden positions that need to be filled - green
                    targetColor = activeKeyColor;
                }
            }

            LeanTween.color(keyBtn.background.rectTransform, targetColor, 0.3f);
        }
    }

    void CheckLevelCompletion()
    {
        bool allCompleted = true;
        foreach (var kvp in letterCompleted)
        {
            if (!kvp.Value)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
        {
            LeanTween.delayedCall(1f, () => {
                if (currentLevel < selectedLevels.Count - 1)
                {
                     NextLevel();
                   
                }
                else
                {
                    ShowWinScreen();
                }
            });
        }
    }

    void NextLevel()
    {
        currentLevel++;
        StartLevel();
    }

    void RetryLevel()
    {
        if (losePanel) losePanel.Deactivate();
        StartLevel();
    }

    void ShowWinScreen()
    {
        gameActive = false;
        if (winPanel)
        {
            winPanel.Activate();
          
            
        }
    }

    void ShowLoseScreen()
    {
        gameActive = false;
        if (losePanel)
        {
            losePanel.Activate();
           
            
        }
    }

  

    void UpdateUI()
    {
        if (mistakesText)
            mistakesText.text = $"Mistakes: {mistakes}/{maxMistakes}";

        if (levelText)
            levelText.text = $"Level {currentLevel + 1}";
    }

    void Update()
    {
        if (!gameActive || selectedSlot == null) return;

        // Handle keyboard input
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                string keyName = keyCode.ToString();
                if (keyName.Length == 1)
                {
                    char key = keyName[0];
                    if (keyButtons.ContainsKey(key))
                    {
                        OnKeyPressed(key);
                    }
                }
            }
        }
    }
}

public class   CryptoQuest
{


    public Action OnFinishQuest;

    public void Win()
    {
        OnFinishQuest?.Invoke();
    }
}