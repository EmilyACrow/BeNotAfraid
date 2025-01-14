using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public float speed = 5.0f;
	[Export]
	public  float jumpVelocity = 4.5f;
	[Export]
	public float lookSensitivity = .002f;
	private Camera3D _camera;
	private Node3D _pivot;
	
	public override void _Ready()
	{
		_pivot = GetNode<Node3D>("Pivot");
	}

	public override void _PhysicsProcess(double delta)
	{
		Movement(delta);
		MoveAndSlide();
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

	private void Movement(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		// if (!IsOnFloor())
		// {
		// 	velocity += GetGravity() * (float)delta;
		// }

		// // Handle Jump.
		// if (Input.IsActionJustPressed("jump") && IsOnFloor())
		// {
		// 	velocity.Y = jumpVelocity;
		// }

		//Vertical traverse
		if (Input.IsActionPressed("jump"))
		{
			velocity.Y = speed;
		}
		else if (Input.IsActionPressed("crouch"))
		{
			velocity.Y = -speed;
		}
		else 
		{
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, speed);
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
		}
		
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = (Input.MouseMode == Input.MouseModeEnum.Visible) ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
		}

		Velocity = velocity;
	}

}
