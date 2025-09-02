using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlidingPuzzleManager : MonoBehaviour
{
    [Header("UI Setup")]
    [SerializeField] private RectTransform puzzleContainer;
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private Texture2D puzzleImage;

    [Header("Puzzle Settings")]
    [SerializeField] private int size = 3;
    [SerializeField] private float gapSize = 5f;
    [SerializeField] private bool emptyAtStart = true; // Empty ở vị trí đầu (0,0)

    [Header("Animation")]
    [SerializeField] private float moveAnimationDuration = 0.2f;

    private List<GameObject> pieces; // Danh sách các piece objects
    private int emptyIndex; // Vị trí hiện tại của ô trống
    private Vector2 pieceSize;
    private bool isMoving = false;

    public event System.Action PuzzleCompleted;

    void Start()
    {
        InitializePuzzle();
    }

    void InitializePuzzle()
    {
        pieces = new List<GameObject>();

        if (puzzleContainer == null)
            puzzleContainer = GetComponent<RectTransform>();

        CreatePuzzlePieces();
        StartCoroutine(ShuffleAfterDelay(1f));
    }

    void CreatePuzzlePieces()
    {
        // Clear existing pieces
        foreach (Transform child in puzzleContainer)
        {
            DestroyImmediate(child.gameObject);
        }
        pieces.Clear();

        // Calculate piece size
        Vector2 containerSize = puzzleContainer.rect.size;
        float totalGapX = gapSize * (size - 1);
        float totalGapY = gapSize * (size - 1);

        pieceSize = new Vector2(
            (containerSize.x - totalGapX) / size,
            (containerSize.y - totalGapY) / size
        );

        // Create pieces
        for (int i = 0; i < size * size; i++)
        {
            int row = i / size;
            int col = i % size;

            GameObject pieceObj = Instantiate(piecePrefab, puzzleContainer);
            pieces.Add(pieceObj);

            // Setup RectTransform
            RectTransform pieceRect = pieceObj.GetComponent<RectTransform>();
            pieceRect.sizeDelta = pieceSize;
            pieceRect.anchorMin = Vector2.zero;
            pieceRect.anchorMax = Vector2.zero;

            Vector2 position = new Vector2(
                col * (pieceSize.x + gapSize),
                -row * (pieceSize.y + gapSize)
            );
            pieceRect.anchoredPosition = position;

            // Setup piece
            if (emptyAtStart && i == 0) // Empty at position 0
            {
                emptyIndex = 0;
                pieceObj.SetActive(false);
                pieceObj.name = "EmptySpace";
            }
            else if (!emptyAtStart && i == size * size - 1) // Empty at last position
            {
                emptyIndex = size * size - 1;
                pieceObj.SetActive(false);
                pieceObj.name = "EmptySpace";
            }
            else
            {
                SetupPieceVisual(pieceObj, i, row, col);
                SetupPieceButton(pieceObj, i);
            }
        }
    }

    void SetupPieceVisual(GameObject pieceObj, int index, int row, int col)
    {
        Image pieceImage = pieceObj.GetComponent<Image>();
        if (pieceImage == null)
            pieceImage = pieceObj.AddComponent<Image>();

        if (puzzleImage != null)
        {
            // Create sprite from puzzle image
            int pieceWidth = puzzleImage.width / size;
            int pieceHeight = puzzleImage.height / size;

            Rect spriteRect = new Rect(
                col * pieceWidth,
                (size - 1 - row) * pieceHeight, // Flip Y
                pieceWidth,
                pieceHeight
            );

            Sprite pieceSprite = Sprite.Create(puzzleImage, spriteRect, new Vector2(0.5f, 0.5f));
            pieceImage.sprite = pieceSprite;
            pieceImage.color = Color.white;
        }
        else
        {
            // Fallback color and number
            float hue = (index * 0.618f) % 1f;
            pieceImage.color = Color.HSVToRGB(hue, 0.6f, 0.9f);

            // Add text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(pieceObj.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.text = (index + 1).ToString();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = Mathf.RoundToInt(pieceSize.x * 0.3f);
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontStyle = FontStyle.Bold;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        pieceObj.name = $"Piece_{index}";
    }

    void SetupPieceButton(GameObject pieceObj, int index)
    {
        Button button = pieceObj.GetComponent<Button>();
        if (button == null)
            button = pieceObj.AddComponent<Button>();

        button.onClick.AddListener(() => OnPieceClicked(index));
    }

    void OnPieceClicked(int originalIndex)
    {
        if (isMoving) return;

        // Find current position of this piece
        int currentPos = FindPiecePosition(originalIndex);
        if (currentPos == -1) return;

        // Check if can move to empty space
        if (CanMovePiece(currentPos, emptyIndex))
        {
            StartCoroutine(MovePieceToEmpty(currentPos));
        }
    }

    int FindPiecePosition(int originalIndex)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name == $"Piece_{originalIndex}")
            {
                return i;
            }
        }
        return -1;
    }

    bool CanMovePiece(int piecePos, int emptyPos)
    {
        int pieceRow = piecePos / size;
        int pieceCol = piecePos % size;
        int emptyRow = emptyPos / size;
        int emptyCol = emptyPos % size;

        // Check if adjacent
        bool sameRow = (pieceRow == emptyRow) && Mathf.Abs(pieceCol - emptyCol) == 1;
        bool sameCol = (pieceCol == emptyCol) && Mathf.Abs(pieceRow - emptyRow) == 1;

        return sameRow || sameCol;
    }

    IEnumerator MovePieceToEmpty(int piecePosition)
    {
        isMoving = true;

        GameObject pieceToMove = pieces[piecePosition];
        Vector2 targetPosition = GetPositionForIndex(emptyIndex);

        if (moveAnimationDuration > 0)
        {
            Vector2 startPos = pieceToMove.GetComponent<RectTransform>().anchoredPosition;
            float elapsed = 0f;

            while (elapsed < moveAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / moveAnimationDuration;
                t = Mathf.SmoothStep(0f, 1f, t); // Smooth animation

                pieceToMove.GetComponent<RectTransform>().anchoredPosition =
                    Vector2.Lerp(startPos, targetPosition, t);

                yield return null;
            }
        }

        pieceToMove.GetComponent<RectTransform>().anchoredPosition = targetPosition;

        // Swap pieces in list
        (pieces[piecePosition], pieces[emptyIndex]) = (pieces[emptyIndex], pieces[piecePosition]);
        emptyIndex = piecePosition;

        isMoving = false;

        // Check win condition
        if (CheckWinCondition())
        {
            OnPuzzleComplete();
        }
    }

    Vector2 GetPositionForIndex(int index)
    {
        int row = index / size;
        int col = index % size;

        return new Vector2(
            col * (pieceSize.x + gapSize),
            -row * (pieceSize.y + gapSize)
        );
    }

    bool CheckWinCondition()
    {
        // Check if empty is in correct position
        int correctEmptyPos = emptyAtStart ? 0 : (size * size - 1);
        if (emptyIndex != correctEmptyPos) return false;

        // Check if all pieces are in correct positions
        for (int i = 0; i < pieces.Count; i++)
        {
            if (i == correctEmptyPos) continue; // Skip empty space

            if (pieces[i].name != $"Piece_{i}")
                return false;
        }

        return true;
    }

    void OnPuzzleComplete()
    {
        Debug.Log("Puzzle Complete!");

        // Show empty piece temporarily
        pieces[emptyIndex].SetActive(true);

        PuzzleCompleted?.Invoke();

        // Auto shuffle after delay
        StartCoroutine(ShuffleAfterDelay(2f));
    }

    IEnumerator ShuffleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Hide empty piece
        pieces[emptyIndex].SetActive(false);

        // Perform shuffle
        for (int i = 0; i < size * size * 10; i++)
        {
            List<int> validMoves = GetValidMoves();
            if (validMoves.Count > 0)
            {
                int randomMove = validMoves[Random.Range(0, validMoves.Count)];
                SwapInstant(randomMove, emptyIndex);
            }
        }
    }

    List<int> GetValidMoves()
    {
        List<int> validMoves = new List<int>();

        int emptyRow = emptyIndex / size;
        int emptyCol = emptyIndex % size;

        // Check four directions
        int[] directions = { -size, size, -1, 1 }; // Up, Down, Left, Right

        foreach (int dir in directions)
        {
            int newPos = emptyIndex + dir;

            if (newPos >= 0 && newPos < pieces.Count)
            {
                int newRow = newPos / size;
                int newCol = newPos % size;

                // Check for horizontal wrapping
                if (dir == -1 && emptyCol == 0) continue;
                if (dir == 1 && emptyCol == size - 1) continue;

                validMoves.Add(newPos);
            }
        }

        return validMoves;
    }

    void SwapInstant(int pos1, int pos2)
    {
        // Swap pieces
        (pieces[pos1], pieces[pos2]) = (pieces[pos2], pieces[pos1]);

        // Update positions
        pieces[pos1].GetComponent<RectTransform>().anchoredPosition = GetPositionForIndex(pos1);
        pieces[pos2].GetComponent<RectTransform>().anchoredPosition = GetPositionForIndex(pos2);

        // Update empty index
        emptyIndex = pos1;
    }

    // Public methods
    public void SetPuzzleImage(Texture2D newImage)
    {
        puzzleImage = newImage;
        CreatePuzzlePieces();
    }

    public void SetSize(int newSize)
    {
        if (newSize >= 2 && newSize <= 6)
        {
            size = newSize;
            CreatePuzzlePieces();
        }
    }

    public void ToggleEmptyPosition()
    {
        emptyAtStart = !emptyAtStart;
        CreatePuzzlePieces();
    }
}