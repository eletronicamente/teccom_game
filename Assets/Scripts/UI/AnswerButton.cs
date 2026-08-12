using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CircuitGame.Core;
 
namespace CircuitGame
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class AnswerButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image           buttonImage;
        // iconImage REMOVIDO — não precisa mais de filho "Icon"
 
        [Header("Cores do Botão")]
        [SerializeField] private Color defaultColor  = new Color(1f, 1f, 1f, 1f);    // branco = sprite na cor original
        [SerializeField] private Color correctColor  = new Color(0.4f, 1f, 0.4f, 1f); // tint verde
        [SerializeField] private Color selectedColor = new Color(0.4f, 0.7f, 1f, 1f); // tint azul
 
        private float  _value;
        private bool   _isCorrect;
        private bool   _isTutorial;
        private Button _button;
 
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(OnClicked);
 
            if (buttonImage == null)
                buttonImage = GetComponent<Image>();
 
            if (valueText == null)
                valueText = GetComponentInChildren<TextMeshProUGUI>();
 
            if (buttonImage == null)
                Debug.LogError($"[AnswerButton] buttonImage não encontrado em '{gameObject.name}'!");
            if (valueText == null)
                Debug.LogError($"[AnswerButton] valueText não encontrado em '{gameObject.name}'!");
        }
 
        // =====================================================
        /// <summary>
        /// Configura o botão.
        /// O sprite é exibido diretamente no buttonImage —
        /// defaultColor = branco deixa o sprite com cor original.
        /// </summary>
        public void Setup(float value, string unit, bool isCorrect, bool isTutorial, Sprite questionSprite = null)
        {
            _value      = value;
            _isCorrect  = isCorrect;
            _isTutorial = isTutorial;
 
            if (valueText != null)
                valueText.text = $"{value:F1} {unit}";
 
            if (buttonImage != null)
            {
                // Troca o sprite do botão conforme o tipo da questão
                if (questionSprite != null)
                {
                    buttonImage.sprite         = questionSprite;
                    buttonImage.preserveAspect = true;
                    buttonImage.type           = Image.Type.Simple;
                }
 
                // Tint: verde no tutorial para o correto, branco nos outros
                buttonImage.color = (isTutorial && isCorrect) ? correctColor : defaultColor;
            }
 
            if (_button != null)
                _button.interactable = !isTutorial;
        }
 
        private void OnClicked()
        {
            if (buttonImage != null)
                buttonImage.color = selectedColor;
 
            if (GameManager.Instance == null)
            {
                Debug.LogError("[AnswerButton] GameManager.Instance é null!");
                return;
            }
 
            GameManager.Instance.OnAnswerSelected(_value);
        }
 
        public void ResetColor()
        {
            if (buttonImage != null) buttonImage.color = defaultColor;
            if (_button != null)    _button.interactable = true;
        }
    }
}