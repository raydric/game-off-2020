using Godot;

public class ScrapMetal : Area2D
{
	private const int Amount = 10;

	private float _movement;
	private bool _moveDown = true;

	public override void _Ready()
	{
		AddToGroup("collectibles");
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 direction;
		_movement += delta;	
		
		if (_moveDown)
		{
			direction = new Vector2(0, 1);
			if (_movement > 0.25)
			{
				_movement = 0;
				_moveDown = false;
			}
		}
		else
		{
			direction = new Vector2(0, -1);
			if (_movement > 0.25)
			{
				_movement = 0;
				_moveDown = true;
			}
		}
		
		var velocity = direction * 10;
		Position += velocity * delta;
	}

	public int Collect()
	{
		QueueFree();
		return Amount;
	}
}
