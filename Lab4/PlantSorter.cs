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
    private int floweringCount = 0;
    private int decorativeCount = 0;
    private int vegetableCount = 0;
    private int fruitCount = 0;
    private int bushCount = 0;
    private List<Plant> sunnyPlants = new List<Plant>();
    private List<Plant> shadePlants = new List<Plant>();
    private List<Plant> partialShadePlants = new List<Plant>();
    private List<Plant> wetPlants = new List<Plant>();
    private List<Plant> dryPlants = new List<Plant>();
    private bool analysisDone = false;
    private bool sortingDone = false;

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

    // Methods with critical section implementations
    
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
                    // Critical section protected by Mutex
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

    // NEW METHODS IMPLEMENTING NESTED THREADS AND DEPENDENCIES

    // 1. Task with nested child tasks using AttachedToParent
    public void AnalyzePlantsByCondition()
    {
        Console.WriteLine("\n=== Analyzing plants by growing conditions with nested tasks ===");
        var stopwatch = Stopwatch.StartNew();
        
        // Create parent task
        var parentTask = Task.Factory.StartNew(() =>
        {
            Console.WriteLine("Parent task started - Analyzing growing conditions");
            
            // Initialize result lists
            sunnyPlants = new List<Plant>();
            shadePlants = new List<Plant>();
            partialShadePlants = new List<Plant>();
            wetPlants = new List<Plant>();
            dryPlants = new List<Plant>();
            
            // Create child tasks that will be attached to parent
            Task sunnyTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Child task started - Finding sunny plants");
                sunnyPlants = plants.Where(p => p.GrowingConditions == "Sunny").ToList();
                Console.WriteLine($"Found {sunnyPlants.Count} sunny plants");
            }, TaskCreationOptions.AttachedToParent);
            
            Task shadeTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Child task started - Finding shade plants");
                shadePlants = plants.Where(p => p.GrowingConditions == "Shade").ToList();
                Console.WriteLine($"Found {shadePlants.Count} shade plants");
            }, TaskCreationOptions.AttachedToParent);
            
            Task partialShadeTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Child task started - Finding partial shade plants");
                partialShadePlants = plants.Where(p => p.GrowingConditions == "Partial Shade").ToList();
                Console.WriteLine($"Found {partialShadePlants.Count} partial shade plants");
            }, TaskCreationOptions.AttachedToParent);
            
            Task wetTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Child task started - Finding moderately wet plants");
                wetPlants = plants.Where(p => p.GrowingConditions == "Moderately Wet").ToList();
                Console.WriteLine($"Found {wetPlants.Count} moderately wet plants");
            }, TaskCreationOptions.AttachedToParent);
            
            Task dryTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Child task started - Finding dry plants");
                dryPlants = plants.Where(p => p.GrowingConditions == "Dry").ToList();
                Console.WriteLine($"Found {dryPlants.Count} dry plants");
            }, TaskCreationOptions.AttachedToParent);
            
            // The parent task will automatically wait for all child tasks
            Console.WriteLine("Parent task waiting for all child tasks to complete");
        });
        
        // Wait for the parent task to complete (which implicitly waits for all child tasks)
        parentTask.Wait();
        analysisDone = true;
        
        stopwatch.Stop();
        Console.WriteLine($"Plant analysis complete. Time: {stopwatch.ElapsedMilliseconds} ms");
        
        // Print summary of results
        Console.WriteLine("\nGrowing Conditions Analysis Summary:");
        Console.WriteLine($"Sunny plants: {sunnyPlants.Count}");
        Console.WriteLine($"Shade plants: {shadePlants.Count}");
        Console.WriteLine($"Partial shade plants: {partialShadePlants.Count}");
        Console.WriteLine($"Moderately wet plants: {wetPlants.Count}");
        Console.WriteLine($"Dry plants: {dryPlants.Count}");
    }
    
    // 2. Tasks with ContinueWhenAll dependency
    public void RunSortingWithContinuation()
    {
        Console.WriteLine("\n=== Running sorting with ContinueWhenAll ===");
        var stopwatch = Stopwatch.StartNew();
        
        // Create three sorting tasks
        Task<List<Plant>> typeTask = Task.Run(() => plants.OrderBy(p => p.Type).ToList());
        Task<List<Plant>> varietyTask = Task.Run(() => plants.OrderBy(p => p.Variety).ToList());
        Task<List<Plant>> conditionsTask = Task.Run(() => plants.OrderBy(p => p.GrowingConditions).ToList());
        
        // Continue with a new task when all sorting tasks complete
        Task.Factory.ContinueWhenAll(
            new Task[] { typeTask, varietyTask, conditionsTask },
            completedTasks =>
            {
                Console.WriteLine("All sorting tasks completed. Generating report...");
                
                // Access results from each task
                var byType = ((Task<List<Plant>>)completedTasks[0]).Result;
                var byVariety = ((Task<List<Plant>>)completedTasks[1]).Result;
                var byConditions = ((Task<List<Plant>>)completedTasks[2]).Result;
                
                // Generate a report with results from all tasks
                Console.WriteLine("\nSorting Results Summary:");
                Console.WriteLine("=== Plants Sorted By Type ===");
                PrintFirstFew(byType, 2);
                
                Console.WriteLine("=== Plants Sorted By Variety ===");
                PrintFirstFew(byVariety, 2);
                
                Console.WriteLine("=== Plants Sorted By Growing Conditions ===");
                PrintFirstFew(byConditions, 2);
                
                sortingDone = true;
            }
        ).Wait(); // Wait for the continuation task to complete
        
        stopwatch.Stop();
        Console.WriteLine($"Sorting with continuation complete. Time: {stopwatch.ElapsedMilliseconds} ms");
    }
    
    // 3. Tasks with ContinueWhenAny dependency
    public void FilterRarePlantsWithContinuation()
    {
        Console.WriteLine("\n=== Filtering rare plants with ContinueWhenAny ===");
        var stopwatch = Stopwatch.StartNew();
        
        // Create tasks to find plants of each variety with the smallest counts
        Task<List<Plant>> ukrainianTask = Task.Run(() => 
            plants.Where(p => p.Variety == "Ukrainian").ToList());
        
        Task<List<Plant>> europeanTask = Task.Run(() => 
            plants.Where(p => p.Variety == "European").ToList());
        
        Task<List<Plant>> asianTask = Task.Run(() => 
            plants.Where(p => p.Variety == "Asian").ToList());
        
        Task<List<Plant>> experimentalTask = Task.Run(() => 
            plants.Where(p => p.Variety == "Experimental").ToList());
        
        Task<List<Plant>> localTask = Task.Run(() => 
            plants.Where(p => p.Variety == "Local").ToList());
        
        // Continue as soon as any task completes
        Task<string> rareVarietyTask = Task.Factory.ContinueWhenAny(
            new Task<List<Plant>>[] { ukrainianTask, europeanTask, asianTask, experimentalTask, localTask },
            completedTask =>
            {
                var plants = completedTask.Result;
                var variety = plants.FirstOrDefault()?.Variety ?? "Unknown";
                Console.WriteLine($"First completed task found: {plants.Count} {variety} plants");
                
                return variety;
            }
        );
        
        // Wait for the rare variety task to complete
        string firstVariety = rareVarietyTask.Result;
        
        // Then continue with another task that does something with all results
        Task.Factory.ContinueWhenAll(
            new Task[] { ukrainianTask, europeanTask, asianTask, experimentalTask, localTask },
            allTasks =>
            {
                Console.WriteLine("\nAll variety filtering tasks completed. Finding rarest variety...");
                
                var varieties = new Dictionary<string, int>
                {
                    { "Ukrainian", ukrainianTask.Result.Count },
                    { "European", europeanTask.Result.Count },
                    { "Asian", asianTask.Result.Count },
                    { "Experimental", experimentalTask.Result.Count },
                    { "Local", localTask.Result.Count }
                };
                
                // Find the variety with the least plants
                var rarestVariety = varieties.OrderBy(v => v.Value).First();
                Console.WriteLine($"Rarest variety: {rarestVariety.Key} with {rarestVariety.Value} plants");
                
                // Print distribution
                Console.WriteLine("\nVariety Distribution:");
                foreach (var variety in varieties.OrderBy(v => v.Key))
                {
                    Console.WriteLine($"{variety.Key}: {variety.Value} plants");
                }
            }
        ).Wait();
        
        stopwatch.Stop();
        Console.WriteLine($"Rare plant filtering complete. Time: {stopwatch.ElapsedMilliseconds} ms");
    }
    
    // 4. Combining everything - task dependencies, nested tasks and continuations
    public void RunComprehensiveAnalysis()
    {
        Console.WriteLine("\n=== Running comprehensive plant analysis ===");
        var stopwatch = Stopwatch.StartNew();
        
        // Step 1: Run analysis with nested tasks
        var analysisTask = Task.Run(() => AnalyzePlantsByCondition());
        
        // Step 2: When analysis is done, continue with sorting
        var sortingTask = analysisTask.ContinueWith(_ => 
        {
            Console.WriteLine("\nAnalysis completed, continuing with sorting...");
            RunSortingWithContinuation();
        });
        
        // Step 3: When both analysis and sorting are done, do final reporting
        Task.Factory.ContinueWhenAll(
            new Task[] { analysisTask, sortingTask },
            _ => 
            {
                Console.WriteLine("\n=== Final Comprehensive Report ===");
                
                // Find most common plant type by growing condition
                var sunnyTypes = sunnyPlants.GroupBy(p => p.Type)
                    .OrderByDescending(g => g.Count())
                    .First();
                
                var shadeTypes = shadePlants.GroupBy(p => p.Type)
                    .OrderByDescending(g => g.Count())
                    .First();
                
                Console.WriteLine($"Most common plant type in sunny conditions: {sunnyTypes.Key} ({sunnyTypes.Count()} plants)");
                Console.WriteLine($"Most common plant type in shade: {shadeTypes.Key} ({shadeTypes.Count()} plants)");
                
                // Calculate some statistics
                Console.WriteLine("\nPlant Type Distribution by Growing Conditions:");
                foreach (var type in new[] { "Flowering", "Decorative", "Vegetable", "Fruit", "Bush" })
                {
                    int sunny = sunnyPlants.Count(p => p.Type == type);
                    int shade = shadePlants.Count(p => p.Type == type);
                    int partial = partialShadePlants.Count(p => p.Type == type);
                    
                    Console.WriteLine($"{type} Plants: {sunny} sunny, {shade} shade, {partial} partial shade");
                }
            }
        ).Wait();
        
        stopwatch.Stop();
        Console.WriteLine($"Comprehensive analysis complete. Time: {stopwatch.ElapsedMilliseconds} ms");
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

        // Critical section tests
        CountPlantTypesByLock();
        CountPlantTypesByMonitor();
        CountPlantTypesByMutex();
        CountPlantTypesByInterlocked();
        
        // New tests with nested threads and dependencies
        var dependencyStopwatch = Stopwatch.StartNew();
        
        AnalyzePlantsByCondition();
        RunSortingWithContinuation();
        FilterRarePlantsWithContinuation();
        
        dependencyStopwatch.Stop();
        Console.WriteLine($"\nTotal time for individual dependency methods: {dependencyStopwatch.ElapsedMilliseconds} ms");
        
        // Run the comprehensive analysis that combines everything
        RunComprehensiveAnalysis();

        Console.WriteLine("\n=== Performance Summary ===");
        Console.WriteLine($"Standard methods: {standardStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Task.Run methods: {taskRunStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"New Task with Start() methods: {newTaskStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Task.Factory.StartNew methods: {factoryStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Individual dependency methods: {dependencyStopwatch.ElapsedMilliseconds} ms");
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
    
    // Added properties to use the analysisDone and sortingDone flags
    public bool IsAnalysisDone => analysisDone;
    public bool IsSortingDone => sortingDone;
    
    public void WaitForTaskCompletion()
    {
        while (!analysisDone || !sortingDone)
        {
            Console.WriteLine("Waiting for all tasks to complete...");
            Thread.Sleep(100);
        }
        Console.WriteLine("All tasks completed successfully!");
    }
}