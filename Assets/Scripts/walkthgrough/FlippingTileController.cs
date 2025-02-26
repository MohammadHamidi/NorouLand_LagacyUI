using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FlippingTileController : MonoBehaviour
{
    [Header("Container & Layout Settings")]
    [Tooltip("Parent container (RectTransform) that holds the tile children.")]
    public RectTransform container;
    [Tooltip("Number of columns for the grid layout.")]
    public int columns = 3;
    [Tooltip("Horizontal spacing between tiles.")]
    public float spacingX = 10f;
    [Tooltip("Vertical spacing between tiles.")]
    public float spacingY = 10f;

    [Header("Flip Animation Settings")]
    [Tooltip("Total duration of one flip (scale down and up).")]
    public float flipDuration = 0.5f;
    [Tooltip("Minimum delay before a tile starts its flip animation.")]
    public float minFlipDelay = 0.0f;
    [Tooltip("Maximum delay before a tile starts its flip animation.")]
    public float maxFlipDelay = 2.0f;
    [Tooltip("Should each tile flip continuously?")]
    public bool loopAnimation = true;
    [Tooltip("Randomize the flip start delay for each tile.")]
    public bool randomizeFlip = true;

    [Header("Tile Sprites")]
    [Tooltip("The default sprite shown on the tile.")]
    public Sprite originalSprite;
    [Tooltip("The sprite to show when flipped.")]
    public Sprite flippedSprite;

    // Calculated cell dimensions.
    private float cellWidth;
    private float cellHeight;
    // Starting position for the first cell (top-left of the container).
    private Vector2 gridStart = Vector2.zero;

    private void Awake()
    {
        if (container == null)
            container = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Wait a frame so layout is ready.
        StartCoroutine(SetupAndAnimateTiles());
    }

    // Update layout when container dimensions change.
    private void OnRectTransformDimensionsChange()
    {
        if (Application.isPlaying)
            LayoutTiles();
    }

    /// <summary>
    /// Sets up the grid layout for the child tiles and starts their flip animations.
    /// </summary>
    private IEnumerator SetupAndAnimateTiles()
    {
        yield return new WaitForEndOfFrame();
        LayoutTiles();
        StartFlipAnimations();
    }

    /// <summary>
    /// Calculates the grid layout: each tile is resized and repositioned based on the parent's size.
    /// </summary>
    private void LayoutTiles()
    {
        // Get container dimensions.
        float parentWidth = container.rect.width;
        float parentHeight = container.rect.height;

        // Determine number of rows based on child count.
        int tileCount = container.childCount;
        int rows = Mathf.CeilToInt(tileCount / (float)columns);

        // Compute cell size.
        cellWidth = (parentWidth - (columns - 1) * spacingX) / columns;
        cellHeight = (parentHeight - (rows - 1) * spacingY) / rows;

        // Determine parent's top-left in local space.
        Vector3[] worldCorners = new Vector3[4];
        container.GetWorldCorners(worldCorners);
        Vector3 localTopLeft = container.InverseTransformPoint(worldCorners[1]);

        // Compute grid start so that the center of the first cell is at top-left + half cell.
        gridStart = new Vector2(localTopLeft.x + cellWidth * 0.5f, localTopLeft.y - cellHeight * 0.5f);

        // Position each child tile.
        for (int i = 0; i < tileCount; i++)
        {
            RectTransform tileRT = container.GetChild(i).GetComponent<RectTransform>();
            if (tileRT == null)
                continue;

            int row = i / columns;
            int col = i % columns;
            Vector2 pos = gridStart + new Vector2(col * (cellWidth + spacingX), -row * (cellHeight + spacingY));
            tileRT.sizeDelta = new Vector2(cellWidth, cellHeight);
            tileRT.anchoredPosition = pos;
        }
    }

    /// <summary>
    /// Starts a flip animation on each child tile.
    /// </summary>
    private void StartFlipAnimations()
    {
        int tileCount = container.childCount;
        for (int i = 0; i < tileCount; i++)
        {
            GameObject tile = container.GetChild(i).gameObject;
            Image img = tile.GetComponent<Image>();
            if (img == null)
                continue;

            // Kill any existing tweens on this tile.
            DOTween.Kill(tile.transform);
            // Reset scale.
            tile.transform.localScale = Vector3.one;

            // Determine a start delay.
            float startDelay = randomizeFlip ? Random.Range(minFlipDelay, maxFlipDelay) : minFlipDelay;

            // Create a sequence for this tile.
            Sequence flipSeq = DOTween.Sequence();
            flipSeq.AppendInterval(startDelay);
            flipSeq.Append(tile.transform.DOScaleX(0f, flipDuration / 2f).SetEase(Ease.InQuad));
            flipSeq.AppendCallback(() =>
            {
                // Swap sprite.
                img.sprite = (img.sprite == originalSprite) ? flippedSprite : originalSprite;
            });
            flipSeq.Append(tile.transform.DOScaleX(1f, flipDuration / 2f).SetEase(Ease.OutQuad));
            if (loopAnimation)
            {
                flipSeq.SetLoops(-1, LoopType.Restart);
            }
        }
    }
}
