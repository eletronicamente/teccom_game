using UnityEngine;
using CircuitGame.Circuit;
 
namespace CircuitGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
 
        [Header("Configurações do Jogo")]
        [Tooltip("Quantidade de fases REAIS (não conta o tutorial)")]
        [SerializeField] private int totalPhases = 5;
 
        [Header("Referências")]
        [SerializeField] private UIManager        uiManager;
        [SerializeField] private QuestionGenerator questionGenerator;
 
        // ---- Estado interno ----
        // _currentPhase = 0  → tutorial
        // _currentPhase = 1  → fase real 1
        // _currentPhase = N  → fase real N
        private int             _currentPhase;
        private int             _score;
        private CircuitQuestion _currentQuestion;
        private bool            _answered;
 
        // ---- Propriedades públicas ----
        public int             TotalPhases     => totalPhases;
        public int             Score           => _score;
        public int             CurrentPhase    => _currentPhase;   // 0 = tutorial
        public bool            IsTutorial      => _currentPhase == 0;
 
        // =====================================================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
 
        private void Start() => LoadTutorial();
 
        // =====================================================
 
        /// <summary>Carrega o tutorial (fase 0 — não contada).</summary>
        public void LoadTutorial()
        {
            _currentPhase    = 0;
            _answered        = false;
            _score           = 0;
            _currentQuestion = questionGenerator.GenerateQuestion();
 
            uiManager.DisplayQuestion(_currentQuestion, isTutorial: true);
 
            Debug.Log("[GameManager] Tutorial carregado | " +
                      $"Resposta: {_currentQuestion.CorrectAnswer} {_currentQuestion.Unit}");
        }
 
        /// <summary>Carrega uma fase real (1 … totalPhases).</summary>
        public void LoadPhase(int phase)
        {
            _currentPhase    = phase;
            _answered        = false;
            _currentQuestion = questionGenerator.GenerateQuestion();
 
            uiManager.DisplayQuestion(_currentQuestion, isTutorial: false);
 
            Debug.Log($"[GameManager] Fase {phase}/{totalPhases} carregada | " +
                      $"Resposta: {_currentQuestion.CorrectAnswer} {_currentQuestion.Unit}");
        }
 
        // =====================================================
 
        /// <summary>
        /// Chamado pelo AnswerButton ao clicar numa opção.
        /// No tutorial, ignorado. Na última fase, sempre encerra.
        /// </summary>
        public void OnAnswerSelected(float selectedValue)
        {
            if (IsTutorial) return;
            if (_answered)  return;
            _answered = true;
 
            bool isCorrect = Mathf.Abs(selectedValue - _currentQuestion.CorrectAnswer) < 0.001f;
 
            if (isCorrect)
            {
                _score++;
                Debug.Log($"[GameManager] ✅ Correto! Pontuação: {_score}");
            }
            else
            {
                Debug.Log($"[GameManager] ❌ Errado. " +
                          $"Correto: {_currentQuestion.CorrectAnswer} {_currentQuestion.Unit}");
            }
 
            // Última fase → encerra sempre, independente de acerto/erro
            if (_currentPhase >= totalPhases)
            {
                uiManager.ShowGameComplete(_score, totalPhases);
            }
            else
            {
                uiManager.ShowFeedback(isCorrect, _currentQuestion);
            }
        }
 
        /// <summary>Avança do tutorial para fase 1, ou entre fases.</summary>
        public void NextPhase()
        {
            if (IsTutorial)
                LoadPhase(1);
            else if (_currentPhase < totalPhases)
                LoadPhase(_currentPhase + 1);
            else
                uiManager.ShowGameComplete(_score, totalPhases);
        }
 
        /// <summary>Reinicia tudo desde o tutorial.</summary>
        public void RestartGame()
        {
            _score = 0;
            LoadTutorial();
        }
    }
}