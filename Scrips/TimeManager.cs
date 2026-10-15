using Godot;

public partial class TimeManager : Node
{

	[ExportGroup("References")]
	[Export] Label Timer_label;

	[ExportGroup("Timer")]
	[Export] float Time_to_beat = 5;

	private float Timer = 1;
	private bool Counting = false;

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("restart") || Timer <= 0)
		{
			GetTree().ReloadCurrentScene();
		}

		if(!Counting)
		{
			return;
		}

		Timer -= (float)delta;
		Timer_label.Text = Timer.ToString("0.00");
	}

	public void start_time(Node2D body)
	{
		if(body is PlayerMovement)
		{
			Counting = true;
			Timer = Time_to_beat;
		}
	}

	public void stop_time(Node2D body)
	{
		if(body is PlayerMovement)
		{
			Counting = false;
		}
	}
}
