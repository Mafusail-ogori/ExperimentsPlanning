using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PlantSorter
{
    private readonly List<Plant> plants;
    private readonly object lockObject = new object();
    private readonly Mutex mutex = new Mutex();
    
    // Counters for different plant types - shared resources that need synchronization
    private int floweringCount = 0;
    private int decorativeCount = 0;
    private int vegetableCount = 0;
    private int fruitCount = 0;
    private int bushCount = 0;

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

    // New methods with critical section implementations
    
    // 1. Using lock statement
    public void CountPlantTypesByLock()
    {
        Console.WriteLine("\n=== Counting plant types using lock ===");
        ResetCounters();
        var stopwatch = Stopwatch.StartNew();
        
        // Create multiple tasks to count plants by type
        var tasks = new List<Task>();
        for (int i = 0; i < 5; i++)
        {
            int startIndex = i * (plants.Count / 5);
            int endIndex = (i == 4) ? plants.Count : (i + 1) * (plants.Count / 5);
            
            tasks.Add(Task.Run(() =>
            {
                for (int j = startIndex; j < endIndex; j++)
                {
                    // Critical section protected by lock
                    lock (lockObject)
                    {
                        IncrementCounterForType(plants[j].Type);
                    }
                }
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
        stopwatch.Stop();
        
        PrintTypeCounters();
        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
    }
    
    // 2. Using Monitor.Enter/Exit
    public void CountPlantTypesByMonitor()
    {
        Console.WriteLine("\n=== Counting plant types using Monitor.Enter/Exit ===");
        ResetCounters();
        var stopwatch = Stopwatch.StartNew();
        
        // Create multiple tasks to count plants by type
        var tasks = new List<Task>();
        for (int i = 0; i < 5; i++)
        {
            int startIndex = i * (plants.Count / 5);
            int endIndex = (i == 4) ? plants.Count : (i + 1) * (plants.Count / 5);
            
            tasks.Add(Task.Run(() =>
            {
                for (int j = startIndex; j < endIndex; j++)
                {
                    // Critical section protected by Monitor
                    bool lockTaken = false;
                    try
                    {
                        Monitor.Enter(lockObject, ref lockTaken);
                        IncrementCounterForType(plants[j].Type);
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(lockObject);
                        }
                    }
                }
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
        stopwatch.Stop();
        
        PrintTypeCounters();
        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
    }
    
    public void CountPlantTypesByMutex()
    {
        Console.WriteLine("\n=== Counting plant types using Mutex ===");
        ResetCounters();
        var stopwatch = Stopwatch.StartNew();
        
        var tasks = new List<Task>();
        for (int i = 0; i < 5; i++)
        {
            int startIndex = i * (plants.Count / 5);
            int endIndex = (i == 4) ? plants.Count : (i + 1) * (plants.Count / 5);
            
            tasks.Add(Task.Run(() =>
            {
                for (int j = startIndex; j < endIndex; j++)
                {
                    mutex.WaitOne();
                    try
                    {
                        IncrementCounterForType(plants[j].Type);
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
        stopwatch.Stop();
        
        PrintTypeCounters();
        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
    }
    
    // 4. Using Interlocked
    public void CountPlantTypesByInterlocked()
    {
        Console.WriteLine("\n=== Counting plant types using Interlocked ===");
        ResetCounters();
        var stopwatch = Stopwatch.StartNew();
        
        // For Interlocked, we need separate variables for each counter
        int flowering = 0, decorative = 0, vegetable = 0, fruit = 0, bush = 0;
        
        // Create multiple tasks to count plants by type
        var tasks = new List<Task>();
        for (int i = 0; i < 5; i++)
        {
            int startIndex = i * (plants.Count / 5);
            int endIndex = (i == 4) ? plants.Count : (i + 1) * (plants.Count / 5);
            
            tasks.Add(Task.Run(() =>
            {
                for (int j = startIndex; j < endIndex; j++)
                {
                    // Atomic operations with Interlocked
                    switch (plants[j].Type)
                    {
                        case "Flowering":
                            Interlocked.Increment(ref flowering);
                            break;
                        case "Decorative":
                            Interlocked.Increment(ref decorative);
                            break;
                        case "Vegetable":
                            Interlocked.Increment(ref vegetable);
                            break;
                        case "Fruit":
                            Interlocked.Increment(ref fruit);
                            break;
                        case "Bush":
                            Interlocked.Increment(ref bush);
                            break;
                    }
                }
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
        
        // Copy the final values to the class members
        floweringCount = flowering;
        decorativeCount = decorative;
        vegetableCount = vegetable;
        fruitCount = fruit;
        bushCount = bush;
        
        stopwatch.Stop();
        
        PrintTypeCounters();
        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds} ms");
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

        // Add the critical section comparison tests
        CountPlantTypesByLock();
        CountPlantTypesByMonitor();
        CountPlantTypesByMutex();
        CountPlantTypesByInterlocked();

        Console.WriteLine("\n=== Critical Section Methods Performance Summary ===");
        var criticalSectionStopwatch = Stopwatch.StartNew();
        
        Task.Run(CountPlantTypesByLock).Wait();
        Task.Run(CountPlantTypesByMonitor).Wait();
        Task.Run(CountPlantTypesByMutex).Wait();
        Task.Run(CountPlantTypesByInterlocked).Wait();
        
        criticalSectionStopwatch.Stop();
        Console.WriteLine($"Total time for all critical section methods: {criticalSectionStopwatch.ElapsedMilliseconds} ms");

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
    
    // Helper methods for critical section implementations
    private void ResetCounters()
    {
        floweringCount = 0;
        decorativeCount = 0;
        vegetableCount = 0;
        fruitCount = 0;
        bushCount = 0;
    }
    
    private void IncrementCounterForType(string type)
    {
        switch (type)
        {
            case "Flowering":
                floweringCount++;
                break;
            case "Decorative":
                decorativeCount++;
                break;
            case "Vegetable":
                vegetableCount++;
                break;
            case "Fruit":
                fruitCount++;
                break;
            case "Bush":
                bushCount++;
                break;
        }
    }
    
    private void PrintTypeCounters()
    {
        Console.WriteLine($"Flowering: {floweringCount}");
        Console.WriteLine($"Decorative: {decorativeCount}");
        Console.WriteLine($"Vegetable: {vegetableCount}");
        Console.WriteLine($"Fruit: {fruitCount}");
        Console.WriteLine($"Bush: {bushCount}");
    }
}