using Godot;
using System;

public partial class WanderState : State
{
    private Node3D _navPoint;
    private float _wanderTime;
    private float _viewDistance = 50.0f;
    private Enemy _enemy;
    private bool _hasItem;

    public struct WanderValues {
        float viewDistance;
        float wanderTime;
        bool hasItem;
    }

    public override void _Ready()
    {
        _enemy = GetParent().GetParent().GetNode("Character");
        WanderValues wv = _enemy.GetWanderValues();
        _wanderTime = wv.wanderTime;
        _viewDistance = wv.viewDistance;
        _hasItem = wv.hasItem;
    }

    public override void Enter() 
    {
        _navPoint = GetNewNavPoint()
    }

    public override void _Process(double delta) 
    {
        bool targetSeen = _enemy.CheckLOS() != null;
        if(_hasItem || !targetSeen) 
        {
            if (_wanderTime > 0.0) {
                _wanderTime -= (float)delta;
            }
            else 
            {
                _navPoint = GetNewNavPoint();
            }

            return;
        }

        //Transition to Pursue
        
    }

    public override void _PhysicsProcess(double delta) 
    {
        //Set velocity towards nav point
    }

    private Node3D GetNewNavPoint() 
    {
        //Get new nav point from AiManager
        return new Node3D();
    }

    private void OnFreeze()
    {
        //Immediately transition to freeze state
    }
}
