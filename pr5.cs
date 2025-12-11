using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ======== КЛАС ВЕЛОСИПЕД ========
class Bicycle : ICloneable, IComparable<Bicycle>
{
    public string Model { get; set; }
    public int Speed { get; set; }

    public Bicycle(string model, int speed)
    {
        Model = model;
        Speed = speed;
    }

    public override string ToString()
    {
        return $"{Model} — {Speed} км/год";
    }

    public object Clone()
    {
        return new Bicycle(Model, Speed);
    }

    public int CompareTo(Bicycle other)
    {
        return Speed.CompareTo(other.Speed);
    }
}

// ======== КОЛЕКЦІЯ З ВЛАСНИМ ІТЕРАТОРОМ ========
class BicycleGarage
{
    private List<Bicycle> bicycles = new List<Bicycle>();
    private Mutex mutex = new Mutex(); // синхронізація потоків

    public void Add(Bicycle b)
    {
        // блокування, щоб інші потоки не втрутились
        mutex.WaitOne();
        bicycles.Add(b);
        mutex.ReleaseMutex();
    }

    public IEnumerable<Bicycle> All()
    {
        return bicycles;
    }

    public IEnumerable<Bicycle> FasterThan(int minSpeed)
    {
        foreach (var b in bicycles)
        {
            if (b.Speed > minSpeed)
                yield return b;
        }
    }

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

    // Потокобезпечне збільшення швидкості
    public void IncreaseSpeedSafe(int value)
    {
        mutex.WaitOne();
        foreach (var b in bicycles)
        {
            b.Speed += value;
        }
        mutex.ReleaseMutex();
    }
}

// ======== ПРОГРАМА ========
class Program
{
    static void Main()
    {
        BicycleGarage garage = new BicycleGarage();

        garage.Add(new Bicycle("Giant", 35));
        garage.Add(new Bicycle("Trek", 42));
        garage.Add(new Bicycle("Cube", 28));
        garage.Add(new Bicycle("Scott", 40));

        // ======== ДЕЛЕГАТИ ========
        Console.WriteLine("=== Вивід (делегат Action) ===");
        garage.ProcessAll(b => Console.WriteLine(b));

        // ======== ФІЛЬТРАЦІЯ ========
        Console.WriteLine("\n=== Фільтрація — Speed > 35 ===");
        foreach (var b in garage.Filter(b => b.Speed > 35))
            Console.WriteLine(b);

        // ======== КЛОНУВАННЯ ========
        Console.WriteLine("\n=== Клонування ===");
        var original = new Bicycle("Author", 33);
        var copy = (Bicycle)original.Clone();
        Console.WriteLine("Оригінал: " + original);
        Console.WriteLine("Копія: " + copy);

        // ======== СОРТУВАННЯ ========
        Console.WriteLine("\n=== Сортування ===");
        var sorted = garage.All().OrderBy(b => b).ToList();
        foreach (var b in sorted)
            Console.WriteLine(b);

        // ======== ІТЕРАТОР (YIELD) ========
        Console.WriteLine("\n=== Власний ітератор — Speed > 30 ===");
        foreach (var b in garage.FasterThan(30))
            Console.WriteLine(b);

        // ======== LINQ ========
        Console.WriteLine("\n=== LINQ — середня швидкість ===");
        double avg = garage.All().Average(b => b.Speed);
        Console.WriteLine("Середня швидкість: " + avg);

        // ======== ПАРАЛЕЛЬНІ ПОТОКИ ========
        Console.WriteLine("\n=== Багатопоточність ===");

        Task t1 = Task.Run(() =>
        {
            Console.WriteLine("Потік 1: збільшуємо швидкість на 5...");
            garage.IncreaseSpeedSafe(5);
        });

        Task t2 = Task.Run(() =>
        {
            Console.WriteLine("Потік 2: збільшуємо швидкість на 10...");
            garage.IncreaseSpeedSafe(10);
        });

        Task t3 = Task.Run(() =>
        {
            Console.WriteLine("Потік 3: виводимо всі моделі...");
            garage.ProcessAll(b => Console.WriteLine(">> " + b.Model));
        });

        Task.WaitAll(t1, t2, t3);

        Console.WriteLine("\n=== Після змін швидкості потоками ===");
        garage.ProcessAll(b => Console.WriteLine(b));

        Console.WriteLine("\nГотово.");
    }
}
