using Godot;

namespace Platformer.Scrips;

public partial class LevelManager : Node {
	private static LevelManager instance { get; set; }
	private static string level_name;

	[Export] private string default_level = "default_level";
	[Export] private string level_folder_path = "res://Scenes/Levels/";
	
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
	}
	
	/// <summary>
	/// Reloads the game scene with the same level
	/// </summary>
	public static void ReloadLevel() {
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