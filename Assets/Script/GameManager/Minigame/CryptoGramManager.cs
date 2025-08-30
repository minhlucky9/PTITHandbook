using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CryptoGramManager : MonoBehaviour
{
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
        }
    }

    void SetupUI()
    {
        if (winPanel) winPanel.Deactivate();
        if (losePanel) losePanel.Deactivate();

        if (nextLevelButton) nextLevelButton.onClick.AddListener(NextLevel);
        if (retryButton) retryButton.onClick.AddListener(RetryLevel);

        CreateKeyboard();
    }

    public void Init()
    {
        currentLevel = 0;
        StartLevel();
        Game.Activate();
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
        if (currentLevel >= levels.Count)
        {
            ShowWinScreen();
            return;
        }

        gameActive = true;
        mistakes = 0;
        selectedSlot = null;

        ClearLevel();
        GenerateLevel(levels[currentLevel]);
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
                else if (hiddenPositions.ContainsKey(letter) && hiddenPositions[letter].Count > 0)
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
                if (currentLevel < levels.Count - 1)
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