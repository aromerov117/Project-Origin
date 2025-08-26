using Godot;

public partial class LampRigid : RigidBody3D
{
	private Area3D PickupArea;
	private bool playerInRange = false;

	public override void _Ready()
	{
		// Busca el Area3D hijo
		PickupArea = GetNode<Area3D>("PickupArea");

		if (PickupArea != null)
		{
			PickupArea.BodyEntered += OnBodyEntered;
			PickupArea.BodyExited += OnBodyExited;
		}
		else
		{
			GD.PrintErr("❌ No se encontró PickupArea en LampRigid");
		}
	}

	private void OnBodyEntered(Node3D body)
	{
		// Revisa si el cuerpo que entró es el jugador
		if (body is CharacterBody3D)
		{
			GD.Print("✅ Jugador entró al área de recogida");
			playerInRange = true;
		}
		else
		{
			GD.Print($"➡ Otro cuerpo entró al área: {body.Name}");
		}
	}

	private void OnBodyExited(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			GD.Print("⛔ Jugador salió del área de recogida");
			playerInRange = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (playerInRange && Input.IsActionJustPressed("pickup_lamp"))
		{
			GD.Print("🎮 pickup_lamp presionada dentro del área");

			var lamps = GetTree().GetNodesInGroup("player_lamp");
			foreach (var node in lamps)
			{
				if (node is Lamp lamp)
				{
					GD.Print("🔦 Encontré la linterna de mano, llamando PickUpFromRigid");
					lamp.PickUpFromRigid(this);
					break;
				}
			}
		}
	}
}
