using System.Collections.Generic;
using UnityEngine;
using CircuitGame.Circuit;
 
namespace CircuitGame.Circuit
{
    public class QuestionGenerator : MonoBehaviour
    {
        // ---- Configurações editáveis no Inspector ----
 
        [Header("Tensão — limites do circuito (V)")]
        [SerializeField] private float voltageMin =  5f;   // tensão mínima gerada
        [SerializeField] private float voltageMax = 100f;  // tensão máxima gerada
 
        [Header("Corrente (A)")]
        [SerializeField] private float currentMin = 0.5f;
        [SerializeField] private float currentMax = 5.0f;
 
        [Header("Casas decimais nas opções")]
        [SerializeField] private int decimalPlaces = 1;
 
        // Valores de resistência usados (Ω) — faixa menor para tensões menores
        private readonly float[] _niceResistances =
            { 10f, 15f, 22f, 33f, 47f, 68f, 100f };
 
        // =====================================================
        /// <summary>
        /// Gera uma questão aleatória garantindo que V fique
        /// entre voltageMin e voltageMax.
        /// </summary>
        public CircuitQuestion GenerateQuestion()
        {
            // 1. Sorteia R
            float R = _niceResistances[Random.Range(0, _niceResistances.Length)];
 
            // 2. Calcula o range de I que mantém V dentro dos limites
            //    V = R × I  →  I = V / R
            float iMinAllowed = Mathf.Max(currentMin, voltageMin / R);
            float iMaxAllowed = Mathf.Min(currentMax, voltageMax / R);
 
            // Segurança: se os limites forem inválidos (ex: resistência muito alta),
            // usa a resistência mínima da lista
            if (iMinAllowed >= iMaxAllowed)
            {
                R            = _niceResistances[0];   // 10Ω — menor da lista
                iMinAllowed  = Mathf.Max(currentMin, voltageMin / R);
                iMaxAllowed  = Mathf.Min(currentMax, voltageMax / R);
            }
 
            // 3. Sorteia I dentro do range seguro
            float I = RoundToDecimals(Random.Range(iMinAllowed, iMaxAllowed), decimalPlaces);
            float V = RoundToDecimals(R * I, decimalPlaces);
 
            // 4. Sorteia tipo de questão
            var type = (QuestionType)Random.Range(0, 3);
 
            var question = new CircuitQuestion
            {
                Voltage    = V,
                Current    = I,
                Resistance = R,
                Type       = type
            };
 
            switch (type)
            {
                case QuestionType.FindVoltage:
                    question.CorrectAnswer = V;
                    question.Unit          = "V";
                    question.QuestionTitle = "V = ?";
                    question.GivenValue1   = $"R = {R} Ω";
                    question.GivenValue2   = $"I = {I} A";
                    question.Formula       = "V = R × I";
                    question.Solution      = $"V = {R} × {I} = {V} V";
                    break;
 
                case QuestionType.FindCurrent:
                    question.CorrectAnswer = I;
                    question.Unit          = "A";
                    question.QuestionTitle = "I = ?";
                    question.GivenValue1   = $"V = {V} V";
                    question.GivenValue2   = $"R = {R} Ω";
                    question.Formula       = "I = V ÷ R";
                    question.Solution      = $"I = {V} ÷ {R} = {I} A";
                    break;
 
                case QuestionType.FindResistance:
                    question.CorrectAnswer = R;
                    question.Unit          = "Ω";
                    question.QuestionTitle = "R = ?";
                    question.GivenValue1   = $"V = {V} V";
                    question.GivenValue2   = $"I = {I} A";
                    question.Formula       = "R = V ÷ I";
                    question.Solution      = $"R = {V} ÷ {I} = {R} Ω";
                    break;
            }
 
            question.Options = GenerateOptions(question.CorrectAnswer, question.Unit);
            return question;
        }
 
        // =====================================================
        private List<float> GenerateOptions(float correct, string unit)
        {
            var options = new List<float> { correct };
 
            float[] factors = { 0.5f, 2f, 3f, 0.25f, 1.5f, 4f };
            Shuffle(factors);
 
            foreach (float factor in factors)
            {
                if (options.Count >= 4) break;
                float d = RoundToDecimals(correct * factor, decimalPlaces);
                if (d > 0f && !options.Contains(d))
                    options.Add(d);
            }
 
            int attempt = 0;
            while (options.Count < 4 && attempt < 20)
            {
                attempt++;
                float variation = RoundToDecimals(
                    correct + correct * Random.Range(0.1f, 0.9f) * (Random.value > 0.5f ? 1 : -1),
                    decimalPlaces);
                if (variation > 0f && !options.Contains(variation))
                    options.Add(variation);
            }
 
            Shuffle(options);
            return options;
        }
 
        // =====================================================
        private float RoundToDecimals(float value, int decimals)
        {
            float mult = Mathf.Pow(10f, decimals);
            return Mathf.Round(value * mult) / mult;
        }
 
        private void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}