using Godot;
using System;

public partial class State : Node
{

	[Signal]
	public delegate void TransitionedEventHandler();
	public void Enter()
	{
		// Add your enter logic here
	}

	public void Exit()
	{
		// Add your exit logic here
	}

	public void Update(double delta)
	{
		// Add your update logic here
	}

	public void PhysicsUpdate(double delta)
	{
		// Add your physics update logic here
	}
}
