using Godot;
using System;

public partial class Player : CharacterBody3D
{
	// ----- Ajustes de movimiento -----
	[Export] public float Speed = 6.0f;
	[Export] public float SprintMultiplier = 1.6f;
	[Export] public float JumpVelocity = 5.5f;

	// ----- Cámara / ratón -----
	[Export] public NodePath PivotPath;     // arrastra el nodo Pivot aquí desde el editor
	[Export] public float MouseSensitivity = 0.15f;

	private Node3D _pivot;
	private float _pitch = 0f;  // rotación vertical
	private float _gravity = 9.8f; // puedes ajustar o leer de ProjectSettings

	public override void _Ready()
	{
		// Capturar el mouse para cámara tipo tercera persona
		Input.MouseMode = Input.MouseModeEnum.Captured;

		if (PivotPath != null && !PivotPath.IsEmpty)
			_pivot = GetNode<Node3D>(PivotPath);
		else
			_pivot = GetNode<Node3D>("Pivot"); // fallback si dejaste el nombre por defecto
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			// Girar horizontalmente al Player (yaw)
			RotateY(Mathf.DegToRad(-motion.Relative.X * MouseSensitivity));

			// Girar verticalmente el pivot (pitch)
			_pitch = Mathf.Clamp(_pitch - motion.Relative.Y * MouseSensitivity, -80f, 80f);
			_pivot.RotationDegrees = new Vector3(_pitch, _pivot.RotationDegrees.Y, _pivot.RotationDegrees.Z);
		}

		// Liberar/capturar el mouse con Escape
		if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
				? Input.MouseModeEnum.Visible
				: Input.MouseModeEnum.Captured;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Gravedad
		if (!IsOnFloor())
			velocity.Y -= _gravity * (float)delta;

		// Dirección en XZ basada en input
		Vector3 direction = Vector3.Zero;

		if (Input.IsActionPressed("move_forward")) direction -= Transform.Basis.Z;
		if (Input.IsActionPressed("move_back"))    direction += Transform.Basis.Z;
		if (Input.IsActionPressed("move_left"))    direction -= Transform.Basis.X;
		if (Input.IsActionPressed("move_right"))   direction += Transform.Basis.X;

		direction = direction.Normalized();

		float currentSpeed = Speed * (Input.IsActionPressed("sprint") ? SprintMultiplier : 1f);

		// Aplicar movimiento horizontal
		velocity.X = direction.X * currentSpeed;
		velocity.Z = direction.Z * currentSpeed;

		// Salto
		if (IsOnFloor() && Input.IsActionJustPressed("jump"))
			velocity.Y = JumpVelocity;

		Velocity = velocity;
		MoveAndSlide();
	}
}
