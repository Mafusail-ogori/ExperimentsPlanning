using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

    // Standard sorting method for comparison
    public void SortByTypeStandard()
    {
        var stopwatch = Stopwatch.StartNew();
        var sortedPlants = plants.OrderBy(p => p.Type).ToList();
        stopwatch.Stop();

        Console.WriteLine($"Standard sorting by type. Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    // Method 1: new Task(), task.Start()
    public void SortByTypeWithNewTask()
    {
        var stopwatch = Stopwatch.StartNew();
        
        List<Plant>? sortedPlants = null;
        var task = new Task(() => {
            sortedPlants = plants.OrderBy(p => p.Type).ToList();
        });

        task.Start();
        task.Wait(); // Wait for task to complete
        
        stopwatch.Stop();
        Console.WriteLine($"Sorting using new Task().Start(). Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants!);
    }

    // Method 2: Task.Factory.StartNew()
    public void SortByTypeWithTaskFactory()
    {
        var stopwatch = Stopwatch.StartNew();
        
        var task = Task.Factory.StartNew(() => {
            return plants.OrderBy(p => p.Type).ToList();
        });
        
        var sortedPlants = task.Result; // Wait for completion and get result
        
        stopwatch.Stop();
        Console.WriteLine($"Sorting using Task.Factory.StartNew(). Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    // Method 3: Synchronous execution with task.Result
    public void SortByTypeWithTaskResult()
    {
        var stopwatch = Stopwatch.StartNew();
        
        Task<List<Plant>> task = new Task<List<Plant>>(() => {
            return plants.OrderBy(p => p.Type).ToList();
        });
        
        task.Start();
        var sortedPlants = task.Result; // Synchronously wait for the result
        
        stopwatch.Stop();
        Console.WriteLine($"Sorting using task.Result. Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    // Method 4.1: Synchronization with Thread.Sleep
    public void SortWithThreadSleep()
    {
        var stopwatch = Stopwatch.StartNew();
        
        Task<List<Plant>> task = Task.Run(() => {
            Thread.Sleep(10); // Simulate some work with a small delay
            return plants.OrderBy(p => p.Type).ToList();
        });
        
        var sortedPlants = task.Result;
        
        stopwatch.Stop();
        Console.WriteLine($"Sorting with Thread.Sleep(). Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    // Method 4.2: Synchronization with Thread.SpinWait
    public void SortWithThreadSpinWait()
    {
        var stopwatch = Stopwatch.StartNew();
        
        Task<List<Plant>> task = Task.Run(() => {
            Thread.SpinWait(100); // Busy waiting
            return plants.OrderBy(p => p.Type).ToList();
        });
        
        var sortedPlants = task.Result;
        
        stopwatch.Stop();
        Console.WriteLine($"Sorting with Thread.SpinWait(). Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    // Method 4.3: Synchronization with task.Wait(timeout)
    public void SortWithTaskWaitTimeout()
    {
        var stopwatch = Stopwatch.StartNew();
        
        Task<List<Plant>> task = Task.Run(() => {
            return plants.OrderBy(p => p.Type).ToList();
        });
        
        bool completed = task.Wait(10000); // Wait for max 10 seconds
        
        List<Plant> sortedPlants;
        if (completed)
        {
            sortedPlants = task.Result;
        }
        else
        {
            Console.WriteLine("Task timed out!");
            sortedPlants = new List<Plant>();
        }
        
        stopwatch.Stop();
        Console.WriteLine($"Sorting with task.Wait(timeout). Time: {stopwatch.ElapsedMilliseconds} ms");
        PrintFirstFew(sortedPlants);
    }

    // Method 4.4: Synchronization with Task.WaitAll
    public void SortWithTaskWaitAll()
    {
        var stopwatch = Stopwatch.StartNew();
        
        Task<List<Plant>> sortByTypeTask = Task.Run(() => {
            return plants.OrderBy(p => p.Type).ToList();
        });
        
        Task<List<Plant>> sortByVarietyTask = Task.Run(() => {
            return plants.OrderBy(p => p.Variety).ToList();
        });
        
        Task.WaitAll(sortByTypeTask, sortByVarietyTask);
        
        stopwatch.Stop();
        Console.WriteLine($"Sorting with Task.WaitAll(). Time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine("Results from both tasks:");
        Console.WriteLine("1. Sort by Type:");
        PrintFirstFew(sortByTypeTask.Result);
        Console.WriteLine("2. Sort by Variety:");
        PrintFirstFew(sortByVarietyTask.Result);
    }

    // Method 4.5: Synchronization with Task.WaitAny
    public void SortWithTaskWaitAny()
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Create two tasks - one sorting by type and one by variety
        Task<List<Plant>> sortByTypeTask = Task.Run(() => {
            Thread.Sleep(50); // Add a small delay to one task
            return plants.OrderBy(p => p.Type).ToList();
        });
        
        Task<List<Plant>> sortByVarietyTask = Task.Run(() => {
            return plants.OrderBy(p => p.Variety).ToList();
        });
        
        // Wait for the first task to complete
        int index = Task.WaitAny(sortByTypeTask, sortByVarietyTask);
        
        stopwatch.Stop();
        Console.WriteLine($"First completed task (index {index}) with Task.WaitAny(). Time: {stopwatch.ElapsedMilliseconds} ms");
        
        if (index == 0)
        {
            Console.WriteLine("Sort by Type completed first:");
            PrintFirstFew(sortByTypeTask.Result);
        }
        else
        {
            Console.WriteLine("Sort by Variety completed first:");
            PrintFirstFew(sortByVarietyTask.Result);
        }
        
        // Wait for the other task to complete
        if (index == 0)
        {
            sortByVarietyTask.Wait();
        }
        else
        {
            sortByTypeTask.Wait();
        }
    }

    // Run all methods for comparison
    public void RunAllTaskMethods()
    {
        Console.WriteLine("\n=== Comparing different Task methods ===");
        
        // Standard method for baseline
        SortByTypeStandard();
        
        // Task creation methods
        SortByTypeWithNewTask();
        SortByTypeWithTaskFactory();
        SortByTypeWithTaskResult();
        
        // Synchronization methods
        SortWithThreadSleep();
        SortWithThreadSpinWait();
        SortWithTaskWaitTimeout();
        SortWithTaskWaitAll();
        SortWithTaskWaitAny();
    }

    private void PrintFirstFew(List<Plant> plants, int count = 5)
    {
        Console.WriteLine($"First {count} elements:");
        plants.Take(count).ToList().ForEach(p => Console.WriteLine(p));
        Console.WriteLine();
    }
}