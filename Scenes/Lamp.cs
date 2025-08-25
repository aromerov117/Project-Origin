using Godot;

public partial class Lamp : Node3D
{
	[Export] public SpotLight3D LampLight;       // luz de la linterna
	[Export] public MeshInstance3D LampMesh;     // mesh visible
	[Export] public PackedScene ThrowLampScene;  // linterna física (RigidBody3D)
	[Export] public Node3D Hand;                 // nodo donde se pega la linterna (del jugador)

	private bool isOn = false;     // estado de la luz
	private bool isHeld = true;    // true si está en la mano

	public override void _Ready()
	{
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
			ToggleLight();

		// Tirar linterna
		if (Input.IsActionJustPressed("throw_lamp") && isHeld)
			ThrowLamp();
	}

	private void ToggleLight()
	{
		isOn = !isOn;
		if (LampLight != null)
			LampLight.Visible = isOn;
	}

	private void ThrowLamp()
	{
		if (ThrowLampScene == null)
		{
			GD.PrintErr("No se ha asignado ThrowLampScene.");
			return;
		}

		// Instanciar linterna física
		var lampInstance = (RigidBody3D)ThrowLampScene.Instantiate();

		// Posición frente al jugador
		lampInstance.GlobalTransform = Hand.GlobalTransform;
		lampInstance.Translate(-Hand.GlobalTransform.Basis.Z * 2.0f); // 2 metros adelante

		// Copiar estado de la luz
		var lampLightInInstance = lampInstance.GetNodeOrNull<SpotLight3D>("SpotLight3D");
		if (lampLightInInstance != null)
			lampLightInInstance.Visible = isOn;

		// Agregar al mundo
		GetTree().CurrentScene.AddChild(lampInstance);

		// Aplicar impulso hacia adelante
		lampInstance.ApplyCentralImpulse(-Hand.GlobalTransform.Basis.Z * 5.0f);

		// Ocultar linterna de la mano
		isHeld = false;
		Visible = false;

		GD.Print("Linterna lanzada hacia adelante.");
	}

	// Cuando la linterna física se recoge, se llama esto para volver a la mano
	public void PickUpFromRigid()
	{
		isHeld = true;
		Visible = true;
		GlobalTransform = Hand.GlobalTransform;

		GD.Print("Linterna recogida y de vuelta en la mano.");
	}
}
