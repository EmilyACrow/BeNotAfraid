using Godot;
using System;

public partial class GameManager : Node
{
	private GameState _gameState;
	private CharacterBody3D _player;
	private Node _level;

	[Signal]
	public delegate void GamePausedEventHandler(bool paused);


	private enum GameState
	{
		Menu,
		Playing,
		Paused,
		GameOver
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameState = GameState.Menu;
		_level = GetParent().GetNode<Node>("Level");
		GetParent().GetNode<CanvasLayer>("PauseMenu").Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("esc"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
			if (_gameState == GameState.Menu)
			{
				GetTree().Quit();
			}
			else if (_gameState == GameState.Playing)
			{
				_gameState = GameState.Paused;
				EmitSignal("GamePaused", true);
				GetParent().GetNode<CanvasLayer>("PauseMenu").Show();
			}
			else if (_gameState == GameState.Paused)
			{
				_gameState = GameState.Playing;
				EmitSignal("GamePaused", false);
				GetParent().GetNode<CanvasLayer>("PauseMenu").Hide();
			}
		}
	}

	public void OnStartBtnPressed() {
		StartGame();
	}

	public void OnQuitBtnPressed() {
		GetTree().Quit();
	}

	public void OnRestartBtnPressed() {
		_player.QueueFree();
		GetParent().GetNode<CanvasLayer>("PauseMenu").Hide();
		Input.MouseMode = Input.MouseModeEnum.Captured;
		StartGame();
	}

	private void StartGame() {
		_gameState = GameState.Playing;
		_level.GetNode<ColorRect>("MenuShader").Show();
		SetupPlayer();
		GetParent().GetNode<CanvasLayer>("StartMenu").Hide();		
	}

	private void SetupMainMenu() {
		GetParent().GetNode<CanvasLayer>("StartMenu").Show();
		ShaderMaterial shaderMat = (ShaderMaterial)_level.GetNode<ColorRect>("MenuShader").Material;
		shaderMat.SetShaderParameter("viewport", _level.GetNode<Viewport>("MenuCamViewport/SubViewport").GetTexture());
	}

	private void SetupPlayer() {
		PackedScene playerScene = GD.Load<PackedScene>("res://scenes/Player/player.tscn");
		_player = (CharacterBody3D)playerScene.Instantiate();
		_level.AddChild(_player);
		_player.Transform = GetParent().GetNode<Node3D>("Level/PlayerSpawn").Transform;
		_player.LookAt(_level.GetNode<Node3D>("Campfire").Position);
	}
}
