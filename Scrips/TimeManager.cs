using Godot;

public partial class TimeManager : Node
{

	[ExportGroup("References")]
	[Export] Label Timer_label;

	private float Timer = 0;
	private bool Counting = false;

	public void Reset()
	{
		Counting = false;
		Timer = 0;
	}

	public void Begin()
	{
		Counting = true;
		Timer = 0;

		GetChild(0).QueueFree();
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("restart"))
		{
			GetTree().ReloadCurrentScene();
		}

		if(!Counting)
		{
			return;
		}
		
		Timer += (float)delta;
		Timer_label.Text = Timer.ToString("0.00");
	}

	public void start_time(Node2D body)
	{
		if(body is PlayerMovement)
		{
			Begin();
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
