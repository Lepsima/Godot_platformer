using Godot;
public partial class DeathParticles : GpuParticles2D
{
	public override async void _Ready()
	{
		await ToSignal(this, GpuParticles3D.SignalName.Finished);
        this.QueueFree();
	}
}