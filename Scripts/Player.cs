using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float WalkSpeed { get; set; } = 5.0f;
	[Export] public float RunSpeed { get; set; } = 10.0f;
	[Export] public float JumpVelocity { get; set; } = 10.0f;
	[Export] public float Gravity { get; set; } = 20.0f;
	[Export] public float MouseSensitivity { get; set; } = 0.0025f;

	private Camera3D _camera;
	private float _yaw = 0.0f;
	private float _pitch = 0.0f;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			_yaw -= motion.Relative.X * MouseSensitivity;
			_pitch -= motion.Relative.Y * MouseSensitivity;
			_pitch = Mathf.Clamp(_pitch, -1.2f, 1.2f);

			Rotation = new Vector3(0, _yaw, 0);
			_camera.Rotation = new Vector3(_pitch, 0, 0);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		// Gravedad
		if (!IsOnFloor())
		{
			velocity.Y -= Gravity * (float)delta;
		}
		else if (Input.IsActionJustPressed("jump"))
		{
			velocity.Y = JumpVelocity;
		}

		// Dirección
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		float speed = Input.IsActionPressed("run") ? RunSpeed : WalkSpeed;

		velocity.X = direction.X * speed;
		velocity.Z = direction.Z * speed;

		Velocity = velocity;
		MoveAndSlide();
	}
}
