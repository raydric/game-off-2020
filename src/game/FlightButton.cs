using Godot;

public class FlightButton : Button
{
	public override void _Ready()
	{
		Connect("pressed", this,nameof(_on_FlightButton_pressed));
	}

	private void _on_FlightButton_pressed()
	{
		var game = GetTree().CurrentScene.GetNode<Game>(".");
		game.ChangeGameState(GameState.Start);
		QueueFree();
	}
}
