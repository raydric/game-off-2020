using Godot;

public class Missile : Area2D
{
	private const float Speed = 800;
	private int _damage = 10;

	public override void _Ready()
	{
		Connect("area_entered", this, nameof(_on_Bullet_area_entered));

		GetNode("VisibilityNotifier2D").Connect("screen_exited", this,
			nameof(_on_VisibilityNotifier2D_screen_exited));
	}

	public override void _PhysicsProcess(float delta)
	{
		var velocity = new Vector2(1, 0) * Speed;
		Position += velocity * delta;
	}

	public void SetDamage(int damage)
	{
		_damage = damage;
	}

	private void _on_VisibilityNotifier2D_screen_exited()
	{
		QueueFree();
	}

	private void _on_Bullet_area_entered(Area2D area)
	{
		if (area.IsInGroup("enemies"))
		{
			var enemy = area as Enemy;
			enemy?.TakeDamage(_damage);
			QueueFree();
		}
	}
}
