using System;
using Godot;
public partial class DeathParticles : GpuParticles2D
{
	public override async void _Ready() {
		try {
			await ToSignal(this, GpuParticles3D.SignalName.Finished);
			QueueFree();
		}
		catch (Exception e) {
			GD.Print(e);
		}
	}
}