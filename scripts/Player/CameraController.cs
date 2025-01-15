using Godot;
using System.Collections.Generic;

public partial class CameraController : Node3D
{
	[Export]
	public float lookSensitivity = .002f;
	[Export]
	public float borderThickness = 15.0f;
	[Export]
	public Color borderColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	[Export]
	public float _lerpWeight = 0.005f;
	[Export]
	public float anchorSnapDistance = 0.0001f;
	private List<Camera3D> _cameras;
	private int _activeCamera = 0;
	private ColorRect _view;
	private ShaderMaterial _shaderMat;
	private List<Node3D> _eyes;
	private List<AnchorSet> _anchors;
	private AnchorSet _currentAnchorSet;
	private Node3D _parent;

	[Signal]
	public delegate void CameraSwitchedEventHandler(float angle);
	
	struct AnchorSet {
		public AnchorSet(Vector2 a, Vector2 b, Vector2 c, Vector2 o){
			anchorA = a;
			anchorB = b;
			anchorC = c;
			origin = o;
		}
		public AnchorSet(AnchorSet set) {
			anchorA = set.anchorA;
			anchorB = set.anchorB;
			anchorC = set.anchorC;
			origin = set.origin;
		}
		public Vector2 anchorA;
		public Vector2 anchorB;
		public Vector2 anchorC;
		public Vector2 origin;
	}

	public override void _Ready()
	{
		InitializeEyes();
		InitializeShader();

		Input.MouseMode = Input.MouseModeEnum.Captured;

		_parent = GetParent<Node3D>();
	}

	public override void _Input(InputEvent @event)
	{
		if ((@event is InputEventMouseMotion mouseMotion) && (Input.MouseMode == Input.MouseModeEnum.Captured)) 
		{
			// float rotationX = -mouseMotion.Relative.Y * lookSensitivity;
			// Transform3D transform = Transform;
			// transform.Basis = Basis.Identity;
			if(Input.IsActionPressed("free_look")){
				for(int i = 0; i < _eyes.Count; i++){
					if(i != _activeCamera)
					{
						float counterRotation = _eyes[i].Rotation.Y;
						float xRotation = _eyes[i].Rotation.X;
						Transform3D transform = _eyes[i].Transform;
						transform.Basis = Basis.Identity;
						_eyes[i].Transform = transform;

						_eyes[i].RotateObjectLocal(Vector3.Up, counterRotation + (mouseMotion.Relative.X * lookSensitivity));
						_eyes[i].RotateObjectLocal(Vector3.Right, xRotation);
					}
				}
			}
			_eyes[_activeCamera].RotateX(-mouseMotion.Relative.Y * lookSensitivity);
			_eyes[_activeCamera].Transform = _eyes[_activeCamera].Transform.Orthonormalized();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("switch_camera"))
		{
			Camera3D oldCam = _cameras[_activeCamera];
			_activeCamera = (_activeCamera + 1) % _cameras.Count;

			float angle = oldCam.Rotation.Y - _cameras[_activeCamera].Rotation.Y;

			foreach (Node3D eye in _eyes)
			{
				eye.RotateY(angle);
			}

			EmitSignal(SignalName.CameraSwitched, angle);
			
		}

		if (Input.IsActionJustPressed("switch_camera_back"))
		{
			Camera3D oldCam = _cameras[_activeCamera];
			_activeCamera = _activeCamera - 1;
			if (_activeCamera < 0)
			{
				_activeCamera = _cameras.Count - 1;
			}

			float angle = oldCam.Rotation.Y - _cameras[_activeCamera].Rotation.Y;

			foreach (Node3D eye in _eyes)
			{
				eye.RotateY(angle);
			}

			EmitSignal(SignalName.CameraSwitched, angle);
		}

		for (int i = 0; i < _eyes.Count; i++)
		{
			_cameras[i].GlobalTransform = _eyes[i].GlobalTransform;
		}
		
		UpdateShader(delta);
	}


	private void InitializeEyes()
	{
		GD.Print("Initializing Eyes");
		var eyeScene = ResourceLoader.Load<PackedScene>("res://scenes/Player/eye.tscn");
		_eyes = new List<Node3D>();
		_cameras = new List<Camera3D>();

		for (int i = 0; i < 3; i++)
		{
			Node3D eye = (Node3D)eyeScene.Instantiate();
			GetParent().GetNode<Node3D>("Pivot").AddChild(eye);
			_eyes.Add(eye);
			_cameras.Add(eye.GetNode<Camera3D>("Viewport/Camera"));
		}

		_view = GetNode<ColorRect>("View");
		
		//Rotate one of the cameras for testing purposes
		//_eyes[1].RotateY((float)GD.RandRange(-Mathf.Pi,Mathf.Pi));
	}

	private void InitializeShader()
	{
		_anchors = new List<AnchorSet>();
		AnchorSet anchorSet = new(
			new Vector2(0.6f, 0.0f),
			new Vector2(1.0f, 0.5f),
			new Vector2(0.6f, 1.0f),
			new Vector2(0.7f, 0.5f));
		_anchors.Add(anchorSet);
		anchorSet = new(
			new Vector2(0.2f, 0.0f),
			new Vector2(1.0f, 0.65f),
			new Vector2(0.2f, 1.0f),
			new Vector2(0.3f, 0.65f));
		_anchors.Add(anchorSet);
		anchorSet = new(
			new Vector2(0.2f, 0.0f),
			new Vector2(1.0f, 0.35f),
			new Vector2(0.2f, 1.0f),
			new Vector2(0.3f, 0.35f));
		_anchors.Add(anchorSet);
		_currentAnchorSet = new(_anchors[0]);
		GD.Print(_anchors[0].origin);
		GD.Print(_currentAnchorSet.origin);
		

		_shaderMat = (ShaderMaterial)_view.Material;
		_shaderMat.SetShaderParameter("viewportSize", new Vector2(_view.Size.X, _view.Size.Y));
		_shaderMat.SetShaderParameter("viewport1", _cameras[0].GetParent<Viewport>().GetTexture());
		_shaderMat.SetShaderParameter("viewport2", _cameras[1].GetParent<Viewport>().GetTexture());
		_shaderMat.SetShaderParameter("viewport3", _cameras[2].GetParent<Viewport>().GetTexture());
		_shaderMat.SetShaderParameter("borderThickness", borderThickness);
		_shaderMat.SetShaderParameter("borderLineColor", borderColor);
		_shaderMat.SetShaderParameter("anchorA", _anchors[0].anchorA);
		_shaderMat.SetShaderParameter("anchorB", _anchors[0].anchorB);
		_shaderMat.SetShaderParameter("anchorC", _anchors[0].anchorC);
		_shaderMat.SetShaderParameter("origin", _anchors[0].origin);

	}

	private void UpdateShader(double delta)
	{
		lerpParamAndUpdateShader(ref _currentAnchorSet.origin, _anchors[_activeCamera].origin, "origin");
		lerpParamAndUpdateShader(ref _currentAnchorSet.anchorA, _anchors[_activeCamera].anchorA, "anchorA");
		lerpParamAndUpdateShader(ref _currentAnchorSet.anchorB, _anchors[_activeCamera].anchorB, "anchorB");
		lerpParamAndUpdateShader(ref _currentAnchorSet.anchorC, _anchors[_activeCamera].anchorC, "anchorC");

	}

	private void lerpParamAndUpdateShader(ref Vector2 current, Vector2 target, string paramName) {
		if(current != target)
		{
			current = current.Lerp(target, _lerpWeight);
			if (current.DistanceTo(target) < anchorSnapDistance)
			{
				current = target;
			}
			_shaderMat.SetShaderParameter(paramName, current);
		}
	}
}
