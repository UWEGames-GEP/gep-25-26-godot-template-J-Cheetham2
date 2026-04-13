using Godot;
using System.Collections.Generic;
using System;

public partial class GameManager : Node

{
    public enum GameState { Exploring, Interacting, Paused }
    [Export] public GameState CurrentState { get; set; } = GameState.Exploring;
    [Export] public Label InteractionLabel;
    [Export] public Control InventoryMenu;
    [Export] public ItemList ItemDisplay;
    [Export] public Node3D Player;

    private ItemData _currentSelectedItem;
    private List<ItemData> _inventory = new List<ItemData>();

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()

    {
        GD.Print("GameManager Initialized. Current State: " + CurrentState);

        if (InventoryMenu != null) InventoryMenu.Visible = false;
        if (InteractionLabel != null) InteractionLabel.Visible = false;

        Input.MouseMode = Input.MouseModeEnum.Captured;
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

        if (Input.IsActionJustPressed("ui_inventory"))
        {
            ToggleInventory();
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

            if (InteractionLabel != null)
            {
                InteractionLabel.Text = $"Press [E] to pick up {item.ItemName}";
                InteractionLabel.Visible = true;
            }
        }
    }

    public void OnItemExitedZone(Node3D body)
    {
        if (body == _currentSelectedItem)
        {
            GD.Print("Left the interaction zone.");
            _currentSelectedItem = null;

            if (InteractionLabel != null) InteractionLabel.Visible = false;
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

            if (InteractionLabel != null) InteractionLabel.Visible = false;
        }
    }

    private void ToggleInventory()
    {
        if (CurrentState == GameState.Paused) return;

        if (CurrentState == GameState.Interacting)
        {
            CurrentState = GameState.Exploring;
            InventoryMenu.Visible = false;
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetTree().Paused = false;
        }
        else
        {
            CurrentState = GameState.Interacting;
            UpdateInventoryUI();
            InventoryMenu.Visible = true;
            Input.MouseMode = Input.MouseModeEnum.Visible;
            GetTree().Paused = true;
        }
    }

    private void UpdateInventoryUI()
    {
        ItemDisplay.Clear();
        foreach (var item in _inventory)
        {
            ItemDisplay.AddItem(item.ItemName);
        }
    }

    public void _on_drop_button_pressed()
    {
        int[] selected = ItemDisplay.GetSelectedItems();

        if (selected.Length > 0)
        {
            int index = selected[0];
            ItemData item = _inventory[index];

            if (item.WorldScene != null)
            {
                var droppedInstance = item.WorldScene.Instantiate<Node3D>();
                GetTree().Root.AddChild(droppedInstance);

                droppedInstance.GlobalPosition = Player.GlobalPosition + (-Player.GlobalTransform.Basis.Z * 2.0f);
            }

            _inventory.RemoveAt(index);
            UpdateInventoryUI();
        }
    }

}