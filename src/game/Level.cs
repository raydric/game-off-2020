using Godot;

public class Level : Node2D
{
	private int _enemyHealth = 30;
	private int _enemyDamage = 10;
	
	private readonly PackedScene _enemyScene = ResourceLoader.Load("res://enemy/Enemy.tscn") as PackedScene;
	private Timer _spawnTimer;
	private Timer _levelTimer;
	private RandomNumberGenerator _rng;
	private bool _isRunning;

	public override void _Ready()
	{
		_rng = new RandomNumberGenerator();
		_rng.Randomize();
		
		_spawnTimer = new Timer();
		AddChild(_spawnTimer);
		_spawnTimer.Connect("timeout", this, nameof(_on_Timer_timeout));
		_spawnTimer.WaitTime = 1;
		_spawnTimer.OneShot = true;
		
		_levelTimer = new Timer();
		AddChild(_levelTimer);
		_levelTimer.Connect("timeout", this, nameof(_on_Timer_timeout_level));
		_levelTimer.WaitTime = 30;
		_levelTimer.OneShot = true;
	}

	public bool IsRunning()
	{
		return _isRunning;
	}
	
	public void Start()
	{
		_isRunning = true;
		_spawnTimer.Start();
		_levelTimer.Start();
	}

	public void IncreaseDifficulty()
	{
		_enemyDamage += 5;
		_enemyHealth += 5;
	}

	private void _on_Timer_timeout()
	{
		_spawnTimer.WaitTime = _rng.RandfRange(2, 6);
		_spawnTimer.Start();
		
		if (_enemyScene.Instance() is Enemy enemy)
		{
			enemy.Health = _enemyHealth;
			enemy.Damage = _enemyDamage;
			enemy.GlobalPosition = new Vector2(1300, _rng.RandfRange(100, 700));
			AddChild(enemy);
		}
	}
	
	private async void _on_Timer_timeout_level()
	{
		_spawnTimer.Stop();

		var children = GetChildren(); ;
		
		foreach (var child in children)
		{
			if (child is Enemy enemy)
			{
				enemy.WarpAway();
			}
		}
		
		var timer = new Timer {WaitTime = 5, OneShot = true};
		AddChild(timer);
		timer.Start();
		await ToSignal(timer, "timeout");
		timer.QueueFree();

		_isRunning = false;
	}
}
