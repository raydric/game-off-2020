using Godot;

public class GameLevel : Node2D
{
	private Label _levelNumber;

	public override void _Ready()
	{
		_levelNumber = GetParent().GetNode<Label>("GameLevel/HBoxContainer/LevelNumberLabel");
	}

	public void SetLevel(int level)
	{
		_levelNumber.Text = level.ToString();
	}
	
}
