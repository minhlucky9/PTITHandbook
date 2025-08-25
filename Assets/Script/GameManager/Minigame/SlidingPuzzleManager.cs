using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlidingPuzzleManager : MonoBehaviour
{
    [Header("UI Setup")]
    [SerializeField] private RectTransform puzzleContainer;
    [SerializeField] private GameObject piecePrefab; // UI Button hoặc Image prefab
    [SerializeField] private Texture2D puzzleImage;

    [Header("Puzzle Settings")]
    [SerializeField] private int size = 4;
    [SerializeField] private float gapSize = 5f;

    [Header("Animation")]
    [SerializeField] private float moveAnimationDuration = 0.2f;
    [SerializeField] private LeanTweenType animationEase = LeanTweenType.easeOutCubic;

    private List<RectTransform> pieces;
    private List<Image> pieceImages;
    private int emptyLocation;
    private bool shuffling = false;
    private bool isAnimating = false;

    // Grid layout helper
    private Vector2 pieceSize;
    private Vector2 containerSize;

    void Start()
    {
        pieces = new List<RectTransform>();
        pieceImages = new List<Image>();

        if (puzzleContainer == null)
            puzzleContainer = GetComponent<RectTransform>();

        CreatePuzzlePieces();

        // Debug info
        DebugPuzzleState();

        // Shuffle sau khi tạo xong
        StartCoroutine(DelayedShuffle(1f));
    }

    // Debug method để kiểm tra trạng thái puzzle
    private void DebugPuzzleState()
    {
        Debug.Log($"=== Puzzle State Debug ===");
        Debug.Log($"Size: {size}x{size} = {size * size} total positions");
        Debug.Log($"Pieces array count: {pieces.Count}");
        Debug.Log($"Empty location (grid position): {emptyLocation}");

        // Show grid layout
        string gridDisplay = "Grid Layout:\n";
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                int gridIndex = row * size + col;
                if (gridIndex == emptyLocation)
                {
                    gridDisplay += "[EMPTY] ";
                }
                else if (pieces[gridIndex] != null)
                {
                    string pieceName = pieces[gridIndex].name;
                    string pieceNumber = pieceName.Replace("Piece_", "");
                    gridDisplay += $"[{pieceNumber}] ";
                }
                else
                {
                    gridDisplay += "[NULL] ";
                }
            }
            gridDisplay += "\n";
        }
        Debug.Log(gridDisplay);

        // Detailed piece info
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i] != null)
            {
                Debug.Log($"Grid pos {i}: {pieces[i].name} - Active: {pieces[i].gameObject.activeSelf}");
            }
            else
            {
                string status = (i == emptyLocation) ? "EMPTY SPACE" : "UNEXPECTED NULL";
                Debug.Log($"Grid pos {i}: NULL ({status})");
            }
        }
    }

    private void CreatePuzzlePieces()
    {
        // Tính toán kích thước container và pieces
        containerSize = puzzleContainer.rect.size;
        float totalGapX = gapSize * (size - 1);
        float totalGapY = gapSize * (size - 1);

        pieceSize = new Vector2(
            (containerSize.x - totalGapX) / size,
            (containerSize.y - totalGapY) / size
        );

        // Tạo từng piece
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                int index = (row * size) + col;

                // Tạo piece từ prefab
                GameObject pieceObj = Instantiate(piecePrefab, puzzleContainer);
                RectTransform pieceRect = pieceObj.GetComponent<RectTransform>();
                Image pieceImage = pieceObj.GetComponent<Image>();

                // Nếu prefab không có Image component, thêm vào
                if (pieceImage == null)
                    pieceImage = pieceObj.AddComponent<Image>();

                // Setup Button component nếu chưa có
                Button pieceButton = pieceObj.GetComponent<Button>();
                if (pieceButton == null)
                    pieceButton = pieceObj.AddComponent<Button>();

                // Add click listener
                int capturedIndex = index; // Capture for closure
                pieceButton.onClick.AddListener(() => OnPieceClicked(capturedIndex));

                // Set size và position
                pieceRect.sizeDelta = pieceSize;
                pieceRect.anchorMin = Vector2.zero;
                pieceRect.anchorMax = Vector2.zero;

                // Tính position (từ top-left)
                Vector2 position = new Vector2(
                    col * (pieceSize.x + gapSize),
                    -row * (pieceSize.y + gapSize) // Negative vì UI coordinate system
                );
                pieceRect.anchoredPosition = position;

                // Set name
                pieceObj.name = $"Piece_{index}";

                // Add to lists
                pieces.Add(pieceRect);
                pieceImages.Add(pieceImage);

                // Handle empty space (bottom-right piece)
                if (row == size - 1 && col == size - 1)
                {
                    emptyLocation = index; // Vị trí trống trong grid
                    pieceObj.SetActive(false);
                    // Vẫn add vào list nhưng set null để đánh dấu vị trí trống
                    pieces[pieces.Count - 1] = null;
                    pieceImages[pieceImages.Count - 1] = null;
                }
                else
                {
                    // Set up sprite từ puzzle image
                    if (puzzleImage != null)
                    {
                        Sprite pieceSprite = CreateSpriteFromTexture(puzzleImage, col, row);
                        pieceImage.sprite = pieceSprite;
                    }
                    else
                    {
                        // Fallback: tạo màu random hoặc số
                        pieceImage.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f);

                        // Thêm text hiển thị số
                        GameObject textObj = new GameObject("Text");
                        textObj.transform.SetParent(pieceObj.transform, false);
                        Text text = textObj.AddComponent<Text>();
                        text.text = index.ToString();
                        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                        text.fontSize = Mathf.RoundToInt(pieceSize.x * 0.3f);
                        text.alignment = TextAnchor.MiddleCenter;
                        text.color = Color.white;

                        RectTransform textRect = text.GetComponent<RectTransform>();
                        textRect.anchorMin = Vector2.zero;
                        textRect.anchorMax = Vector2.one;
                        textRect.offsetMin = Vector2.zero;
                        textRect.offsetMax = Vector2.zero;
                    }
                }
            }
        }
    }

    private Sprite CreateSpriteFromTexture(Texture2D sourceTexture, int col, int row)
    {
        int pieceWidth = sourceTexture.width / size;
        int pieceHeight = sourceTexture.height / size;

        Rect spriteRect = new Rect(
            col * pieceWidth,
            (size - 1 - row) * pieceHeight, // Flip Y vì texture coordinate
            pieceWidth,
            pieceHeight
        );

        return Sprite.Create(sourceTexture, spriteRect, new Vector2(0.5f, 0.5f), 100f);
    }

    private void OnPieceClicked(int originalPieceIndex)
    {
        if (shuffling || isAnimating) return;

        // Tìm vị trí hiện tại của piece này trong grid
        int currentGridPosition = -1;
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i] != null && pieces[i].name == $"Piece_{originalPieceIndex}")
            {
                currentGridPosition = i;
                break;
            }
        }

        if (currentGridPosition == -1) return;

        // Check valid moves
        if (IsValidMove(currentGridPosition))
        {
            StartCoroutine(AnimateSwap(currentGridPosition, emptyLocation));
        }
    }

    private bool IsValidMove(int pieceIndex)
    {
        int row = pieceIndex / size;
        int col = pieceIndex % size;
        int emptyRow = emptyLocation / size;
        int emptyCol = emptyLocation % size;

        // Check if adjacent (same row/col and distance of 1)
        bool sameRow = (row == emptyRow) && Mathf.Abs(col - emptyCol) == 1;
        bool sameCol = (col == emptyCol) && Mathf.Abs(row - emptyRow) == 1;

        return sameRow || sameCol;
    }

    private IEnumerator AnimateSwap(int pieceGridPosition, int emptyGridPosition)
    {
        isAnimating = true;

        // Get piece at the grid position
        RectTransform pieceToMove = pieces[pieceGridPosition];
        if (pieceToMove == null)
        {
            isAnimating = false;
            yield break;
        }

        // Get target position for the piece
        Vector2 targetPosition = GetPositionForIndex(emptyGridPosition);

        // Animate piece to empty position
        if (moveAnimationDuration > 0)
        {
            LeanTween.move(pieceToMove, targetPosition, moveAnimationDuration)
                    .setEase(animationEase);

            yield return new WaitForSeconds(moveAnimationDuration);
        }
        else
        {
            pieceToMove.anchoredPosition = targetPosition;
        }

        // Swap in pieces array: move piece to empty position, set old position to null
        pieces[emptyGridPosition] = pieces[pieceGridPosition];
        pieces[pieceGridPosition] = null;

        // Swap images array too
        pieceImages[emptyGridPosition] = pieceImages[pieceGridPosition];
        pieceImages[pieceGridPosition] = null;

        // Update empty location
        emptyLocation = pieceGridPosition;

        isAnimating = false;

        // Check completion
        if (CheckCompletion())
        {
            OnPuzzleCompleted();
        }
    }

    private Vector2 GetPositionForIndex(int index)
    {
        int row = index / size;
        int col = index % size;

        return new Vector2(
            col * (pieceSize.x + gapSize),
            -row * (pieceSize.y + gapSize)
        );
    }

    private bool CheckCompletion()
    {
        // Kiểm tra nếu tất cả pieces đang ở đúng vị trí
        for (int i = 0; i < size * size - 1; i++) // Không check empty space
        {
            if (pieces[i] == null) return false; // Vị trí này không nên trống

            // Extract original piece number từ name
            string pieceName = pieces[i].name;
            if (pieceName.StartsWith("Piece_"))
            {
                if (int.TryParse(pieceName.Substring(6), out int originalIndex))
                {
                    if (originalIndex != i) return false; // Piece không ở đúng vị trí
                }
            }
        }

        // Empty space phải ở vị trí cuối cùng
        return emptyLocation == (size * size - 1);
    }

    private void OnPuzzleCompleted()
    {
        Debug.Log("Puzzle Completed!");

        // Hiện empty piece - kiểm tra bounds
        if (emptyLocation >= 0 && emptyLocation < pieces.Count && pieces[emptyLocation] != null)
        {
            pieces[emptyLocation].gameObject.SetActive(true);
        }

        // Optional: Effect hoặc callback
        StartCoroutine(DelayedShuffle(2f));
    }

    private IEnumerator DelayedShuffle(float delay)
    {
        yield return new WaitForSeconds(delay);
        shuffling = true;

        // Ẩn empty piece trở lại - kiểm tra bounds
        if (emptyLocation >= 0 && emptyLocation < pieces.Count && pieces[emptyLocation] != null)
        {
            pieces[emptyLocation].gameObject.SetActive(false);
        }

        Shuffle();
        shuffling = false;
    }

    private void Shuffle()
    {
        int shuffleCount = size * size * size;

        for (int i = 0; i < shuffleCount; i++)
        {
            List<int> validMoves = GetValidMoves();

            if (validMoves.Count > 0)
            {
                int randomPiecePosition = validMoves[Random.Range(0, validMoves.Count)];
                SwapInstant(randomPiecePosition, emptyLocation);
            }
        }
    }

    private List<int> GetValidMoves()
    {
        List<int> validMoves = new List<int>();

        int emptyRow = emptyLocation / size;
        int emptyCol = emptyLocation % size;

        // Check all four directions
        int[] directions = { -size, size, -1, 1 }; // Up, Down, Left, Right

        for (int i = 0; i < directions.Length; i++)
        {
            int newIndex = emptyLocation + directions[i];

            // Boundary checks
            if (newIndex >= 0 && newIndex < pieces.Count)
            {
                int newRow = newIndex / size;
                int newCol = newIndex % size;

                // Additional check for horizontal moves (prevent wrapping)
                if (directions[i] == -1 && emptyCol == 0) continue;
                if (directions[i] == 1 && emptyCol == size - 1) continue;

                validMoves.Add(newIndex);
            }
        }

        return validMoves;
    }

    private void SwapInstant(int pieceGridPosition, int emptyGridPosition)
    {
        // Kiểm tra bounds
        if (pieceGridPosition < 0 || pieceGridPosition >= pieces.Count ||
            emptyGridPosition < 0 || emptyGridPosition >= pieces.Count)
        {
            Debug.LogError($"SwapInstant: Invalid indices {pieceGridPosition}, {emptyGridPosition}");
            return;
        }

        // Kiểm tra rằng emptyGridPosition thực sự trống
        if (pieces[emptyGridPosition] != null)
        {
            Debug.LogError($"SwapInstant: Target position {emptyGridPosition} is not empty!");
            return;
        }

        // Move piece từ pieceGridPosition sang emptyGridPosition
        pieces[emptyGridPosition] = pieces[pieceGridPosition];
        pieces[pieceGridPosition] = null;

        // Move image reference
        pieceImages[emptyGridPosition] = pieceImages[pieceGridPosition];
        pieceImages[pieceGridPosition] = null;

        // Update position
        if (pieces[emptyGridPosition] != null)
        {
            pieces[emptyGridPosition].anchoredPosition = GetPositionForIndex(emptyGridPosition);
        }

        // Update empty location
        emptyLocation = pieceGridPosition;
    }

    // Public methods để control từ bên ngoài
    public void SetPuzzleImage(Texture2D newImage)
    {
        puzzleImage = newImage;
        UpdatePuzzleVisuals();
    }

    public void SetSize(int newSize)
    {
        if (newSize < 2 || newSize > 10) return;

        size = newSize;
        ClearPuzzle();
        CreatePuzzlePieces();
    }

    private void UpdatePuzzleVisuals()
    {
        if (puzzleImage == null) return;

        for (int i = 0; i < pieces.Count; i++)
        {
            if (i != emptyLocation)
            {
                int row = i / size;
                int col = i % size;
                Sprite newSprite = CreateSpriteFromTexture(puzzleImage, col, row);
                pieceImages[i].sprite = newSprite;
            }
        }
    }

    private void ClearPuzzle()
    {
        foreach (RectTransform piece in pieces)
        {
            if (piece != null)
                DestroyImmediate(piece.gameObject);
        }

        pieces.Clear();
        pieceImages.Clear();
    }

    private void OnDestroy()
    {
        ClearPuzzle();
    }
}