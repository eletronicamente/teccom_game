using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CircuitGame.Circuit;
 
namespace CircuitGame
{
    public class CircuitVisualizer : MonoBehaviour
    {
        // ---- Slots dos componentes ----
        [Header("── Slots dos Componentes ─────────────")]
        [Tooltip("GameObject com CircuitElement — lado esquerdo (Bateria/Tensão)")]
        [SerializeField] private CircuitElement voltageSlot;
 
        [Tooltip("GameObject com CircuitElement — lado superior (Resistor)")]
        [SerializeField] private CircuitElement resistorSlot;
 
        [Tooltip("GameObject com CircuitElement — lado direito (Amperímetro/Corrente)")]
        [SerializeField] private CircuitElement currentSlot;
 
        // ---- Sprites dos componentes ----
        [Header("── Sprites ──────────────────────────")]
        [SerializeField] private Sprite batterySprite;    // ícone da bateria/tensão
        [SerializeField] private Sprite resistorSprite;   // ícone do resistor
        [SerializeField] private Sprite currentSprite;    // ícone do amperímetro/corrente
 
        // ---- Configuração do fio (LineRenderer) ----
        [Header("── Fios (LineRenderer) ─────────────")]
        [Tooltip("Material do fio — use o material padrão 'Sprites/Default' ou 'UI/Default'")]
        [SerializeField] private Material wireMaterial;
 
        [SerializeField] private Color  wireColor     = new Color(0.85f, 0.85f, 0.85f);
        [SerializeField] private float  wireThickness = 0.04f;
 
        // ---- RectTransform do painel do circuito ----
        [Header("── Painel ────────────────────────────")]
        [Tooltip("RectTransform do CircuitPanel onde o circuito será desenhado")]
        [SerializeField] private RectTransform circuitPanel;
 
        // ---- Tamanho do circuito (em unidades World) ----
        [Header("── Dimensões do Circuito ──────────────")]
        [SerializeField] private float circuitWidth  = 5f;
        [SerializeField] private float circuitHeight = 3f;
 
        // ---- Internos ----
        private LineRenderer _wireRenderer;
        private Coroutine    _pulseCoroutine;
 
        // =====================================================
        private void Awake()
        {
            // Cria o LineRenderer para desenhar os fios
            _wireRenderer = gameObject.AddComponent<LineRenderer>();
            _wireRenderer.material        = wireMaterial != null
                                             ? wireMaterial
                                             : new Material(Shader.Find("Sprites/Default"));
            _wireRenderer.startColor      = wireColor;
            _wireRenderer.endColor        = wireColor;
            _wireRenderer.startWidth      = wireThickness;
            _wireRenderer.endWidth        = wireThickness;
            _wireRenderer.useWorldSpace   = true;
            _wireRenderer.loop            = false;
            _wireRenderer.sortingOrder    = 1;
        }
 
        // =====================================================
        /// <summary>
        /// Chamado pelo UIManager a cada nova questão.
        /// Atualiza os 3 componentes e redesenha os fios.
        /// </summary>
        public void UpdateCircuit(CircuitQuestion question)
        {
            bool findV = question.Type == QuestionType.FindVoltage;
            bool findR = question.Type == QuestionType.FindResistance;
            bool findI = question.Type == QuestionType.FindCurrent;
 
            // Configura cada slot
            voltageSlot?.Setup(
                batterySprite,
                "Tensão",
                $"{question.Voltage:F1} V",
                isUnknown: findV
            );
 
            resistorSlot?.Setup(
                resistorSprite,
                "Resistência",
                $"{question.Resistance:F1} Ω",
                isUnknown: findR
            );
 
            currentSlot?.Setup(
                currentSprite,
                "Corrente",
                $"{question.Current:F1} A",
                isUnknown: findI
            );
 
            // Redesenha os fios
            DrawWires();
 
            // Pulso no componente desconhecido
            StopPulse();
            CircuitElement unknown = findV ? voltageSlot
                                   : findR ? resistorSlot
                                   : currentSlot;
            if (unknown != null)
                _pulseCoroutine = StartCoroutine(unknown.PulseUnknown());
        }
 
        // =====================================================
        /// <summary>
        /// Desenha o circuito retangular com fios usando LineRenderer.
        /// O retângulo é centrado no circuitPanel.
        /// </summary>
        private void DrawWires()
        {
            if (circuitPanel == null)
            {
                Debug.LogWarning("[CircuitVisualizer] circuitPanel não atribuído!");
                return;
            }
 
            // Centro do painel em coordenadas world
            Vector3 center = circuitPanel.position;
 
            float hw = circuitWidth  * 0.5f;   // half-width
            float hh = circuitHeight * 0.5f;   // half-height
 
            // 4 cantos do retângulo (sentido horário, começando canto sup-esq)
            Vector3 topLeft     = center + new Vector3(-hw,  hh, 0f);
            Vector3 topRight    = center + new Vector3( hw,  hh, 0f);
            Vector3 bottomRight = center + new Vector3( hw, -hh, 0f);
            Vector3 bottomLeft  = center + new Vector3(-hw, -hh, 0f);
 
            // Pontos dos fios:
            // Top-left → Top-right → Bottom-right → Bottom-left → Top-left
            // Os componentes ficam no meio de cada segmento,
            // mas o LineRenderer só desenha os "fios" entre eles.
            // Para uma aparência limpa, desenhamos o retângulo completo
            // e os componentes ficam sobrepostos nos slots corretos.
 
            _wireRenderer.positionCount = 5;
            _wireRenderer.SetPosition(0, topLeft);
            _wireRenderer.SetPosition(1, topRight);
            _wireRenderer.SetPosition(2, bottomRight);
            _wireRenderer.SetPosition(3, bottomLeft);
            _wireRenderer.SetPosition(4, topLeft);   // fecha o loop
 
            // Posiciona os slots nos pontos médios de cada lado
            PositionSlot(voltageSlot,  center + new Vector3(-hw, 0f,  0f));  // esquerda
            PositionSlot(resistorSlot, center + new Vector3(0f,   hh, 0f));  // topo
            PositionSlot(currentSlot,  center + new Vector3( hw,  0f, 0f));  // direita
        }
 
        /// <summary>
        /// Move o slot para a posição world dada, convertendo para RectTransform.
        /// </summary>
        private void PositionSlot(CircuitElement slot, Vector3 worldPos)
        {
            if (slot == null) return;
 
            var rt = slot.GetComponent<RectTransform>();
            if (rt == null) return;
 
            // Converte posição world → posição local dentro do canvas
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;
 
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;
 
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                circuitPanel, screenPoint, cam, out Vector2 localPoint);
 
            rt.anchoredPosition = localPoint;
        }
 
        // =====================================================
        private void StopPulse()
        {
            if (_pulseCoroutine != null)
            {
                StopCoroutine(_pulseCoroutine);
                _pulseCoroutine = null;
            }
        }
 
        private void OnDisable() => StopPulse();
    }
}