using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// IMPORTANTE: Este componente deve ficar em um GameObject dentro de um Canvas
// (ele próprio precisa ter um RectTransform, pois RawImage é um componente de UI).
[RequireComponent(typeof(RectTransform))]
public class SimpleParticleSystem : MonoBehaviour
{
    [Header("Textura da Partícula")]
    [SerializeField] private Texture2D particleTexture;

    [Header("Quantidade e Emissão")]
    [SerializeField] private int maxParticles = 30;
    [SerializeField] private float spawnInterval = 0.3f; // tempo entre cada nova partícula

    [Header("Área de Emissão (offset em relação à posição deste objeto)")]
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-1f, 0f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(1f, 0f);

    [Header("Movimento")]
    [SerializeField] private Vector2 moveDirection = Vector2.up; // direção principal (ex: bolhas sobem)
    [SerializeField] private Vector2 speedRange = new Vector2(0.5f, 1.5f);
    [SerializeField] private float horizontalWobble = 0.3f; // variação lateral, tipo bolha balançando
    [SerializeField] private float moveSpeedMultiplier = 20f; // ajuste fino: pixels/seg aplicados sobre o speedRange

    [Header("Tamanho (em pixels, tamanho do RawImage)")]
    [SerializeField] private Vector2 startSizeRange = new Vector2(40f, 80f);
    [SerializeField] private bool growOverLifetime = false;

    [Header("Tempo de Vida")]
    [SerializeField] private Vector2 lifetimeRange = new Vector2(2f, 4f);
    [SerializeField] private bool fadeOut = true;

    [Header("Cor")]
    [SerializeField] private Color startColor = Color.white;

    [Header("Ordenação Visual (índice na hierarquia do Canvas)")]
    [SerializeField] private bool sendToFront = false; // se true, cada nova partícula fica na frente das demais

    private readonly List<Particle> activeParticles = new List<Particle>();
    private float spawnTimer;
    private RectTransform selfRect;

    private class Particle
    {
        public GameObject gameObject;
        public RectTransform rectTransform;
        public RawImage rawImage;
        public Vector2 velocity;
        public float wobbleSpeed;
        public float wobbleOffset;
        public float startSize;
        public float age;
        public float lifetime;
    }

    private void Awake()
    {
        selfRect = GetComponent<RectTransform>();

        if (particleTexture == null)
        {
            Debug.LogWarning($"[{nameof(SimpleParticleSystem)}] Nenhuma textura atribuída em '{gameObject.name}'.");
        }
    }

    private void Update()
    {
        if (particleTexture == null) return;

        HandleSpawning();
        HandleParticles();
    }

    private void HandleSpawning()
    {
        if (activeParticles.Count >= maxParticles) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnParticle();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnParticle()
    {
        GameObject go = new GameObject("Particle", typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(selfRect, worldPositionStays: false);

        float offsetX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float offsetY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        rt.anchoredPosition = new Vector2(offsetX, offsetY);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);

        RawImage img = go.AddComponent<RawImage>();
        img.texture = particleTexture;
        img.color = startColor;
        img.raycastTarget = false; // partículas não devem bloquear cliques na UI

        if (sendToFront)
        {
            rt.SetAsLastSibling();
        }

        float size = Random.Range(startSizeRange.x, startSizeRange.y);
        rt.sizeDelta = new Vector2(size, size);

        float speed = Random.Range(speedRange.x, speedRange.y);
        Vector2 velocity = moveDirection.normalized * speed;

        Particle p = new Particle
        {
            gameObject = go,
            rectTransform = rt,
            rawImage = img,
            velocity = velocity,
            wobbleSpeed = Random.Range(1f, 3f),
            wobbleOffset = Random.Range(0f, Mathf.PI * 2f),
            startSize = size,
            age = 0f,
            lifetime = Random.Range(lifetimeRange.x, lifetimeRange.y)
        };

        activeParticles.Add(p);
    }

    private void HandleParticles()
    {
        for (int i = activeParticles.Count - 1; i >= 0; i--)
        {
            Particle p = activeParticles[i];
            p.age += Time.deltaTime;

            if (p.age >= p.lifetime)
            {
                Destroy(p.gameObject);
                activeParticles.RemoveAt(i);
                continue;
            }

            // Movimento principal + oscilação lateral (efeito de bolha balançando)
            float wobble = Mathf.Sin((p.age * p.wobbleSpeed) + p.wobbleOffset) * horizontalWobble;
            Vector2 move = new Vector2(
                (p.velocity.x + wobble) * Time.deltaTime,
                p.velocity.y * Time.deltaTime
            );
            // Como estamos em UI, a posição é medida em pixels; o moveSpeedMultiplier
            // ajusta o "peso" visual do deslocamento sem precisar mexer no código.
            p.rectTransform.anchoredPosition += move * moveSpeedMultiplier;

            float lifeRatio = p.age / p.lifetime;

            // Cresce ao longo da vida (opcional)
            if (growOverLifetime)
            {
                float scale = p.startSize * (1f + lifeRatio);
                p.rectTransform.sizeDelta = new Vector2(scale, scale);
            }

            // Fade out (opcional)
            if (fadeOut)
            {
                Color c = p.rawImage.color;
                c.a = startColor.a * (1f - lifeRatio);
                p.rawImage.color = c;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var p in activeParticles)
        {
            if (p.gameObject != null)
                Destroy(p.gameObject);
        }
        activeParticles.Clear();
    }
}