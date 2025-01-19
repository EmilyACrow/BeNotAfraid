using Godot;
using System;

public partial class PursueState : State
{
    [Export]
    public float grabRange = 5.0f;

    private Node3D _target;
    private Node3D _lastKnownLocation;
    private Character3D _enemyBody;
    private Enemy _enemy;
    private PursueValues _vals;

    public struct PursueValues
    {
        Node3D target;
        Node3D lastKnown;
    }

    public override _Ready() 
    {
        _enemyBody = GetParent().GetParent().GetNode("Character");
        _enemy = GetParent().GetParent().GetNode("Character");
    }

    public override Enter() 
    {
        //Set target & lastKnown
    }

    public override _PhysicsProcess(double delta)
    {
        Vector3 direction = new Vector3(_target.GlobalPosition() - _enemy.GlobalPosition()).normalized();
        float distance = _enemy.GlobalPosition().DistanceTo(_target.GlobalPosition());

        if (distance < grabRange)
        {
            _enemy.TakeItem(_target);
            EmitSignal("Pursue", "Wander", _enemy.GetOriginZone());
        }
    }
}
