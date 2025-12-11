using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ======== СТАНИ ВЕЛОСИПЕДА ========
enum BikeState
{
    Idle,
    Riding,
    Servicing
}

// ======== КЛАС ВЕЛОСИПЕД ========
class Bicycle : ICloneable, IComparable<Bicycle>
{
    public string Model { get; set; }
    public int Speed { get; set; }
    public BikeState State { get; private set; } = BikeState.Idle;

    public Bicycle(string model, int speed)
    {
        Model = model;
        Speed = speed;
    }

    public override string ToString()
    {
        return $"{Model} — {Speed} км/год — Стан: {State}";
    }

    // --- ICloneable ---
    public object Clone()
    {
        return new Bicycle(Model, Speed);
    }

    // --- IComparable ---
    public int CompareTo(Bicycle other)
    {
        return Speed.CompareTo(other.Speed);
    }

    // --- STATE MACHINE ---
    public async Task StartRidingAsync()
    {
        if (State != BikeState.Idle)
        {
            Console.WriteLine($"{Model}: не можна почати їзду зі стану {State}");
            return;
        }
        Console.WriteLine($"{Model}: починаємо їзду...");
        State = BikeState.Riding;
        await Task.Delay(1000);
        Console.WriteLine($"{Model}: зараз у стані {State}");
    }

    public async Task StopAsync()
    {
        if (State != BikeState.Riding)
        {
            Console.WriteLine($"{Model}: не можна зупинитись із стану {State}");
            return;
        }
        Console.WriteLine($"{Model}: зупиняємось...");
        await Task.Delay(500);
        State = BikeState.Idle;
        Console.WriteLine($"{Model}: зараз у стані {State}");
    }

    public async Task ServiceAsync()
    {
        if (State == BikeState.Servicing)
        {
            Console.WriteLine($"{Model}: вже в обслуговуванні");
            return;
        }
        Console.WriteLine($"{Model}: починаємо обслуговування...");
        State = BikeState.Servicing;
        await Task.Delay(1500);
        State = BikeState.Idle;
        Console.WriteLine($"{Model}: обслуговування завершено, стан {State}");
    }
}

// ======== КОЛЕКЦІЯ ВЕЛОСИПЕДІВ ========
class BicycleGarage
{
    private List<Bicycle> bicycles = new List<Bicycle>();
    private Mutex mutex = new Mutex();

    public void Add(Bicycle b)
    {
        mutex.WaitOne();
        bicycles.Add(b);
        mutex.ReleaseMutex();
    }

    public IEnumerable<Bicycle> All() => bicycles;

    // Потокобезпечне збільшення швидкості
    public void IncreaseSpeedSafe(int value)
    {
        mutex.WaitOne();
        foreach (var b in bicycles)
            b.Speed += value;
        mutex.ReleaseMutex();
    }

    // Yield-ітератор
    public IEnumerable<Bicycle> FasterThan(int minSpeed)
    {
        foreach (var b in bicycles)
            if (b.Speed > minSpeed)
                yield return b;
    }

    // Делегати
    public void ProcessAll(Action<Bicycle> action)
    {
        foreach (var b in bicycles)
            action(b);
    }

    public IEnumerable<Bicycle> Filter(Func<Bicycle, bool> check)
    {
        foreach (var b in bicycles)
            if (check(b))
                yield return b;
    }
}

// ======== ПРОГРАМА ========
class Program
{
    static async Task Main()
    {
        BicycleGarage garage = new BicycleGarage();

        garage.Add(new Bicycle("Giant", 35));
        garage.Add(new Bicycle("Trek", 42));
        garage.Add(new Bicycle("Cube", 28));
        garage.Add(new Bicycle("Scott", 40));

        Console.WriteLine("=== Делегати ===");
        garage.ProcessAll(b => Console.WriteLine(b));

        Console.WriteLine("\n=== Yield-ітератор (Speed > 35) ===");
        foreach (var b in garage.FasterThan(35))
            Console.WriteLine(b);

        Console.WriteLine("\n=== Клонування ===");
        var original = new Bicycle("Author", 33);
        var copy = (Bicycle)original.Clone();
        Console.WriteLine("Оригінал: " + original);
        Console.WriteLine("Копія: " + copy);

        Console.WriteLine("\n=== Сортування (IComparable) ===");
        var sorted = garage.All().OrderBy(b => b).ToList();
        foreach (var b in sorted)
            Console.WriteLine(b);

        Console.WriteLine("\n=== LINQ ===");
        double avg = garage.All().Average(b => b.Speed);
        Console.WriteLine("Середня швидкість: " + avg);

        var fastModels = garage.All().Where(b => b.Speed > 35).Select(b => b.Model);
        Console.WriteLine("Моделі зі швидкістю > 35:");
        foreach (var m in fastModels)
            Console.WriteLine(m);

        Console.WriteLine("\n=== Багатопоточність (Task + Mutex) ===");
        Task t1 = Task.Run(() => garage.IncreaseSpeedSafe(5));
        Task t2 = Task.Run(() => garage.IncreaseSpeedSafe(10));
        Task.WaitAll(t1, t2);
        garage.ProcessAll(b => Console.WriteLine(b));

        Console.WriteLine("\n=== Асинхронні методи + CancellationToken ===");
        CancellationTokenSource cts = new CancellationTokenSource();

        var loadTask = LoadGarageAsync(garage, cts.Token);
        var avgTask = CalculateAverageSpeedAsync(garage, cts.Token);

        Task.Run(async () =>
        {
            await Task.Delay(2000);
            cts.Cancel();
            Console.WriteLine("\n>>> Скасування асинхронних задач <<<");
        });

        try
        {
            await Task.WhenAll(loadTask, avgTask);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Одна або більше асинхронних задач були скасовані.");
        }

        Console.WriteLine("\n=== State Machine ===");
        Bicycle bike = new Bicycle("SpecialBike", 38);
        await bike.StartRidingAsync();
        await bike.ServiceAsync();
        await bike.StopAsync();
        await bike.ServiceAsync();

        Console.WriteLine("\n=== Фінальний стан велосипеда ===");
        Console.WriteLine(bike);
    }

    // --- Асинхронні методи ---
    static async Task LoadGarageAsync(BicycleGarage garage, CancellationToken token)
    {
        Console.WriteLine("Завантаження гаража...");
        await Task.Delay(3000, token);
        Console.WriteLine("Гараж завантажено.");
    }

    static async Task CalculateAverageSpeedAsync(BicycleGarage garage, CancellationToken token)
    {
        Console.WriteLine("Обчислення середньої швидкості...");
        await Task.Delay(1500, token);
        double avg = garage.All().Average(b => b.Speed);
        Console.WriteLine($"Середня швидкість: {avg}");
    }
}
