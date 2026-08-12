using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
namespace CircuitGame.Circuit
{
    public class CircuitElement : MonoBehaviour
    {
        [Header("Referências internas do prefab")]
        [SerializeField] private Image           icon;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI nameLabel;
 
        [Header("Cores")]
        [SerializeField] private Color knownColor   = Color.white;
        [SerializeField] private Color unknownColor = new Color(1f, 0.85f, 0f); // amarelo
 
        // =====================================================
        /// <summary>
        /// Atualiza o componente visual.
        /// </summary>
        /// <param name="sprite">Sprite do componente (bateria, resistor...)</param>
        /// <param name="label">Nome: "Tensão", "Resistência", "Corrente"</param>
        /// <param name="value">Valor formatado: "9.0 V", "15 Ω", "0.6 A"</param>
        /// <param name="isUnknown">Se true, exibe "?" em vez do valor</param>
        public void Setup(Sprite sprite, string label, string value, bool isUnknown)
        {
            // Sprite do ícone
            if (icon != null)
            {
                icon.sprite          = sprite;
                icon.preserveAspect  = true;
                icon.enabled         = sprite != null;
            }
 
            // Texto do nome (ex: "Resistência")
            if (nameLabel != null)
                nameLabel.text = label;
 
            // Valor ou "?"
            if (valueText != null)
            {
                if (isUnknown)
                {
                    valueText.text     = "?";
                    valueText.color    = unknownColor;
                    valueText.fontSize = 40f;   // grande para chamar atenção
                    valueText.fontStyle = FontStyles.Bold;
                }
                else
                {
                    valueText.text      = value;
                    valueText.color     = knownColor;
                    valueText.fontSize  = 24f;
                    valueText.fontStyle = FontStyles.Normal;
                }
            }
        }
 
        /// <summary>
        /// Anima um pulso suave no componente desconhecido (opcional).
        /// Chame StartCoroutine(PulseUnknown()) se quiser o efeito.
        /// </summary>
        public System.Collections.IEnumerator PulseUnknown()
        {
            if (valueText == null) yield break;
 
            float elapsed = 0f;
            float duration = 0.8f;
            Color baseColor = new Color(1f, 0.85f, 0f);
            Color peakColor = Color.white;
 
            while (true) // loop contínuo — pare chamando StopCoroutine
            {
                elapsed += Time.deltaTime;
                float t = Mathf.PingPong(elapsed / duration, 1f);
                valueText.color = Color.Lerp(baseColor, peakColor, t);
                yield return null;
            }
        }
    }
}