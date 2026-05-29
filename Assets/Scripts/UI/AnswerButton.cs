using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CircuitGame.Core;
 
namespace CircuitGame
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]   // garante que Image existe no GameObject
    public class AnswerButton : MonoBehaviour
    {
        // ---- Referências (podem ser deixadas vazias no Inspector — serão
        //      encontradas automaticamente no Awake) ----
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image           buttonImage;
 
        // ---- Cores ----
        [Header("Cores do Botão")]
        [SerializeField] private Color defaultColor  = new Color(0.15f, 0.15f, 0.25f, 1f);
        [SerializeField] private Color correctColor  = new Color(0.1f,  0.7f,  0.2f,  1f);
        [SerializeField] private Color selectedColor = new Color(0.3f,  0.5f,  0.9f,  1f);
 
        // ---- Estado interno ----
        private float  _value;
        private bool   _isCorrect;
        private bool   _isTutorial;
        private Button _button;
 
        // =====================================================
        private void Awake()
        {
            // Pega o componente Button do PRÓPRIO GameObject
            _button = GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClicked);
 
            // Se buttonImage não foi atribuído no Inspector, busca automaticamente
            if (buttonImage == null)
                buttonImage = GetComponent<Image>();
 
            // Se valueText não foi atribuído, tenta achar no filho
            if (valueText == null)
                valueText = GetComponentInChildren<TextMeshProUGUI>();
 
            // Avisa no console se ainda não encontrou (facilita debug)
            if (buttonImage == null)
                Debug.LogError($"[AnswerButton] buttonImage não encontrado em '{gameObject.name}'!");
 
            if (valueText == null)
                Debug.LogError($"[AnswerButton] valueText (TMP) não encontrado em '{gameObject.name}'!");
        }
 
        // =====================================================
        /// <summary>
        /// Configura o botão com valor, unidade e comportamento.
        /// Chamado pelo UIManager a cada nova questão.
        /// </summary>
        public void Setup(float value, string unit, bool isCorrect, bool isTutorial)
        {
            _value     = value;
            _isCorrect = isCorrect;
            _isTutorial = isTutorial;
 
            // Exibe o valor formatado (ex: "94,0 V")
            if (valueText != null)
                valueText.text = $"{value:F1} {unit}";
 
            // Cor padrão — no tutorial destaca o correto em verde
            if (buttonImage != null)
                buttonImage.color = (isTutorial && isCorrect) ? correctColor : defaultColor;
 
            // No tutorial o botão não é clicável (só observação)
            if (_button != null)
                _button.interactable = !isTutorial;
        }
 
        // =====================================================
        private void OnClicked()
        {
            // Muda a cor para indicar seleção visual
            if (buttonImage != null)
                buttonImage.color = selectedColor;
 
            // Verifica se o GameManager existe antes de chamar
            if (GameManager.Instance == null)
            {
                Debug.LogError("[AnswerButton] GameManager.Instance é null! " +
                               "Verifique se o objeto GameManager está na cena.");
                return;
            }
 
            GameManager.Instance.OnAnswerSelected(_value);
        }
 
        // =====================================================
        /// <summary>
        /// Reseta a cor do botão para o padrão (chamado ao carregar nova fase).
        /// </summary>
        public void ResetColor()
        {
            if (buttonImage != null)
                buttonImage.color = defaultColor;
 
            if (_button != null)
                _button.interactable = true;
        }
    }
}