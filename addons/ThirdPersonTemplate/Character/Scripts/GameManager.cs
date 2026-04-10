using Godot;

using System;

public partial class GameManager : Node

{
    public enum GameState { Exploring, Interacting, Paused }

    [Export] public GameState CurrentState { get; set; } = GameState.Exploring;

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

    }

    private void TogglePause()

    {

        if (CurrentState == GameState.Paused)

        {
            CurrentState = GameState.Exploring;

            GD.Print("Game Resumed...");

        }

        else

        {
            CurrentState = GameState.Paused;

            GD.Print("Game Paused!");

        }

    }

}