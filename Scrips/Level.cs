using System;
using Godot;

namespace Platformer.Scrips;

public partial class Level : Node2D {
	private static Level instance;
	private static float Time => Godot.Time.GetTicksMsec() / 1000.0f;
	
	[Export] private float time_to_beat = 5;
	
	private float deadline = -1;
	private float finish = -1;

	public static float GetRemainingTime() => instance?.remaining_time() ?? 0;
	
	public override void _EnterTree() {
		instance = this;
	}
	
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
	
	public void start_level(Node2D body) {
		if (body is not PlayerMovement) return;
		deadline = Time + time_to_beat;
		finish = -1;
	}

	public void finish_level(Node2D body) {
		if (body is not PlayerMovement) return;
		finish = Time;
	}
}