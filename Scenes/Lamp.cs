using Godot;

public partial class Lamp : Node3D
{
	private SpotLight3D lampLight; // o OmniLight3D, depende lo que uses
	private bool isOn = false;

	public override void _Ready()
	{
		// Busca el nodo de luz dentro de la lámpara
		lampLight = GetNode<SpotLight3D>("SpotLight3D");
		lampLight.Visible = false; // inicia apagada
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("toggle_lamp"))
		{
			isOn = !isOn;
			lampLight.Visible = isOn;
		}
	}
}
