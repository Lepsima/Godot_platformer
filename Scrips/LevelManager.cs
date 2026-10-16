using System;
using System.Diagnostics;
using Godot;

namespace Platformer.Scrips;

public partial class LevelManager : Node {
	private static LevelManager instance { get; set; }
	private static string level_name;

	[Export] private string default_level = "default_level";

	private Camera2D camera;
	
	public override void _Ready() {
		instance = this;
		
		level_name ??= default_level;
		string path = $"res://Scenes/Levels/{level_name}.tscn";

		if (!ResourceLoader.Exists(path)) return;
		PackedScene level_resource = GD.Load<PackedScene>(path);
		
		Node level_instance = level_resource.Instantiate();
		AddChild(level_instance);

		camera = GetViewport().GetCamera2D();
		if(camera != null)
		{
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

	public static void LoadLevel(string levelName) {
		level_name = levelName;
		ReloadLevel();
	}
}