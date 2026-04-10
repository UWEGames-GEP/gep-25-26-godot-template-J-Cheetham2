using Godot;
using System;

public partial class ItemData : Node3D
{
    [Export] public string ItemName { get; set; } = "New Item";
    [Export] public int Value { get; set; } = 0;

    public override void _Ready()
    {
        GD.Print($"{ItemName} is ready with a value of {Value}");
    }
}