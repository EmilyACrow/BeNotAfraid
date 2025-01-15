using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public float speed = 5.0f;
	[Export]
	public float accel = 6f;
	[Export]
	public float deaccel = 8.5f;
	[Export]
	public  float jumpVelocity = 4.5f;
	[Export]
	public float airAccel = 2.0f;
	[Export]
	public float lookSensitivity = .002f;
	[Export]
	public bool moveEnabled;

	private Camera3D _camera;
	private Node3D _pivot;
	
	public override void _Ready()
	{
		moveEnabled = true;
		_pivot = GetNode<Node3D>("Pivot");
		Node gameManager = GetParent().GetParent().GetNode<Node>("GameManager");
		Callable gamePaused = new(this, nameof(OnGamePaused));
		gameManager.Connect("GamePaused", gamePaused);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (moveEnabled)
		{
			MoveAndSlide();
			Movement(delta);
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if ((@event is InputEventMouseMotion mouseMotion) && (Input.MouseMode == Input.MouseModeEnum.Captured)) {
			RotateY(-mouseMotion.Relative.X * lookSensitivity);
		}
	}

	private void OnCameraSwitched(float angle)
	{
		// Set the player's rotation to match the new camera's rotation
		RotateY(angle*-1);
	}

	private void OnGamePaused(bool paused)
	{
		moveEnabled = !paused;
	}

	private void Movement(double delta)
	{
		Vector3 velocity = Velocity;

		//Add the gravity.
		if(!IsOnFloor()) {
			velocity += GetGravity() * (float)delta * 2;
		}

		if (Input.IsActionJustPressed("jump")) {
			
			GD.Print(IsOnFloor());
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y += jumpVelocity;
		}

		//Vertical traverse
		// if (Input.IsActionPressed("jump"))
		// {
		// 	velocity.Y = speed;
		// }
		// else if (Input.IsActionPressed("crouch"))
		// {
		// 	velocity.Y = -speed * 1.1f;
		// }
		// else 
		// {
		// 	velocity.Y = Mathf.MoveToward(Velocity.Y, 0, speed);
		// }

		// Get the input direction and handle the movement/deceleration.

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (IsOnFloor()) {
			if (direction != Vector3.Zero)
			{
				velocity.X = Mathf.Lerp(Velocity.X, direction.X * speed, accel*(float)delta);
				velocity.Z = Mathf.Lerp(Velocity.Z, direction.Z * speed, accel*(float)delta);
			}
			else
			{
				velocity.X = Mathf.Lerp(Velocity.X, 0, deaccel*(float)delta);
				velocity.Z = Mathf.Lerp(Velocity.Z, 0, deaccel*(float)delta);
			}
		} 
		else {
			if (direction != Vector3.Zero)
			{
				velocity.X = Mathf.Lerp(Velocity.X, direction.X * speed, airAccel * (float)delta);
				velocity.Z = Mathf.Lerp(Velocity.Z, direction.Z * speed, airAccel * (float)delta);
			}
			else
			{
				velocity.X = Mathf.Lerp(Velocity.X, 0, deaccel * (float)delta);
				velocity.Z = Mathf.Lerp(Velocity.Z, 0, deaccel * (float)delta);
			}
		}
		// if (Input.IsActionJustPressed("ui_cancel"))
		// {
		// 	Input.MouseMode = (Input.MouseMode == Input.MouseModeEnum.Visible) ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
		// }

		Velocity = velocity;
	}

}
