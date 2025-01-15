using Godot;
using System;
using System.Collections.Generic;
// FOllowing this tutorial: https://www.youtube.com/watch?v=ow_Lum-Agbs

public partial class StateMachine : Node
{
	[Export]
	public State initialState;
	private Dictionary<string, Node> _states;
	private State _currentState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child.GetType() == typeof(State)) {
				_states.Add(child.Name, child);
				child.Connect("Transitioned", new Callable(this, nameof(OnTransition)));
			}
		}

		if (initialState != null) {
			_currentState = (State)initialState;
			_currentState.Enter();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_currentState != null) {
			_currentState.Update(delta);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_currentState != null) {
			_currentState.PhysicsUpdate(delta);
		}
	}

	private void OnTransition(string state, string newStateName)
	{
		if (_states.ContainsKey(newStateName)) {
			_currentState.Exit();
			_currentState = (State)_states[newStateName];
			_currentState.Enter();
		}
	}
}
