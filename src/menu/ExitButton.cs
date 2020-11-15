using Godot;

public class ExitButton : Button
{
	public override void _Ready()
	{
		Connect("pressed", this,nameof(_on_ExitButton_pressed));
	}

	private void _on_ExitButton_pressed()
	{
		GetTree().Quit();
	}
}
