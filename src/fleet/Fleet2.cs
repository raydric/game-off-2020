using Godot;

public class Fleet2 : Node2D
{
	private float _movement;
	private bool _moveDown;
	
	private RandomNumberGenerator _rng;

	public override void _Ready()
	{
		AddToGroup("fleet");
		
		_rng = new RandomNumberGenerator();
		_rng.Randomize();

		if (_rng.RandiRange(0, 1) == 1)
		{
			_moveDown = true;
		}
		else
		{
			_moveDown = false;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 direction;
		_movement += delta;	
		
		if (_moveDown)
		{
			direction = new Vector2(0, 1);
			if (_movement > 2)
			{
				_movement = 0;
				_moveDown = false;
			}
		}
		else
		{
			direction = new Vector2(0, -1);
			if (_movement > 2)
			{
				_movement = 0;
				_moveDown = true;
			}
		}
		
		var velocity = direction * 5;
		Position += velocity * delta;
	}
}
