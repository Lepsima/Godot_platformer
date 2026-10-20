using Godot;
using Platformer.Scrips;
using System;

public partial class TimeLabel : Label
{
	[Export] String level = "default_level";
	public override void _Ready()
	{
		float time = LevelManager.Get_time_from_level(level);
		Text = time.ToString("0.00");
	}
}
