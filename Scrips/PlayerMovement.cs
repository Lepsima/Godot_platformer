using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Platformer.Scrips;

public partial class PlayerMovement : CharacterBody2D
{
	enum PlayerState
	{
		Idle,
		Walking,
		Running,
		Attacking	
	};

	[ExportCategory("Movement")]
	[Export] float run_speed = 1500.0f;
	[Export] float ground_speed = 1500.0f;
	[Export] float air_speed = 500.0f;

	[Export] float run_drag = 6.0f;
	[Export] float ground_drag = 12.0f;
	[Export] float air_drag = 2.0f;

	float current_drag;
	float current_speed;
	bool is_running = false;
	bool is_climbing = false;

	[ExportCategory("Jumping")]
	[Export] float jump_velocity = -400.0f;
	[Export] float jump_sidewards_velocity = -100.0f;
	[Export] float gravity_scale_normal = 4;
	[Export] float gravity_scale_jumping = 1;

	[Export] int coyote_time_default = 5;
	int coyote_timer = 0;

	[ExportCategory("Dashing")]
	[Export] float dash_durration = 0.5f;
	[Export] float dash_length = 20f;
	bool is_dashing = false;

	[ExportCategory("References")]
	[Export] AnimatedSprite2D sprite;
	[Export] GpuParticles2D running_particles;
	[Export] RayCast2D wall_check;
	[Export] PackedScene death_particles;
	[Export] Label timer_label;

	[ExportCategory("State")]
	[Export] PlayerState state;

	public override void _Ready() {
		spawn();
	}

	public override void _Process(double delta)
	{
		float time = Level.GetRemainingTime();
		timer_label.Text = time.ToString("0.00");
	}

	public override void _PhysicsProcess(double delta)
	{
		if(is_dashing)
		{
			MoveAndSlide();
			return;
		}

		float direction = Input.GetAxis("left", "right");
		wall_check.TargetPosition = new Vector2(4 * direction, 0);
		
		is_climbing = Input.IsActionPressed("climb");
		is_running = Input.IsActionPressed("run");

		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * 
				(Input.IsActionPressed("jump") ? gravity_scale_jumping : gravity_scale_normal);
		}

		coyote_timer--;
		if(IsOnFloor())
		{
			coyote_timer = coyote_time_default;
		}

		if (Input.IsActionPressed("jump") && (coyote_timer > 0 || (wall_check.IsColliding() && is_climbing)))
		{
			velocity.Y = jump_velocity;
		
			if(wall_check.IsColliding() && is_climbing)
			{
				velocity.X = jump_sidewards_velocity * direction;
			}
		}

		if (Input.IsActionJustPressed("dash"))
		{
			Dash();
			return;
		}

		if(direction != 0)
		{
			sprite.FlipH = direction < 0;
		}

		//Movement state
		if(is_running)
		{
			if(coyote_timer <= 0)
			{
				current_speed = air_speed;
				current_drag = air_drag;
			}else
			{
				current_speed = run_speed;
				current_drag = run_drag;
			}
		}else
		{
			current_speed = ground_speed;
			current_drag = ground_drag;
		}

		velocity.X += direction * current_speed * (float)delta;
		velocity.X -= velocity.X * current_drag * (float)delta;

		Velocity = velocity;
		MoveAndSlide();

		for (int i = 0; GetSlideCollisionCount() > i; i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			Rid collider_rid = collision.GetColliderRid();

			uint collision_layer = PhysicsServer2D.BodyGetCollisionLayer(collider_rid);
			if ((collision_layer & (1 << 1)) == 0) continue;
			
			trigger_death();
			break;
		}

		if(state != PlayerState.Attacking) {
			if(Math.Abs(Velocity.X) >= 12) {
				state = PlayerState.Running;
			}
			else
			{
				state = PlayerState.Idle;
			}
		}

		running_particles.Emitting = state == PlayerState.Running && coyote_timer > 0 && is_running;
		running_particles.Scale = new Vector2(Velocity.X > 0 ? 1 : -1, 1);

		switch (state)
		{
			case PlayerState.Idle:
				sprite.Play("Idle");
				break;

			case PlayerState.Running:
				sprite.Play("Running");
				break;
			
			case PlayerState.Walking:
				sprite.Play("Running");
				break;
			
			case PlayerState.Attacking:
				sprite.Play("Attacking");
				break;
		};

	}

	private void trigger_death() {
		GpuParticles2D par = death_particles.Instantiate<GpuParticles2D>();
		par.GlobalPosition = GlobalPosition;
		par.Emitting = true;
		GetTree().CurrentScene.AddChild(par);
		spawn();
	}

	private void spawn() {
		try {
			Velocity = Vector2.Zero;
			GlobalPosition = PlayerSpawnPoint.GetSpawnPosition();
		}
		catch (Exception) {
			LevelManager.ReloadLevel();
		}
	}

	private async void Dash()
	{
		is_dashing = true;

		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if(direction.Length() != 0)
		{
			Velocity = Vector2.Zero;
			Velocity = new Vector2(direction.X >= 0 ? 1 : -1, 0);
			Velocity *= dash_length / dash_durration;

			await Task.Delay((int)(dash_durration * 1000));
		}

		is_dashing = false;
	}
}
