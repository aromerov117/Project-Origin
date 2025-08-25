using Godot;

public partial class LampRigid : RigidBody3D
{
	public override void _PhysicsProcess(double delta)
	{
		// Siempre imprimimos para ver si el script corre
		GD.Print("LampRigid _PhysicsProcess corriendo");

		// Detectamos tecla pickup (R)
		if (Input.IsActionJustPressed("pickup_lamp"))
		{
			GD.Print("pickup_lamp presionada sobre linterna física");

			// Buscamos la linterna de mano
			var lamps = GetTree().GetNodesInGroup("player_lamp");
			GD.Print("Número de Lamp en grupo player_lamp: ", lamps.Count);

			foreach (var node in lamps)
			{
				if (node is Lamp lamp)
				{
					GD.Print("Encontré la linterna de mano, llamando PickUpFromRigid");
					lamp.PickUpFromRigid(this);
					break;
				}
			}
		}
	}
}
