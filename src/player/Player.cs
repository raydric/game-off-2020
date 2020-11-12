using System;
using Godot;

public class Player : Area2D
{
	// PLAYER VARIABLES
	private int _health = 100;
	private int _maxHealth = 100;
	private int _fleetHealth = 300;
	private int _maxFleetHealth = 300;
	private int _speed = 200;
	
	private int _gunDamage = 10;
	private float _gunCooldown = 0.5f;
	
	private int _missileDamage = 50;
	private float _missileCooldown = 5f;

	private int _metalScrap;
	
	// PUBLIC 
	public int Health => _health;
	public int MaxHealth => _maxHealth;
	public int FleetHealth => _fleetHealth;
	public int MaxFleetHealth => _maxFleetHealth;
	public int Speed => _speed;
	public int GunDamage => _gunDamage;
	public float GunCooldown => _gunCooldown;
	public int MissileDamage => _missileDamage;
	public float MissileCooldown => _missileCooldown;
	public int MetalScrap => _metalScrap;
	
	// STATE
	private bool _active;
	private float _movement;
	private bool _moveDown = true;
	
	// INTERNAL
	private readonly PackedScene _bulletScene = ResourceLoader.Load("res://bullet/Bullet.tscn") as PackedScene;
	private readonly PackedScene _missileScene = ResourceLoader.Load("res://missile/Missile.tscn") as PackedScene;
	private readonly PackedScene _explosionScene = ResourceLoader.Load("res://explosion/Explosion.tscn") as PackedScene;
	
	private Timer _gunCooldownTimer;
	private Timer _missileCooldownTimer;
	
	private Node2D _gunPosition;
	private Node2D _missilePosition;
	
	private HUD _hud;

	public override void _Ready()
	{
		AddToGroup("player");
		
		_gunPosition = GetParent().GetNode<Node2D>("Player/FiringPositions/Gun");
		_missilePosition = GetParent().GetNode<Node2D>("Player/FiringPositions/Missile");
		_gunCooldownTimer = GetParent().GetNode<Timer>("Player/GunCooldown");
		_missileCooldownTimer = GetParent().GetNode<Timer>("Player/MissileCooldown");
		
		Connect("area_entered", this,nameof(_on_Player_area_entered));
	}

	public override void _Process(float delta)
	{
		if (!_active) return;

		if (Input.IsKeyPressed((int) KeyList.Space) && _gunCooldownTimer.IsStopped())
		{
			_gunCooldownTimer.Start(_gunCooldown);

			if (_bulletScene.Instance() is Bullet bullet)
			{
				bullet.SetDamage(_gunDamage);
				bullet.GlobalPosition = _gunPosition.GlobalPosition;
				GetTree().CurrentScene.AddChild(bullet);
			}
		}

		if (Input.IsKeyPressed((int) KeyList.E) && _missileCooldownTimer.IsStopped())
		{
			_missileCooldownTimer.Start(_missileCooldown);

			if (_missileScene.Instance() is Missile missile)
			{
				missile.SetDamage(_missileDamage);
				missile.GlobalPosition = _missilePosition.GlobalPosition;
				GetTree().CurrentScene.AddChild(missile);
			}
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!_active) return;

		Vector2 velocity;
		var direction = new Vector2(0, 0);
		var viewRect = GetViewportRect();
		var idle = true;

		if (Input.IsKeyPressed((int) KeyList.W))
			if (Position.y > 80)
			{
				direction.y = -1;
				idle = false;
			}

		if (Input.IsKeyPressed((int) KeyList.S))
			if (Position.y < viewRect.Size.y - 30)
			{
				direction.y = 1;
				idle = false;
			}

		if (Input.IsKeyPressed((int) KeyList.A))
			if (Position.x > 70)
			{
				direction.x = -1;
				idle = false;
			}

		if (Input.IsKeyPressed((int) KeyList.D))
			if (Position.x < viewRect.Size.x - 80)
			{
				direction.x = 1;
				idle = false;
			}
		
		if (idle)
		{
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

			velocity = direction * 5;
		}
		else
		{
			velocity = direction.Normalized() * _speed;
		}
		
		Position += velocity * delta;
	}

	public void Activate()
	{
		_active = true;
		_hud.InitHealthBar(_maxHealth, _health);
		_hud.InitFleetHealthBar(_maxFleetHealth, _fleetHealth);
		_hud.SetGunDamage($"{_gunDamage} ({_gunCooldown})");
		_hud.SetMissileDamage($"{_missileDamage} ({_missileCooldown})");
		_hud.SetMetalScrap(_metalScrap.ToString());
	}

	public void Deactivate()
	{
		_active = false;
	}
	
	public void SetHud(HUD hud)
	{
		_hud = hud;
	}
	
	public void TakeDamage(int damage)
	{
		_health -= damage;
		_hud.SetHealth(_health);
		
		if (_health <= 0)
		{
			_active = false;
			_hud.QueueFree();
			QueueFree();
			
			if (_explosionScene.Instance() is Explosion explosion)
			{
				explosion.Position = Position;
				GetTree().CurrentScene.CallDeferred("add_child", explosion);
			}
		}
	}
	
	public void TakeFleetDamage(int damage)
	{
		_fleetHealth -= damage;
		_hud.SetFleetHealth(_fleetHealth);
		
		if (_fleetHealth <= 0)
		{
			_active = false;
			_hud.QueueFree();
			QueueFree();
		}
	}
	
	private void _on_Player_area_entered(Area2D area)
	{
		if (area.IsInGroup("collectibles"))
		{
			if (area is ScrapMetal scrapMetal)
			{
				_metalScrap += scrapMetal.Collect();
				_hud.SetMetalScrap(_metalScrap.ToString());
			}
		}
		else if (area.IsInGroup("enemies"))
		{
			if (area is Enemy enemy)
			{
				TakeDamage(30);
				enemy.TakeDamage(1000);
			}
		}
	}
	
}
