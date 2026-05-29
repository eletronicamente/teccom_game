using System.Collections.Generic;

namespace CircuitGame.Circuit
{
    /// <summary>
    /// Tipo de questão: qual grandeza o jogador deve calcular.
    /// </summary>
    public enum QuestionType
    {
        FindVoltage,     // Dado R e I, calcule V
        FindCurrent,     // Dado V e R, calcule I
        FindResistance   // Dado V e I, calcule R
    }

    /// <summary>
    /// Representa uma questão completa de circuito elétrico.
    /// Contém os valores dados, a resposta correta e as opções embaralhadas.
    /// </summary>
    public class CircuitQuestion
    {
        // ---- Grandezas do circuito ----
        public float Voltage    { get; set; }   // Tensão em Volts (V)
        public float Current    { get; set; }   // Corrente em Amperes (A)
        public float Resistance { get; set; }   // Resistência em Ohms (Ω)

        // ---- Dados da questão ----
        public QuestionType Type          { get; set; }
        public float        CorrectAnswer { get; set; }
        public string       Unit          { get; set; }  // "V", "A" ou "Ω"

        // ---- Opções dos 4 botões (embaralhadas) ----
        public List<float> Options { get; set; } = new List<float>();

        // ---- Textos exibidos na tela ----
        public string QuestionTitle { get; set; }   // Ex: "Calcule a Tensão (V)"
        public string GivenValue1   { get; set; }   // Ex: "R = 47 Ω"
        public string GivenValue2   { get; set; }   // Ex: "I = 2,0 A"
        public string Formula       { get; set; }   // Ex: "V = R × I"
        public string Solution      { get; set; }   // Ex: "V = 47 × 2 = 94 V"
    }
}