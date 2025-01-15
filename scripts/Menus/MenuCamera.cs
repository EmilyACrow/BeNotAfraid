using Godot;
using System;

public partial class MenuCamera : SubViewport
{
	[Export]
	public float rotationSpeed = 0.01f;
	private Camera3D _camera;
	private Node3D _camPivot;
	private bool _isPlaying;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Pivot/Camera");
		_camPivot = GetNode<Node3D>("Pivot");
		_isPlaying = true;

		_camera.LookAt(_camPivot.Position);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isPlaying)
		{
			_camPivot.RotateY(rotationSpeed * (float)delta);
		}
	}

	public void OnStartBtnPressed()
	{
		_isPlaying = false;
		_camera.Current = false;
	}
}
