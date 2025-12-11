using System;
using System.Collections.Generic;
using System.Linq;

// Абстрактний клас
public abstract class Bicycle
{
    public string Brand { get; set; }
    public int Year { get; set; }

    public Bicycle(string brand, int year)
    {
        Brand = brand;
        Year = year;
    }

    public abstract string GetInfo();
}

public class MountainBike : Bicycle
{
    public int Suspension { get; set; }

    public MountainBike(string brand, int year, int suspension)
        : base(brand, year)
    {
        Suspension = suspension;
    }

    public override string GetInfo()
        => $"MTB: {Brand}, {Year}, {Suspension} mm";
}

public class RoadBike : Bicycle
{
    public double Weight { get; set; }

    public RoadBike(string brand, int year, double weight)
        : base(brand, year)
    {
        Weight = weight;
    }

    public override string GetInfo()
        => $"Road: {Brand}, {Year}, {Weight} kg";
}


// Сервіс з ітератором
public class BicycleService
{
    private List<Bicycle> bikes = new List<Bicycle>();

    public void Add(Bicycle b) => bikes.Add(b);

    // --- Власний ітератор з yield ---
    public IEnumerable<Bicycle> NewerThan(int year)
    {
        foreach (var b in bikes)
            if (b.Year > year)
                yield return b;   // Повертаємо тільки якщо відповідає умові
    }

    // --- Доступ до всієї колекції ---
    public IEnumerable<Bicycle> GetAll() => bikes;
}


// Програма
public class Program
{
    static void Main()
    {
        BicycleService service = new BicycleService();

        service.Add(new MountainBike("Trek", 2023, 120));
        service.Add(new MountainBike("Scott", 2020, 80));
        service.Add(new RoadBike("Giant", 2022, 8.5));
        service.Add(new RoadBike("Cube", 2019, 7.9));

        Console.WriteLine("All bicycles:");
        foreach (var b in service.GetAll())
            Console.WriteLine(b.GetInfo());


        // --- Власний ітератор (yield) ---
        Console.WriteLine("\nBicycles newer than 2020:");
        foreach (var b in service.NewerThan(2020))
            Console.WriteLine(b.GetInfo());


        // --- LINQ приклади ---

        // 1. Фільтрація
        var roadOnly = service.GetAll()
            .Where(b => b is RoadBike);

        Console.WriteLine("\nRoad bikes:");
        foreach (var b in roadOnly)
            Console.WriteLine(b.GetInfo());


        // 2. Вибірка + Сортування
        var sortedByYear = service.GetAll()
            .OrderBy(b => b.Year);

        Console.WriteLine("\nSorted by year:");
        foreach (var b in sortedByYear)
            Console.WriteLine(b.GetInfo());


        // 3. Агрегування
        int countNew = service.GetAll()
            .Count(b => b.Year >= 2022);

        Console.WriteLine($"\nNumber of new bikes (2022+): {countNew}");

        double avgYear = service.GetAll()
            .Average(b => b.Year);

        Console.WriteLine($"Average production year: {avgYear:F1}");
    }
}
