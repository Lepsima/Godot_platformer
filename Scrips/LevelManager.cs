using System.Text.Json;
using System.Collections.Generic;
using Godot;

namespace Platformer.Scrips;

public partial class LevelManager : Node {
	private static LevelManager instance { get; set; }
	private static string level_name;
	private const string time_save_path = "user://Saves/Times.json";

	[ExportGroup("Level paths")]
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
		Node level_instance = level_resource.Instantiate();
		AddChild(level_instance);

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
		//Starts the scene transition, and changes scenes once the animation is over
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

	public static void Submit_time(float time) {
		//Loads the timesheet
		Dictionary<string, float> saved_times = LoadSavedTimes();

		//Makes sure to never rewrite with a worse time
		if (!saved_times.TryGetValue(level_name, out float best_time) || time > best_time) {
			saved_times[level_name] = time;
		}

		//Writes the data back to the json file
		var json_options = new JsonSerializerOptions { WriteIndented = true };
		string updated_times = JsonSerializer.Serialize(saved_times, json_options);
		FileAccess file = FileAccess.Open(time_save_path, FileAccess.ModeFlags.Write);
		
		if (file == null) {
			return;
		}

		file.StoreString(updated_times);
		file.Close();
	}

	public static float Get_time_from_level(string level) {
		Dictionary<string, float> saved_times = LoadSavedTimes();
		return saved_times.TryGetValue(level, out float time) ? time : 0.0f;
	}

	//Loads the saved times as a dictionary
	private static Dictionary<string, float> LoadSavedTimes() {
		if (!FileAccess.FileExists(time_save_path))
		{
			return new Dictionary<string, float>();
		}

		string times_file = FileAccess.GetFileAsString(time_save_path);
		return JsonSerializer.Deserialize<Dictionary<string, float>>(times_file) ?? new Dictionary<string, float>();
	}
}