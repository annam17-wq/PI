using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class RawImageRandomMover : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private RectTransform rectTransform;

    [Header("Range de Movimento (offset em relação à posição original, em pixels)")]
    [SerializeField] private Vector2 rangeX = new Vector2(-200f, 200f);
    [SerializeField] private Vector2 rangeY = new Vector2(-100f, 100f);

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Vector2 timeBetweenMoves = new Vector2(1f, 3f);

    [Header("Flip do eixo X (baseado na direção do movimento)")]
    [SerializeField] private bool enableFlip = true;

    private Vector2 originalPosition;
    private Vector2 targetPosition;
    private float moveTimer;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        PickNewTargetPosition();
        ResetMoveTimer();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );

        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            PickNewTargetPosition();
            ResetMoveTimer();
        }
    }

    private void PickNewTargetPosition()
    {
        float currentX = rectTransform.anchoredPosition.x;

        float offsetX = Random.Range(rangeX.x, rangeX.y);
        float offsetY = Random.Range(rangeY.x, rangeY.y);
        targetPosition = originalPosition + new Vector2(offsetX, offsetY);

        if (enableFlip)
            UpdateFlip(currentX, targetPosition.x);
    }

    private void UpdateFlip(float fromX, float toX)
    {
        // Se o novo destino é MAIOR que a posição atual -> flipa
        // Se é MENOR (ou igual) -> mantém sem flip
        bool shouldFlip = toX > fromX;
        SetFlipX(shouldFlip);
    }

    private void SetFlipX(bool flip)
    {
        Vector3 scale = rectTransform.localScale;
        float absX = Mathf.Abs(scale.x);
        scale.x = flip ? -absX : absX;
        rectTransform.localScale = scale;
    }

    private void ResetMoveTimer()
    {
        moveTimer = Random.Range(timeBetweenMoves.x, timeBetweenMoves.y);
    }
}