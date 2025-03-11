using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PlantSorter
{
    private readonly List<Plant> plants;

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

        Console.WriteLine("\n=== Task.Run sorting methods ===");
        var taskRunStopwatch = Stopwatch.StartNew();

        var taskRunType = Task.Run(SortByType);
        var taskRunVariety = Task.Run(SortByVariety);
        var taskRunConditions = Task.Run(SortByGrowingConditions);

        Task.WaitAll(taskRunType, taskRunVariety, taskRunConditions);

        taskRunStopwatch.Stop();
        Console.WriteLine($"Total time for Task.Run methods: {taskRunStopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine("\n=== New Task with Start() sorting methods ===");
        var newTaskStopwatch = Stopwatch.StartNew();

        var newTaskType = new Task(SortByType);
        var newTaskVariety = new Task(SortByVariety);
        var newTaskConditions = new Task(SortByGrowingConditions);

        newTaskType.Start();
        newTaskVariety.Start();
        newTaskConditions.Start();

        Task.WaitAll(newTaskType, newTaskVariety, newTaskConditions);

        newTaskStopwatch.Stop();
        Console.WriteLine($"Total time for New Task with Start() methods: {newTaskStopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine("\n=== Task.Factory.StartNew sorting methods ===");
        var factoryStopwatch = Stopwatch.StartNew();

        var factoryTaskType = Task.Factory.StartNew(SortByType);
        var factoryTaskVariety = Task.Factory.StartNew(SortByVariety);
        var factoryTaskConditions = Task.Factory.StartNew(SortByGrowingConditions);

        Task.WaitAll(factoryTaskType, factoryTaskVariety, factoryTaskConditions);

        factoryStopwatch.Stop();
        Console.WriteLine($"Total time for Task.Factory.StartNew methods: {factoryStopwatch.ElapsedMilliseconds} ms");


        Console.WriteLine("\n=== Performance Summary ===");
        Console.WriteLine($"Standard methods: {standardStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Task.Run methods: {taskRunStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"New Task with Start() methods: {newTaskStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Task.Factory.StartNew methods: {factoryStopwatch.ElapsedMilliseconds} ms");
    }

    private static void PrintFirstFew(List<Plant> plants, int count = 3)
    {
        Console.WriteLine($"First {count} elements:");
        plants.Take(count).ToList().ForEach(p => Console.WriteLine(p));
        Console.WriteLine();
    }
}