using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        int plantCount = args.Length > 0 && int.TryParse(args[0], out int count) 
            ? count 
            : 1000;

        Console.WriteLine($"Generating {plantCount} plants");

        var sorter = new PlantSorter(plantCount);
        sorter.RunAllTaskMethods();
    }
}