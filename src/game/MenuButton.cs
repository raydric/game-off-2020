using Godot;

public class MenuButton : Button
{
	public override void _Ready()
	{
		Connect("pressed", this,nameof(_on_MenuButton_pressed));
	}

	private void _on_MenuButton_pressed()
	{
		GetTree().ChangeScene("res://menu/Menu.tscn");
	}
}
