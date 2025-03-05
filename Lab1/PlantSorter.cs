using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public class PlantSorter
{
    private List<Plant> plants;

    public PlantSorter(int count)
    {
        plants = GeneratePlants(count);
    }

    private List<Plant> GeneratePlants(int count)
    {
        var random = new Random();
        var types = new[] { "Flowering", "Decorative", "Vegetable", "Fruit", "Bush" };
        var varieties = new[] { "Ukrainian", "European", "Asian", "Experimental", "Local" };
        var conditions = new[] { "Sunny", "Partial Shade", "Shade", "Moderately Wet", "Dry" };

        return Enumerable.Range(0, count)
            .Select(_ => new Plant
            {
                Type = types[random.Next(types.Length)],
                Variety = varieties[random.Next(varieties.Length)],
                Description = $"Plant description {random.Next(1000)}",
                GrowingConditions = conditions[random.Next(conditions.Length)],
                Photo = $"photo_{random.Next(1000)}.jpg"
            }).ToList();
    }

    public void SortByType()
    {
        var stopwatch = Stopwatch.StartNew();
        var sortedPlants = plants.OrderBy(p => p.Type).ToList();
        stopwatch.Stop();

        Console.WriteLine($"Sorting by type. Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    public void SortByVariety()
    {
        var stopwatch = Stopwatch.StartNew();
        var sortedPlants = plants.OrderBy(p => p.Variety).ToList();
        stopwatch.Stop();

        Console.WriteLine($"Sorting by variety. Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    public void SortByGrowingConditions()
    {
        var stopwatch = Stopwatch.StartNew();
        var sortedPlants = plants.OrderBy(p => p.GrowingConditions).ToList();
        stopwatch.Stop();

        Console.WriteLine($"Sorting by growing conditions. Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    public void SortByTypeThreaded()
    {
        var stopwatch = Stopwatch.StartNew();
        
        List<Plant>? sortedPlants = null;
        var thread = new Thread(() => {
            sortedPlants = plants.OrderBy(p => p.Type).ToList();
        });

        thread.Start();
        thread.Join();

        stopwatch.Stop();
        Console.WriteLine($"Sorting by type (threaded). Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants!);
    }

    public void SortByVarietyThreaded()
    {
        var stopwatch = Stopwatch.StartNew();
        
        List<Plant>? sortedPlants = null;
        var thread = new Thread(() => {
            sortedPlants = plants.OrderBy(p => p.Variety).ToList();
        });

        thread.Start();
        thread.Join();

        stopwatch.Stop();
        Console.WriteLine($"Sorting by variety (threaded). Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants!);
    }

    public void SortByGrowingConditionsThreaded()
    {
        var stopwatch = Stopwatch.StartNew();
        
        List<Plant>? sortedPlants = null;
        var thread = new Thread(() => {
            sortedPlants = plants.OrderBy(p => p.GrowingConditions).ToList();
        });

        thread.Start();
        thread.Join();

        stopwatch.Stop();
        Console.WriteLine($"Sorting by growing conditions (threaded). Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants!);
    }

    public void RunAllComparisons()
    {
        Console.WriteLine("\n=== Standard sorting methods ===");
        SortByType();
        SortByVariety();
        SortByGrowingConditions();

        Console.WriteLine("\n=== Threaded sorting methods ===");
        SortByTypeThreaded();
        SortByVarietyThreaded();
        SortByGrowingConditionsThreaded();
    }

    private void PrintFirstFew(List<Plant> plants, int count = 5)
    {
        Console.WriteLine($"First {count} elements:");
        plants.Take(count).ToList().ForEach(p => Console.WriteLine(p));
        Console.WriteLine();
    }
}