using System;
using System.Collections.Generic;

// Абстрактний клас Bicycle
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

// MountainBike
public class MountainBike : Bicycle
{
    public int Suspension { get; set; }

    public MountainBike(string brand, int year, int suspension)
        : base(brand, year)
    {
        Suspension = suspension;
    }

    public override string GetInfo()
        => $"MTB: {Brand}, {Year}, {Suspension}mm";
}

// RoadBike
public class RoadBike : Bicycle
{
    public double Weight { get; set; }

    public RoadBike(string brand, int year, double weight)
        : base(brand, year)
    {
        Weight = weight;
    }

    public override string GetInfo()
        => $"Road: {Brand}, {Year}, {Weight}kg";
}

// Делегати
public delegate void BikeAction(Bicycle bike);          // Для дій над об’єктами
public delegate bool BikeCondition(Bicycle bike);       // Для перевірки умов

public class BicycleService
{
    private List<Bicycle> bikes = new List<Bicycle>();

    public void Add(Bicycle b) => bikes.Add(b);

    // 1) Метод, що застосовує делегат-дiю
    public void ForEach(BikeAction action)
    {
        foreach (var b in bikes)
            action(b);
    }

    // 2) Метод, що рахує елементи за умовою
    public int CountWhere(BikeCondition condition)
    {
        int count = 0;
        foreach (var b in bikes)
            if (condition(b))
                count++;
        return count;
    }
}

public class Program
{
    static void Main()
    {
        BicycleService service = new BicycleService();

        service.Add(new MountainBike("Trek", 2023, 120));
        service.Add(new MountainBike("Scott", 2020, 80));
        service.Add(new RoadBike("Giant", 2022, 8.5));
        service.Add(new RoadBike("Cube", 2019, 7.9));

        // --- ДЕЛЕГАТ 1: Виведення інформації ---
        BikeAction showInfo = b => Console.WriteLine(b.GetInfo());

        Console.WriteLine("All bicycles:");
        service.ForEach(showInfo);


        // --- ДЕЛЕГАТ 2: Фільтр — тільки нові велосипеди ---
        BikeCondition isNew = b => b.Year >= 2022;

        int newCount = service.CountWhere(isNew);
        Console.WriteLine($"\nNew bikes (2022+): {newCount}");


        // --- ДЕЛЕГАТ 3: Фільтр — тільки шоссейні (RoadBike) ---
        BikeCondition roadOnly = b => b is RoadBike;

        int roadCount = service.CountWhere(roadOnly);
        Console.WriteLine($"Road bikes: {roadCount}");


        // --- ДЕЛЕГАТ 4: Друк лише брендів ---
        BikeAction printBrand = b => Console.WriteLine("Brand: " + b.Brand);

        Console.WriteLine("\nBrands:");
        service.ForEach(printBrand);
    }
}
