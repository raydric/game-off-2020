using Godot;

public class StartButton : Button
{
	public override void _Ready()
	{
		Connect("pressed", this,nameof(_on_StartButton_pressed));
	}

	private void _on_StartButton_pressed()
	{
		GetTree().ChangeScene("res://menu/Intro.tscn");
	}
}
