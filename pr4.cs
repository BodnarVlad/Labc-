using System;
using System.Collections.Generic;
using System.Linq;

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

    // ---- ICloneable ----
    public object Clone()
    {
        return new Bicycle(Model, Speed);
    }

    // ---- IComparable ----
    public int CompareTo(Bicycle other)
    {
        return Speed.CompareTo(other.Speed);
    }
}

// ======== КОЛЕКЦІЯ З ВЛАСНИМ ІТЕРАТОРОМ ========
class BicycleGarage
{
    private List<Bicycle> bicycles = new List<Bicycle>();

    public void Add(Bicycle b) => bicycles.Add(b);

    public IEnumerable<Bicycle> All() => bicycles;

    // Ітератор: повертає тільки велосипеди із швидкістю > заданої
    public IEnumerable<Bicycle> FasterThan(int minSpeed)
    {
        foreach (var b in bicycles)
        {
            if (b.Speed > minSpeed)
                yield return b;
        }
    }

    // Метод, що приймає делегат для обробки кожного елемента
    public void ProcessAll(Action<Bicycle> action)
    {
        foreach (var b in bicycles)
            action(b);
    }

    // інший тип делегата — перевірка умови
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
    static void Main()
    {
        BicycleGarage garage = new BicycleGarage();

        garage.Add(new Bicycle("Giant", 35));
        garage.Add(new Bicycle("Trek", 42));
        garage.Add(new Bicycle("Cube", 28));
        garage.Add(new Bicycle("Scott", 40));

        Console.WriteLine("=== Вивід (делегат Action) ===");
        garage.ProcessAll(b => Console.WriteLine(b));

        Console.WriteLine();
        Console.WriteLine("=== Фільтрація (делегат Func) — швидкість > 35 ===");
        foreach (var b in garage.Filter(b => b.Speed > 35))
            Console.WriteLine(b);

        Console.WriteLine();
        Console.WriteLine("=== Клонування ===");
        var original = new Bicycle("Author", 33);
        var copy = (Bicycle)original.Clone();
        Console.WriteLine("Оригінал: " + original);
        Console.WriteLine("Копія: " + copy);

        Console.WriteLine();
        Console.WriteLine("=== Сортування (IComparable) ===");
        var sorted = garage.All().OrderBy(b => b).ToList();
        foreach (var b in sorted)
            Console.WriteLine(b);

        Console.WriteLine();
        Console.WriteLine("=== Власний ітератор (yield) — швидше 30 ===");
        foreach (var b in garage.FasterThan(30))
            Console.WriteLine(b);

        Console.WriteLine();
        Console.WriteLine("=== LINQ — середня швидкість ===");
        double avg = garage.All().Average(b => b.Speed);
        Console.WriteLine("Середня швидкість: " + avg);

        Console.WriteLine();
        Console.WriteLine("=== LINQ — вибірка моделей зі Speed > 35 ===");
        var fastModels = garage.All()
                               .Where(b => b.Speed > 35)
                               .Select(b => b.Model);
        foreach (var m in fastModels)
            Console.WriteLine(m);
    }
}
