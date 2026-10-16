using Godot;

namespace Platformer.Scrips;

public partial class LevelManager : Node {
	private static LevelManager instance { get; set; }
	private static string level_name;

	[Export] private string default_level = "default_level";
	
	public override void _Ready() {
		instance = this;
		
		level_name ??= default_level;
		string path = $"res://Scenes/Levels/{level_name}.tscn";

		if (!ResourceLoader.Exists(path)) return;
		PackedScene level_resource = GD.Load<PackedScene>(path);
		AddChild(level_resource.Instantiate());
	}

	public static void ReloadLevel() {
		instance.GetTree().ReloadCurrentScene();
	}

	public static void LoadLevel(string levelName) {
		level_name = levelName;
		ReloadLevel();
	}
}