using System;
using Godot;

namespace Platformer.Scrips;

public partial class PlayerSpawnPoint : Node2D {
	private static PlayerSpawnPoint _instance;
	
	public static Vector2 GetSpawnPosition() {
		if (_instance == null) {
			throw new Exception("No PlayerSpawnPoint found");
		}
		
		return _instance.GlobalPosition;
	}

	// Deletes the old spawn point and sets this as the new one
	public override void _EnterTree() {
		base._EnterTree();
		_instance?.QueueFree();
		_instance = this;
	}

	// Unregisters this spawn point if it's still registered
	public override void _ExitTree() {
		base._ExitTree();
		if (_instance == this) _instance = null;
	}
}