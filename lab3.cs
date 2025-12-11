using System;
using System.Collections.Generic;

// Базовий клас
public abstract class Bicycle
{
    private string brand;
    public string Brand
    {
        get => brand;
        set
        {
            brand = value;
            OnDataChanged?.Invoke(this, $"{Brand}: data changed");
        }
    }

    public int Year { get; set; }

    // Подія зміни даних
    public event Action<Bicycle, string> OnDataChanged;

    public Bicycle(string brand, int year)
    {
        Brand = brand;
        Year = year;
    }

    public abstract void Service();
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

    public override void Service()
    {
        if (Suspension <= 0)
            throw new Exception("Suspension must be > 0");
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
        => $"Road: {Brand}, {Year}, {Weight}kg";

    public override void Service()
    {
        if (Weight <= 0)
            throw new Exception("Weight must be > 0");
    }
}

// Сервіс велосипедів з подіями
public class BicycleService
{
    List<Bicycle> bikes = new List<Bicycle>();

    // Події
    public event Action<Bicycle> OnAdded;
    public event Action<Bicycle> OnRemoved;
    public event Action<Bicycle> OnServiced;

    public void Add(Bicycle b)
    {
        bikes.Add(b);
        SafeInvoke(() => OnAdded?.Invoke(b));
    }

    public void Remove(Bicycle b)
    {
        if (bikes.Remove(b))
            SafeInvoke(() => OnRemoved?.Invoke(b));
    }

    public void Show()
    {
        foreach (var b in bikes)
            Console.WriteLine(b.GetInfo());
    }

    public void ServiceAll()
    {
        foreach (var b in bikes)
        {
            try
            {
                b.Service();
                SafeInvoke(() => OnServiced?.Invoke(b));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error servicing {b.Brand}: {ex.Message}");
            }
        }
    }

    // Простий захист від падіння подій
    private void SafeInvoke(Action action)
    {
        try { action(); }
        catch (Exception ex)
        {
            Console.WriteLine($"Event error: {ex.Message}");
        }
    }
}


// Програма
public class Program
{
    static void Main()
    {
        BicycleService service = new BicycleService();

        // Найпростіші підписки
        service.OnAdded += b => Console.WriteLine($"Added: {b.Brand}");
        service.OnRemoved += b => Console.WriteLine($"Removed: {b.Brand}");
        service.OnServiced += b => Console.WriteLine($"Serviced: {b.Brand}");

        // Створення об'єктів
        var trek = new MountainBike("Trek", 2023, 120);
        trek.OnDataChanged += (bike, msg) => Console.WriteLine(msg);

        service.Add(trek);
        service.Add(new RoadBike("Giant", 2022, 8.5));

        Console.WriteLine("\nAll bicycles:");
        service.Show();

        Console.WriteLine("\nService:");
        service.ServiceAll();

        Console.WriteLine("\nChanging data:");
        trek.Brand = "Trek Pro";

        Console.WriteLine("\nRemoving bike:");
        service.Remove(trek);
    }
}
