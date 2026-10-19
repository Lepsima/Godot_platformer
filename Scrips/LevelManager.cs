using System;
using System.Diagnostics;
using Godot;

namespace Platformer.Scrips;

public partial class LevelManager : Node {
	private static LevelManager instance { get; set; }
	private static string level_name;

	[Export] private string default_level = "default_level";
	[Export] private string level_folder_path = "res://Scenes/Levels/";

	private Camera2D camera;
	
	// Spawn a fresh level prefab when loading the game scene
	public override void _Ready() {
		instance = this;
		
		// Get the path of the level to be loaded
		level_name ??= default_level;
		string path = $"{level_folder_path}{level_name}";
		if (!path.EndsWith(".tscn")) path += ".tscn";

		// Create level prefab, and place it on the current tree
		PackedScene level_resource = GD.Load<PackedScene>(path);
		if (level_resource == null) return;
		AddChild(level_resource.Instantiate());

    // Set camera limits
		camera = GetViewport().GetCamera2D();
		if(camera != null) {
			Node2D background = level_instance.GetChild<Node2D>(0);
		
			camera.LimitRight = Mathf.RoundToInt(background.GlobalPosition.X + background.Scale.X);
			camera.LimitBottom = Mathf.RoundToInt(background.GlobalPosition.Y + background.Scale.Y);
			camera.LimitLeft = Mathf.RoundToInt(background.GlobalPosition.X);
			camera.LimitTop = Mathf.RoundToInt(background.GlobalPosition.Y);
		}
	}

	public static async void ReloadLevel() {
		AnimationPlayer transition = instance.GetNode<AnimationPlayer>("%Transition");
		transition.Play("Close");
		await instance.ToSignal(transition, AnimationPlayer.SignalName.AnimationFinished);

		instance.GetTree().ReloadCurrentScene();
	}
	
	/// <summary>
	/// Reloads the game scene with a new level
	/// </summary>
	/// <param name="levelName">The name of the new level to load</param>
	public static void LoadLevel(string levelName) {
		level_name = levelName;
		ReloadLevel();
	}
}