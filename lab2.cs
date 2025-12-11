using System;
using System.Collections.Generic;

// Абстрактний базовий клас
public abstract class Bicycle
{
    public string Brand { get; set; }
    public int Year { get; set; }

    public Bicycle(string brand, int year)
    {
        Brand = brand;
        Year = year;
    }

    // Абстрактний метод 
    public abstract string GetInfo();

    // Віртуальний метод 
    public virtual void Ride()
    {
        Console.WriteLine($"{Brand} is riding normally.");
    }

    // Ще один віртуальний метод для лаб 2 
    public virtual void Service()
    {
        Console.WriteLine($"Basic service for {Brand}.");
    }
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
    {
        return $"Mountain Bike: {Brand}, {Year}, Suspension: {Suspension} mm";
    }

    public override void Ride()
    {
        Console.WriteLine($"{Brand} is riding off-road!");
    }

    public override void Service()
    {
        if (Suspension <= 0)
            throw new Exception("Suspension value must be positive!");

        Console.WriteLine($"{Brand} suspension checked ({Suspension} mm).");
    }
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
    {
        return $"Road Bike: {Brand}, {Year}, Weight: {Weight} kg";
    }

    public override void Ride()
    {
        Console.WriteLine($"{Brand} is riding very fast!");
    }

    public override void Service()
    {
        if (Weight <= 0)
            throw new Exception("Weight must be positive!");

        Console.WriteLine($"{Brand} weight OK: {Weight} kg.");
    }
}

// ElectricBike (новий клас для Лаб 2)
public class ElectricBike : Bicycle
{
    public int Battery { get; set; }

    public ElectricBike(string brand, int year, int battery)
        : base(brand, year)
    {
        Battery = battery;
    }

    public override string GetInfo()
    {
        return $"E-Bike: {Brand}, {Year}, Battery: {Battery} Wh";
    }

    public override void Ride()
    {
        Console.WriteLine($"{Brand} uses electric motor to help riders.");
    }

    public override void Service()
    {
        if (Battery < 100)
            throw new Exception("Battery too weak!");

        Console.WriteLine($"{Brand} battery checked ({Battery} Wh).");
    }
}

// Сервісний клас
public class BicycleService
{
    private List<Bicycle> bikes = new List<Bicycle>();

    public void Add(Bicycle bike)
    {
        bikes.Add(bike);
    }

    public void ShowAll()
    {
        foreach (var b in bikes)
            Console.WriteLine(b.GetInfo());
    }

    public void DoService()
    {
        foreach (var b in bikes)
        {
            try
            {
                b.Service();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error servicing {b.Brand}: {ex.Message}");
            }
        }
    }
}

// Program
internal class Program
{
    static void Main(string[] args)
    {
        BicycleService service = new BicycleService();

        service.Add(new MountainBike("Trek", 2023, 120));
        service.Add(new RoadBike("Giant", 2022, 8.5));
        service.Add(new ElectricBike("Cube", 2024, 400));

        Console.WriteLine("Bicycles:");
        service.ShowAll();

        Console.WriteLine("\nService:");
        service.DoService();
    }
}
