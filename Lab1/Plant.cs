using System;

public class Plant
{
    public required string Type { get; set; }
    public required string Variety { get; set; }
    public required string Description { get; set; }
    public required string GrowingConditions { get; set; }
    public required string Photo { get; set; }

    public override string ToString()
    {
        return $"Type: {Type}, Variety: {Variety}, Conditions: {GrowingConditions}";
    }
}