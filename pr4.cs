using System;
using System.Collections.Generic;

// Базовий клас з інтерфейсами
public abstract class Bicycle : ICloneable, IComparable<Bicycle>
{
    public string Brand { get; set; }
    public int Year { get; set; }

    public Bicycle(string brand, int year)
    {
        Brand = brand;
        Year = year;
    }

    public abstract string GetInfo();

    // --- ICloneable ---
    public virtual object Clone()
    {
        return this.MemberwiseClone(); // Просте клонування
    }

    // --- IComparable ---
    // Порівняння за роком випуску (новіші > старі)
    public int CompareTo(Bicycle other)
    {
        return this.Year.CompareTo(other.Year);
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
        => $"MTB: {Brand}, {Year}, {Suspension} mm";

    public override object Clone()
    {
        return new MountainBike(Brand, Year, Suspension);
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
        => $"Road: {Brand}, {Year}, {Weight} kg";

    public override object Clone()
    {
        return new RoadBike(Brand, Year, Weight);
    }
}


// Сервіс для роботи з колекцією
public class BicycleService
{
    public List<Bicycle> Bikes { get; } = new List<Bicycle>();

    public void Add(Bicycle b) => Bikes.Add(b);

    public void Show()
    {
        foreach (var b in Bikes)
            Console.WriteLine(b.GetInfo());
    }

    public void Sort()
    {
        Bikes.Sort();   // Використовує IComparable
    }
}


// Головна програма
public class Program
{
    static void Main()
    {
        BicycleService service = new BicycleService();

        service.Add(new MountainBike("Trek", 2023, 120));
        service.Add(new MountainBike("Scott", 2020, 80));
        service.Add(new RoadBike("Giant", 2022, 8.5));
        service.Add(new RoadBike("Cube", 2019, 7.9));

        Console.WriteLine("Original list:");
        service.Show();

        // --- Сортування ---
        service.Sort();

        Console.WriteLine("\nAfter sorting by year:");
        service.Show();

        // --- Клонування ---
        Console.WriteLine("\nCloning example:");
        Bicycle original = service.Bikes[0];
        Bicycle clone = (Bicycle)original.Clone();

        Console.WriteLine("Original: " + original.GetInfo());
        Console.WriteLine("Clone:    " + clone.GetInfo());

        // Змінюємо клон — оригінал залишиться тим самим
        clone.Brand = clone.Brand + " COPY";

        Console.WriteLine("\nAfter modifying clone:");
        Console.WriteLine("Original: " + original.GetInfo());
        Console.WriteLine("Clone:    " + clone.GetInfo());
    }
}
