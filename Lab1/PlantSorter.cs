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



    public void RunAllComparisons()
    {
        Console.WriteLine("\n=== Standard sorting methods ===");
        var standardStopwatch = Stopwatch.StartNew();

        SortByType();
        SortByVariety();
        SortByGrowingConditions();

        standardStopwatch.Stop();
        Console.WriteLine($"Total time for standard methods: {standardStopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine("\n=== Parallel sorting methods ===");
        var threadedStopwatch = Stopwatch.StartNew();

        var thread1 = new Thread(SortByType);
        var thread2 = new Thread(SortByVariety);
        var thread3 = new Thread(SortByGrowingConditions);

        thread1.Start();
        thread2.Start();
        thread3.Start();

        thread1.Join();
        thread2.Join();
        thread3.Join();

        threadedStopwatch.Stop();
        Console.WriteLine($"Total time for parallel methods: {threadedStopwatch.ElapsedMilliseconds} ms");
    }


    private void PrintFirstFew(List<Plant> plants, int count = 3)
    {
        Console.WriteLine($"First {count} elements:");
        plants.Take(count).ToList().ForEach(p => Console.WriteLine(p));
        Console.WriteLine();
    }
}