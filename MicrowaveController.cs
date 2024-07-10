using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

public class MicrowaveController
{
    private List<PredefinedProgram> _predefinedPrograms;
    private List<CustomProgram> _customPrograms;
    private Timer _heatingTimer;
    private TimeSpan _remainingTime;
    private bool _isHeating;

    // Evento para notificar mudanças de status
    public event Action<string> OnMicrowaveStatusChanged;

    // Construtor para inicialização dos programas pré-definidos
    public MicrowaveController()
    {
        InitializePredefinedPrograms();
        _customPrograms = new List<CustomProgram>();
        LoadCustomPrograms();
    }

    // Propriedade para obter programas pré-definidos
    public List<PredefinedProgram> PredefinedPrograms => _predefinedPrograms;

    // Propriedade para obter programas customizados
    public List<CustomProgram> CustomPrograms => _customPrograms;

    // Estrutura para representar um programa pré-definido
    public class PredefinedProgram
    {
        public string Name { get; }
        public string Food { get; }
        public TimeSpan Time { get; }
        public int Power { get; }
        public string HeatingString { get; }
        public string Instructions { get; }

        public PredefinedProgram(string name, string food, TimeSpan time, int power, string heatingString, string instructions)
        {
            Name = name;
            Food = food;
            Time = time;
            Power = power;
            HeatingString = heatingString;
            Instructions = instructions;
        }
    }

    // Estrutura para representar um programa customizado
    public class CustomProgram
    {
        public string Name { get; }
        public string Food { get; }
        public TimeSpan Time { get; }
        public int Power { get; }
        public string HeatingString { get; }
        public string Instructions { get; }

        public CustomProgram(string name, string food, TimeSpan time, int power, string heatingString, string instructions)
        {
            Name = name;
            Food = food;
            Time = time;
            Power = power;
            HeatingString = heatingString;
            Instructions = instructions;
        }
    }

    // Método para inicializar os programas pré-definidos
    private void InitializePredefinedPrograms()
    {
        _predefinedPrograms = new List<PredefinedProgram>
        {
            new PredefinedProgram(
                "Pipoca",
                "Pipoca (de micro-ondas)",
                TimeSpan.FromMinutes(3),
                7,
                "!!! Aquecimento de Pipoca !!!",
                "Observar o barulho de estouros do milho, interromper se houver intervalo de mais de 10 segundos entre estouros."
            ),
            new PredefinedProgram(
                "Leite",
                "Leite",
                TimeSpan.FromMinutes(5),
                5,
                "*** Aquecimento de Leite ***",
                "Cuidado com aquecimento de líquidos, risco de fervura imediata."
            ),
            new PredefinedProgram(
                "Carnes de boi",
                "Carne em pedaço ou fatias",
                TimeSpan.FromMinutes(14),
                4,
                "### Aquecimento de Carnes ###",
                "Interromper na metade e virar para descongelamento uniforme."
            ),
            new PredefinedProgram(
                "Frango",
                "Frango (qualquer corte)",
                TimeSpan.FromMinutes(8),
                7,
                "@@@ Aquecimento de Frango @@@",
                "Interromper na metade e virar para descongelamento uniforme."
            ),
            new PredefinedProgram(
                "Feijão",
                "Feijão congelado",
                TimeSpan.FromMinutes(8),
                9,
                "^^^ Aquecimento de Feijão ^^^",
                "Deixe o recipiente destampado para evitar danos."
            )
        };
    }

    // Método para iniciar um programa pré-definido ou customizado
    public void StartPredefinedProgram(int programIndex, bool isCustom = false)
    {
        if (isCustom)
        {
            if (programIndex < 0 || programIndex >= _customPrograms.Count)
            {
                ShowMessage("Programa customizado não encontrado.");
                return;
            }

            var program = _customPrograms[programIndex];
            StartHeating(program.HeatingString, program.Time, program.Power);
        }
        else
        {
            if (programIndex < 0 || programIndex >= _predefinedPrograms.Count)
            {
                ShowMessage("Programa pré-definido não encontrado.");
                return;
            }

            var program = _predefinedPrograms[programIndex];
            StartHeating(program.HeatingString, program.Time, program.Power);
        }
    }

    // Método para iniciar o aquecimento com tempo e potência específicos
    public void StartMicrowave(TimeSpan time, int power)
    {
        ShowMessage("Não é possível definir tempo e potência para programas pré-definidos.");
    }

    // Método para pausar o aquecimento
    public void PauseMicrowave()
    {
        if (_isHeating)
        {
            _heatingTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _isHeating = false;
            ShowMessage("Aquecimento pausado.");
        }
    }

    // Método para cancelar o aquecimento
    public void CancelMicrowave()
    {
        if (_isHeating)
        {
            _heatingTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _isHeating = false;
            ShowMessage("Aquecimento cancelado.");
        }
    }

    // Método para adicionar tempo durante o aquecimento
    public void AddTime(TimeSpan additionalTime)
    {
        if (_isHeating)
        {
            _remainingTime += additionalTime;
            ShowMessage($"Tempo adicional de {additionalTime.TotalMinutes} minutos adicionado.");
        }
    }

    // Método para iniciar o aquecimento
    private void StartHeating(string heatingString, TimeSpan time, int power)
    {
        if (_isHeating)
        {
            ShowMessage("O microondas já está aquecendo.");
            return;
        }

        ShowMessage(heatingString);
        Console.WriteLine($"Tempo: {time.TotalMinutes} minutos");
        Console.WriteLine($"Potência: {power}");

        _remainingTime = time;
        _isHeating = true;

        _heatingTimer = new Timer((state) =>
        {
            if (_remainingTime > TimeSpan.Zero)
            {
                _remainingTime -= TimeSpan.FromSeconds(1);
                Console.WriteLine("Aquecimento em andamento...");
            }
            else
            {
                _heatingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _isHeating = false;
                ShowMessage("Aquecimento concluído.");
            }
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    // Método para exibir mensagens na interface
    private void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }

    // Método para cadastrar novos programas customizados
    public void RegisterCustomProgram(string name, string food, TimeSpan time, int power, string heatingString, string instructions = "")
    {
        // Verifica se o caractere de aquecimento é único
        if (!IsHeatingCharacterValid(heatingString))
        {
            ShowMessage("Caractere de aquecimento inválido ou já utilizado.");
            return;
        }

        // Cria o programa customizado
        var newProgram = new CustomProgram(name, food, time, power, heatingString, instructions);
        _customPrograms.Add(newProgram);

        // Mostra mensagem de sucesso
        ShowMessage($"Programa customizado '{name}' cadastrado com sucesso.");
    }

    // Método para verificar se o caractere de aquecimento é único
    private bool IsHeatingCharacterValid(string heatingString)
    {
        // Verifica se o caractere de aquecimento é único
        foreach (var program in _predefinedPrograms)
        {
            if (program.HeatingString == heatingString)
            {
                return false;
            }
        }

        foreach (var program in _customPrograms)
        {
            if (program.HeatingString == heatingString)
            {
                return false;
            }
        }

        // Verifica se o caractere de aquecimento não é o padrão '.'
        if (heatingString == ".")
        {
            return false;
        }

        return true;
    }

    // Método para carregar programas customizados a partir de um arquivo JSON
    private void LoadCustomPrograms()
    {
        try
        {
            string jsonFilePath = "custom_programs.json";

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                _customPrograms = JsonSerializer.Deserialize<List<CustomProgram>>(jsonContent);
                ShowMessage("Programas customizados carregados com sucesso.");
            }
            else
            {
                ShowMessage("Arquivo de programas customizados não encontrado. Será criado um novo ao salvar.");
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Erro ao carregar programas customizados: {ex.Message}");
        }
    }

    // Método para salvar programas customizados em um arquivo JSON
    private void SaveCustomPrograms()
    {
        try
        {
            string jsonFilePath = "custom_programs.json";
            string jsonContent = JsonSerializer.Serialize(_customPrograms, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonFilePath, jsonContent);
            ShowMessage("Programas customizados salvos com sucesso.");
        }
        catch (Exception ex)
        {
            ShowMessage($"Erro ao salvar programas customizados: {ex.Message}");
        }
    }
}
