using Godot;
using System;

public partial class AiManager : Node
{
    private List<Node3D> _zones;
    private Node _level;

    public override void _Ready() {
        zones = new List<Zones>();
        _level = GetParent().GetNode<Node>("Level");
        foreach(Node3D zone in _level.GetNode("Zones/AiZones"))
        {
            _zones.Add(zone);
        }
    }


    public Node3D GetNewNavPoint(Node3D currentZone, Node3D weightedZone = null, float weight = 0.0)
    {
        Node3D zone;
        if (_zones.Contains(currentZone)) {
            if (_zones.Contains(weightedZone)) {
                return new Node3D();
            }
        }
    }
}
