using Godot;

public partial class Lamp : Node3D
{
	[Export] public SpotLight3D LampLight;
	[Export] public MeshInstance3D LampMesh;
	[Export] public PackedScene ThrowLampScene;
	[Export] public Node3D Hand;

	private bool isOn = false;
	private bool isHeld = true;

	public override void _Ready()
	{
		AddToGroup("player_lamp");
		GD.Print("Lamp _Ready: Linterna agregada al grupo player_lamp");

		if (LampLight == null)
			LampLight = GetNodeOrNull<SpotLight3D>("SpotLight3D");
		if (LampMesh == null)
			LampMesh = GetNodeOrNull<MeshInstance3D>("LampMesh");

		if (LampLight != null)
			LampLight.Visible = isOn;
	}

	public override void _Process(double delta)
	{
		// Encender/apagar linterna
		if (Input.IsActionJustPressed("toggle_lamp") && isHeld)
		{
			GD.Print("Se presionó toggle_lamp");
			ToggleLight();
		}

		// Tirar linterna
		if (Input.IsActionJustPressed("throw_lamp") && isHeld)
		{
			GD.Print("Se presionó throw_lamp");
			ThrowLamp();
		}

		// Debug para ver el estado
		if (Input.IsActionJustPressed("pickup_lamp"))
		{
			GD.Print("Se presionó pickup_lamp en Lamp (estado isHeld=", isHeld, ")");
		}
	}

	private void ToggleLight()
	{
		isOn = !isOn;
		if (LampLight != null)
			LampLight.Visible = isOn;
		GD.Print("ToggleLight: isOn=", isOn);
	}

	private void ThrowLamp()
	{
		if (ThrowLampScene == null)
		{
			GD.PrintErr("No se ha asignado ThrowLampScene.");
			return;
		}

		var lampInstance = (RigidBody3D)ThrowLampScene.Instantiate();
		lampInstance.GlobalTransform = Hand.GlobalTransform;
		lampInstance.Translate(-Hand.GlobalTransform.Basis.Z * 2.0f);

		var lampLightInInstance = lampInstance.GetNodeOrNull<SpotLight3D>("SpotLight3D");
		if (lampLightInInstance != null)
			lampLightInInstance.Visible = isOn;

		GetTree().CurrentScene.AddChild(lampInstance);
		lampInstance.ApplyCentralImpulse(-Hand.GlobalTransform.Basis.Z * 5.0f);

		isHeld = false;
		Visible = false;

		GD.Print("ThrowLamp: Linterna lanzada, isHeld=", isHeld);
	}

	public void PickUpFromRigid(RigidBody3D rigid)
	{
		GD.Print("PickUpFromRigid: Iniciando recogida de linterna");
		rigid.QueueFree();
		GD.Print("PickUpFromRigid: Linterna física destruida");

		isHeld = true;
		Visible = true;
		GlobalTransform = Hand.GlobalTransform;

		GD.Print("PickUpFromRigid: Linterna de vuelta en la mano, isHeld=", isHeld);
	}
}
