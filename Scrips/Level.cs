using System;
using Godot;

namespace Platformer.Scrips;

public partial class Level : Node2D {
	private static Level instance;
	private static float Time => Godot.Time.GetTicksMsec() / 1000.0f;
	
	[ExportGroup("Timing")]
	[Export] private float time_to_beat = 5;
	private float deadline = -1;
	private float finish = -1;

	/// <summary>
	/// Get the remaining time to beat the current active level
	/// </summary>
	public static float GetRemainingTime() => instance?.remaining_time() ?? 0;
	
	[ExportGroup("Camera limits")]
	[Export] public int left_limit = -9999;
	[Export] public int top_limit = 9999;
	[Export] public int right_limit = 9999;
	[Export] public int bottom_limit = -9999;

	public override void _EnterTree() {
		instance = this;
	}
	
	// Check if the level should be reloaded
	public override void _Process(double delta) {
		if(Input.IsActionJustPressed("restart") || is_overtime()) {
			LevelManager.ReloadLevel();
		}
	}

	private bool has_finished() {
		return finish >= 0;
	}
	
	private float remaining_time() {
		float timer = has_finished() ? finish : Time;
		return Math.Clamp(deadline - timer, 0, time_to_beat);
	}
	
	private bool is_overtime() {
		return deadline >= 0 && remaining_time() <= 0;
	}
	
	/// <summary>
	/// Call this function to start the level timer
	/// If the timer ends before <see cref="finish_level"/> is called, the level reloads.
	/// </summary>
	public void start_level(Node2D body) {
		if (body is not PlayerMovement) return;
		deadline = Time + time_to_beat;
		finish = -1;
	}

	/// <summary>
	/// Called to stop the level timer.
	/// </summary>
	public void finish_level(Node2D body) {
		if (body is not PlayerMovement) return;
		finish = Time;
		LevelManager.Submit_time(deadline - finish);
	}
}
