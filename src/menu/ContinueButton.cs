using Godot;

public class ContinueButton : Button
{
	public override void _Ready()
	{
		Connect("pressed", this,nameof(_on_ContinueButton_pressed));
	}

	private void _on_ContinueButton_pressed()
	{
		GetTree().ChangeScene("res://game/Game.tscn");
	}
}
