using Godot;
public partial class Trigger : Area2D
{
	[Export] private bool destroy_on_trigger = true;

	public void _on_body_entered(Node2D body)
	{
		if(body.Name == "Player" && destroy_on_trigger)
		{
			QueueFree();
		}
	}
}
