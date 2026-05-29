using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CircuitGame.Circuit;
using CircuitGame.Core;
 
namespace CircuitGame
{
    public class UIManager : MonoBehaviour
    {
        [Header("── Painel de Fase ──────────────────")]
        [SerializeField] private TextMeshProUGUI phaseText;
        [SerializeField] private TextMeshProUGUI scoreText;
 
        [Header("── Painel da Questão ──────────────")]
        [SerializeField] private TextMeshProUGUI questionTitleText;
        [SerializeField] private TextMeshProUGUI givenValue1Text;
        [SerializeField] private TextMeshProUGUI givenValue2Text;
        /// <summary>
        /// [SerializeField] private TextMeshProUGUI formulaText;
        /// </summary>
 
        [Header("── Botões de Resposta (arraste os 4) ──")]
        [SerializeField] private AnswerButton[] answerButtons;
 
        [Header("── Tutorial ────────────────────────")]
        [SerializeField] private GameObject      tutorialPanel;
        [SerializeField] private TextMeshProUGUI tutorialText;
        // Botão "Começar →" dentro do TutorialPanel
        [SerializeField] private Button          tutorialNextButton;
 
        [Header("── Painel de Feedback (fases intermediárias) ──")]
        [SerializeField] private GameObject      feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackTitleText;
        [SerializeField] private TextMeshProUGUI feedbackDetailText;
        // Apenas o botão Próxima Fase — Retry foi removido
        [SerializeField] private Button          nextButton;
 
        [Header("── Tela de Conclusão ───────────────")]
        [SerializeField] private GameObject      completePanel;
        [SerializeField] private TextMeshProUGUI completeText;
        [SerializeField] private Button          restartButton;   // único botão de ação
 
        [Header("── Cores ───────────────────────────")]
        [SerializeField] private Color correctColor   = new Color(0.2f, 0.8f, 0.3f);
        [SerializeField] private Color incorrectColor = new Color(0.9f, 0.2f, 0.2f);
 
        // =====================================================
        private void Awake()
        {
            SetActive(feedbackPanel, false);
            SetActive(tutorialPanel, false);
            SetActive(completePanel, false);
 
            tutorialNextButton?.onClick.AddListener(OnNextClicked);
            nextButton?.onClick.AddListener(OnNextClicked);
            restartButton?.onClick.AddListener(OnRestartClicked);
 
            ValidateReferences();
        }
 
        // =====================================================
        public void DisplayQuestion(CircuitQuestion question, bool isTutorial)
        {
            SetActive(feedbackPanel, false);
            SetActive(completePanel, false);
 
            int phase = GameManager.Instance.CurrentPhase;
            int total = GameManager.Instance.TotalPhases;
            int score = GameManager.Instance.Score;
 
            // Tutorial → "Tutorial" | Fase real → "Fase 1 / 5"
            SetText(phaseText, isTutorial
                ? "0/0"
                : $"{phase}/{total}");
 
            SetText(scoreText,         $"Score: {score}");
            SetText(questionTitleText,  question.QuestionTitle);
            SetText(givenValue1Text,    question.GivenValue1);
            SetText(givenValue2Text,    question.GivenValue2);
            //SetText(formulaText,        question.Formula);
 
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i] == null) continue;
                float value     = question.Options[i];
                bool  isCorrect = Mathf.Abs(value - question.CorrectAnswer) < 0.001f;
                answerButtons[i].Setup(value, question.Unit, isCorrect, isTutorial);
            }
 
            if (isTutorial)
            {
                SetActive(tutorialPanel, true);
                SetText(tutorialText,
                    $"<b>Exemplo resolvido:</b>\n\n" +
                    $"Fórmula: <color=#FFD700>{question.Formula}</color>\n\n" +
                    $"{question.Solution}\n\n" +
                    $"<i>O botão verde é a resposta correta.\n" +
                    $"Nas próximas fases você responderá sozinho!</i>");
            }
            else
            {
                SetActive(tutorialPanel, false);
            }
        }
 
        // =====================================================
        /// <summary>
        /// Chamado só nas fases intermediárias (não na última).
        /// </summary>
        public void ShowFeedback(bool isCorrect, CircuitQuestion question)
        {
            SetActive(feedbackPanel, true);
 
            if (isCorrect)
            {
                if (feedbackTitleText != null)
                {
                    feedbackTitleText.text  = "✅ Correto!";
                    feedbackTitleText.color = correctColor;
                }
                SetText(feedbackDetailText,
                    $"Muito bem!\n\n" +
                    $"<b>{question.Formula}</b>\n" +
                    $"<color=#FFD700>{question.Solution}</color>");
            }
            else
            {
                if (feedbackTitleText != null)
                {
                    feedbackTitleText.text  = "❌ Resposta incorreta";
                    feedbackTitleText.color = incorrectColor;
                }
                SetText(feedbackDetailText,
                    $"A resposta certa era <b>{question.CorrectAnswer} {question.Unit}</b>\n\n" +
                    $"<b>{question.Formula}</b>\n" +
                    $"<color=#FFD700>{question.Solution}</color>");
            }
 
            // Botão "Próxima Fase" sempre visível (retry não existe mais)
            if (nextButton != null)
                nextButton.gameObject.SetActive(true);
        }
 
        // =====================================================
        /// <summary>
        /// Tela final — aparece após a última fase (acerto ou erro).
        /// </summary>
        public void ShowGameComplete(int score, int total)
        {
            SetActive(feedbackPanel, false);
            SetActive(tutorialPanel, false);
            SetActive(completePanel, true);
 
            string performance = score switch
            {
                _ when score == total        => "🏆 Perfeito! Você domina a Lei de Ohm!",
                _ when score >= total * 0.7f => "🎯 Muito bem! Continue praticando.",
                _ when score >= total * 0.4f => "📚 Quase lá! Revise as fórmulas.",
                _                            => "🔁 Continue estudando. Tente novamente!"
            };
 
            SetText(completeText,
                $"Jogo Concluído!\n\n" +
                $"Score: <b>{score} / {total}</b>\n\n" +
                $"{performance}\n\n" +
                $"<size=70%>Clique em Reiniciar para jogar novamente\n" +
                $"(o Tutorial aparecerá novamente)</size>");
        }
 
        // =====================================================
        private void SetActive(GameObject obj, bool active)
        {
            if (obj != null) obj.SetActive(active);
        }
 
        private void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text;
            else Debug.LogWarning($"[UIManager] TMP não atribuído — texto: \"{text}\"");
        }
 
        private void OnNextClicked()    => GameManager.Instance?.NextPhase();
        private void OnRestartClicked() => GameManager.Instance?.RestartGame();
 
        private void ValidateReferences()
        {
            if (tutorialNextButton == null)
                Debug.LogWarning("[UIManager] ⚠ tutorialNextButton não atribuído!");
            if (nextButton    == null) Debug.LogWarning("[UIManager] ⚠ nextButton não atribuído.");
            if (feedbackPanel == null) Debug.LogWarning("[UIManager] ⚠ feedbackPanel não atribuído.");
            if (completePanel == null) Debug.LogWarning("[UIManager] ⚠ completePanel não atribuído.");
            if (restartButton == null) Debug.LogWarning("[UIManager] ⚠ restartButton não atribuído.");
        }
    }
}