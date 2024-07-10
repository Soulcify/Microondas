using System;

class Program
{
    static void Main()
    {
        MicrowaveController microwaveController = new MicrowaveController();

        while (true)
        {
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1. Iniciar programa pré-definido");
            Console.WriteLine("2. Registrar novo programa customizado");
            Console.WriteLine("3. Sair");

            Console.Write("Opção: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    // Opção de iniciar programa pré-definido
                    Console.WriteLine("\nEscolha um programa pré-definido para iniciar:");
                    for (int i = 0; i < microwaveController.PredefinedPrograms.Count; i++)
                    {
                        var program = microwaveController.PredefinedPrograms[i];
                        Console.WriteLine($"{i}: {program.Name} - {program.Food} - {program.Time.TotalMinutes} minutos - Potência {program.Power}");
                    }

                    Console.Write("Digite o número do programa: ");
                    int programIndex = int.Parse(Console.ReadLine());

                    microwaveController.StartPredefinedProgram(programIndex);
                    break;

                case "2":
                    // Opção de registrar novo programa customizado
                    Console.WriteLine("\nRegistrar novo programa customizado:");

                    Console.Write("Nome do programa: ");
                    string name = Console.ReadLine();

                    Console.Write("Alimento: ");
                    string food = Console.ReadLine();

                    Console.Write("Tempo (em minutos): ");
                    int minutes = int.Parse(Console.ReadLine());
                    TimeSpan time = TimeSpan.FromMinutes(minutes);

                    Console.Write("Potência: ");
                    int power = int.Parse(Console.ReadLine());

                    Console.Write("Caractere de aquecimento: ");
                    string heatingString = Console.ReadLine();

                    microwaveController.RegisterCustomProgram(name, food, time, power, heatingString);
                    break;

                case "3":
                    // Opção de sair do programa
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
            Console.Clear(); // Limpa o console para a próxima iteração do loop
        }
    }
}
