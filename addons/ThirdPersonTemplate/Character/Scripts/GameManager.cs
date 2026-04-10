using Godot;
using System.Collections.Generic;
using System;

public partial class GameManager : Node

{
    public enum GameState { Exploring, Interacting, Paused }
    [Export] public GameState CurrentState { get; set; } = GameState.Exploring;

    private ItemData _currentSelectedItem;
    private List<ItemData> _inventory = new List<ItemData>();

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()

    {

        GD.Print("GameManager Initialized. Current State: " + CurrentState);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _Process(double delta)

    {

        if (Input.IsActionJustPressed("ui_cancel"))

        {
            TogglePause();
        }

        if (Input.IsActionJustPressed("interact") || Input.IsKeyPressed(Key.E))
        {
            AttemptInteraction();
        }

    }

    private void TogglePause()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Exploring;
            GetTree().Paused = false;
            GD.Print("Resuming Game...");
        }
        else
        {
            CurrentState = GameState.Paused;
            GetTree().Paused = true;
            GD.Print("Game Paused!");
        }
    }

    public void OnItemEnteredZone(Node3D body)
    {
        if (body is ItemData item)
        {
            _currentSelectedItem = item;
            GD.Print("Press E to interact with: " + _currentSelectedItem.ItemName);
        }
    }

    public void OnItemExitedZone(Node3D body)
    {
        if (body == _currentSelectedItem)
        {
            GD.Print("Left the interaction zone.");
            _currentSelectedItem = null;
        }
    }

    private void AttemptInteraction()
    {
        if (_currentSelectedItem != null)
        {
            GD.Print("Collected: " + _currentSelectedItem.ItemName);

            _inventory.Add(_currentSelectedItem);

            _currentSelectedItem.QueueFree();
            
            _currentSelectedItem = null;
        }
    }

}