using Godot;

public class Enemy : Area2D
{
	private const float Speed = 100;
	private int _health;
	private int _damage;
	
	private const float BulletCooldown = 3f;

	// INTERNAL
	private readonly PackedScene _scrapScene = ResourceLoader.Load("res://scrapmetal/ScrapMetal.tscn") as PackedScene;
	private readonly PackedScene _bulletScene = ResourceLoader.Load("res://bullet/EnemyBullet.tscn") as PackedScene;
	private readonly PackedScene _explosionScene = ResourceLoader.Load("res://explosion/Explosion.tscn") as PackedScene;
	private readonly PackedScene _warpScene = ResourceLoader.Load("res://warp/Warp.tscn") as PackedScene;

	private Node2D _gunPosition;
	private AudioStreamPlayer _impactSound;
	private AudioStreamPlayer _gunSound;

	private Timer _bulletTimer;

	public int Health
	{
		set => _health = value;
	}

	public int Damage
	{
		set => _damage = value;
	}

	public override void _Ready()
	{
		AddToGroup("enemies");

		_gunPosition = GetNode<Node2D>("FiringPositions/Gun");
		_impactSound = GetNode<AudioStreamPlayer>("ImpactSound");
		_gunSound = GetNode<AudioStreamPlayer>("GunSound");

		GetNode("VisibilityNotifier2D").Connect("screen_exited", this,
			nameof(_on_VisibilityNotifier2D_screen_exited));

		_bulletTimer = new Timer();
		AddChild(_bulletTimer);

		_bulletTimer.Connect("timeout", this, nameof(_on_Timer_timeout));
		_bulletTimer.WaitTime = BulletCooldown;
		_bulletTimer.OneShot = false;
		_bulletTimer.Start();
	}

	public override void _PhysicsProcess(float delta)
	{
		var velocity = new Vector2(-1, 0) * Speed;
		Position += velocity * delta;
	}

	public void TakeDamage(int damage)
	{
		_impactSound.Play();
		_health -= damage;
		if (_health <= 0)
		{
			if (_scrapScene.Instance() is ScrapMetal scrapMetal)
			{
				scrapMetal.Position = Position;
				GetTree().CurrentScene.CallDeferred("add_child", scrapMetal);
			}
			
			if (_explosionScene.Instance() is Explosion explosion)
			{
				explosion.Position = Position;
				GetTree().CurrentScene.CallDeferred("add_child", explosion);
			}
			
			QueueFree();
		}
	}

	public void WarpAway()
	{
		if (_warpScene.Instance() is Warp warp)
		{
			warp.Position = Position;
			GetTree().CurrentScene.CallDeferred("add_child", warp);
		}
		
		QueueFree();
	}

	private void _on_VisibilityNotifier2D_screen_exited()
	{
		if (Position.x < 0)
		{

			var player = GetTree().CurrentScene.GetNodeOrNull<Player>("Player");
			player?.TakeFleetDamage(_damage);
		}

		QueueFree();
	}

	private void _on_Timer_timeout()
	{
		if (Position.x > 200)
		{
			if (_bulletScene.Instance() is EnemyBullet bullet)
			{
				_gunSound.Play();
				GetTree().CurrentScene.AddChild(bullet);
				bullet.SetDamage(_damage);
				bullet.GlobalPosition = _gunPosition.GlobalPosition;
			}
		}
	}
}
