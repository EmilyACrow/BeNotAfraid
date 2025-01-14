using Godot;
using System;

public partial class Eye : Node3D
{
    public float lightIntensity = 1.0f;

    private SpotLight3D _light;
    private Camera3D _camera;

    public override void _Ready()
    {
        _light = GetNode<SpotLight3D>("Viewport/Camera/SpotLight3D");
        _camera = GetNode<Camera3D>("Viewport/Camera");
    }

    public void OnDestroyed()
    {
        _light.Visible = false;
        _camera.Current = false;
    }
}
