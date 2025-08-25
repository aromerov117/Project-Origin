using Godot;

public partial class LampRigid : RigidBody3D
{
	[Export] public Area3D PickupArea;
	private bool playerInRange = false;

	public override void _Ready()
	{
		if (PickupArea != null)
		{
			PickupArea.BodyEntered += _on_PickupArea_body_entered;
			PickupArea.BodyExited += _on_PickupArea_body_exited;
		}
	}

	private void _on_PickupArea_body_entered(Node3D body)
	{
		if (body.IsInGroup("player"))
			playerInRange = true;
	}

	private void _on_PickupArea_body_exited(Node3D body)
	{
		if (body.IsInGroup("player"))
			playerInRange = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (playerInRange && Input.IsActionJustPressed("throw_lamp"))
		{
			// Buscar la linterna de mano en el grupo
			var lamps = GetTree().GetNodesInGroup("player_lamp");
			foreach (var node in lamps)
			{
				if (node is Lamp lamp)
				{
					lamp.PickUpFromRigid();
					break;
				}
			}

			QueueFree(); // destruir la linterna física
		}
	}
}
