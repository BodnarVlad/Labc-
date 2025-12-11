using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        BicycleGarage garage = new BicycleGarage();

        garage.Add(new Bicycle("Giant", 35));
        garage.Add(new Bicycle("Trek", 42));
        garage.Add(new Bicycle("Cube", 28));

        Console.WriteLine("=== Асинхронна обробка даних ===");

        CancellationTokenSource cts = new CancellationTokenSource();

        // Створюємо кілька задач
        var task1 = LoadGarageAsync(garage, cts.Token);
        var task2 = CalculateAverageSpeedAsync(garage, cts.Token);
        var task3 = SaveReportAsync(garage, cts.Token);

        // Можемо скасувати через 2 секунди
        Task.Run(async () =>
        {
            await Task.Delay(2000);
            cts.Cancel();
            Console.WriteLine("\n>>> Виконання завдань скасовано! <<<");
        });

        try
        {
            await Task.WhenAll(task1, task2, task3);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nОдна або більше асинхронних операцій були скасовані.");
        }

        Console.WriteLine("\n=== Готово ===");
    }

    // --------------------------------------------------
    // Асинхронні методи
    // --------------------------------------------------

    // Імітація довгої операції (наприклад — завантаження даних із БД)
    static async Task LoadGarageAsync(BicycleGarage garage, CancellationToken token)
    {
        Console.WriteLine("\n[1] Завантаження даних...");
        await Task.Delay(3000, token);
        Console.WriteLine("[1] Дані гаража завантажено.");
    }

    // Асинхронне обчислення середньої швидкості
    static async Task CalculateAverageSpeedAsync(BicycleGarage garage, CancellationToken token)
    {
        Console.WriteLine("\n[2] Обчислення середньої швидкості...");
        await Task.Delay(1500, token);

        double avg = garage.All().Average(b => b.Speed);
        Console.WriteLine($"[2] Середня швидкість: {avg}");
    }

    // Асинхронне "збереження" у файл (імітація)
    static async Task SaveReportAsync(BicycleGarage garage, CancellationToken token)
    {
        Console.WriteLine("\n[3] Збереження звіту...");
        await Task.Delay(2500, token);

        Console.WriteLine("[3] Звіт збережено!");
    }
}
