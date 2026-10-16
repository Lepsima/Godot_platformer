using System;
using Godot;
using Platformer.Scrips;
public partial class Trigger : Area2D
{
	[Export] private bool destroy_on_trigger = true;
	[Export] private bool load_level_on_trigger = false;
	[Export] private String level_path = "default_level";

	public void _on_body_entered(Node2D body)
	{
		if(body.Name == "Player")
		{
			if(load_level_on_trigger)
			{
				LevelManager.LoadLevel(level_path);
			}

			if(destroy_on_trigger)
			{
				QueueFree();
			}
		}
	}
}
