using System;
using Godot;

public class Game : Node2D
{
	// INTERNAL
	private readonly PackedScene _playerScene = ResourceLoader.Load("res://player/Player.tscn") as PackedScene;
	private readonly PackedScene _gameOverScene = ResourceLoader.Load("res://game/GameOver.tscn") as PackedScene;
	private readonly PackedScene _levelScene = ResourceLoader.Load("res://game/Level.tscn") as PackedScene;
	private readonly PackedScene _hudScene = ResourceLoader.Load("res://hud/HUD.tscn") as PackedScene;
	private readonly PackedScene _gameLevelScene = ResourceLoader.Load("res://game/GameLevel.tscn") as PackedScene;
	private readonly PackedScene _fleetScene = ResourceLoader.Load("res://fleet/Fleet.tscn") as PackedScene;
	private readonly PackedScene _fleetScene2 = ResourceLoader.Load("res://fleet/Fleet2.tscn") as PackedScene;
	private readonly PackedScene _flightButtonScene = ResourceLoader.Load("res://game/FlightButton.tscn") as PackedScene;
	private readonly PackedScene _menuButtonScene = ResourceLoader.Load("res://game/MenuButton.tscn") as PackedScene;
	private readonly PackedScene _upgradeScene = ResourceLoader.Load("res://game/UpgradeScreen.tscn") as PackedScene;

	private Camera2D _camera2D;
	private const float CameraSpeed = 800;

	// STATE
	private GameState _gameState;
	private Player _player;
	private HUD _hud;
	private GameLevel _gameLevel;
	private Level _level;
	private Rect2 _viewRect;
	private int _levelNumber = 1;
	private float _levelWaitTime;
	private FlightButton _flightButton;
	private MenuButton _menuButton;
	private GameOver _gameOver;
	private UpgradeScreen _upgradeScreen;

	private Fleet fleet1;
	private Fleet fleet2;
	private Fleet fleet3;
	private Fleet fleet4;
	private Fleet fleet5;
	private Fleet2 fleet6;
	private Fleet2 fleet7;
	private Fleet2 fleet8;
	private Fleet2 fleet9;
	private Fleet2 fleet10;

	public override void _Ready()
	{
		_viewRect = GetViewportRect();
		_camera2D = GetParent().GetNode<Camera2D>("Game/Camera2D");

		if (_playerScene.Instance() is Player player)
		{
			player.Position = new Vector2(100, _viewRect.Size.y / 2);
			GetTree().CurrentScene.AddChild(player);
			_player = player;
		}

		if (_levelScene.Instance() is Level level)
		{
			GetTree().CurrentScene.AddChild(level);
			_level = level;
		}
		
		if (_fleetScene.Instance() is Fleet f1)
		{
			f1.Position = new Vector2(-200, 250);
			GetTree().CurrentScene.AddChild(f1);
			fleet1 = f1;
		}
		
		if (_fleetScene.Instance() is Fleet f2)
		{
			f2.Position = new Vector2(-200, 450);
			GetTree().CurrentScene.AddChild(f2);
			fleet2 = f2;
		}
		
		if (_fleetScene.Instance() is Fleet f3)
		{
			f3.Position = new Vector2(-500, 150);
			GetTree().CurrentScene.AddChild(f3);
			fleet3 = f3;
		}
		
		if (_fleetScene.Instance() is Fleet f4)
		{
			f4.Position = new Vector2(-500, 350);
			GetTree().CurrentScene.AddChild(f4);
			fleet4 = f4;
		}
		
		if (_fleetScene.Instance() is Fleet f5)
		{
			f5.Position = new Vector2(-500, 550);
			GetTree().CurrentScene.AddChild(f5);
			fleet5 = f5;
		}
		
		if (_fleetScene2.Instance() is Fleet2 f6)
		{
			f6.Position = new Vector2(-800, 250);
			GetTree().CurrentScene.AddChild(f6);
			fleet6 = f6;
		}
		
		if (_fleetScene2.Instance() is Fleet2 f7)
		{
			f7.Position = new Vector2(-800, 450);
			GetTree().CurrentScene.AddChild(f7);
			fleet7 = f7;
		}
		
		if (_fleetScene2.Instance() is Fleet2 f8)
		{
			f8.Position = new Vector2(-1100, 150);
			GetTree().CurrentScene.AddChild(f8);
			fleet8 = f8;
		}
		
		if (_fleetScene2.Instance() is Fleet2 f9)
		{
			f9.Position = new Vector2(-1100, 350);
			GetTree().CurrentScene.AddChild(f9);
			fleet9 = f9;
		}
		
		if (_fleetScene2.Instance() is Fleet2 f10)
		{
			f10.Position = new Vector2(-1100, 550);
			GetTree().CurrentScene.AddChild(f10);
			fleet10 = f10;
		}

		_gameState = GameState.Intro;

		_player.Connect("tree_exited", this, nameof(_on_Player_tree_exited));
	}

	public override void _Process(float delta)
	{
		switch (_gameState)
		{
			case GameState.Game:
				if (!_level.IsRunning())
				{
					if (MoveCameraBack(delta))
					{
						_gameState = GameState.UpgradeMenu;
					}
					else
					{
						if (_hud != null)
						{
							_hud.QueueFree();
							_hud = null;
							_player.Deactivate();

							UpdateFleet();
						}
					}
				}
				break;
			case GameState.PrepareGame:
				if (LevelIntroduction(delta))
				{
					return;
				}
				
				if (_levelNumber != 1)
				{
					_level.IncreaseDifficulty();
				}
				
				_levelNumber++;
				_level.Start();
				CreateHud();
				_player.SetHud(_hud);
				_player.Activate();
				_gameState = GameState.Game;
				break;
			case GameState.GameOver:
				if (_level != null)
				{
					_level.QueueFree();
					_level = null;
				}
				
				ShowGameOver();
				
				if (MoveCameraBack(delta))
				{
					if (_menuButton == null)
					{
						if (_menuButtonScene.Instance() is MenuButton menuButton)
						{
							menuButton.RectPosition = new Vector2(-325, 600);
							GetTree().CurrentScene.AddChild(menuButton);
							_menuButton = menuButton;
						}
					}
				}
				break;
			case GameState.UpgradeMenu:
				if (_upgradeScreen == null)
				{
					if (_upgradeScene.Instance() is UpgradeScreen upgradeScreen)
					{
						GetTree().CurrentScene.AddChild(upgradeScreen);
						_upgradeScreen = upgradeScreen;
					}
				}
				
				if (_flightButton == null)
				{
					if (_flightButtonScene.Instance() is FlightButton flightButton)
					{
						flightButton.RectPosition = new Vector2(-250, 600);
						GetTree().CurrentScene.AddChild(flightButton);
						_flightButton = flightButton;
					}
				}
				
				break;
			case GameState.Start:
				if (_upgradeScreen != null)
				{
					_upgradeScreen.QueueFree();
					_upgradeScreen = null;
				}
				
				if (MoveCameraForward(delta))
				{
					_levelWaitTime = 0;
					_flightButton = null;
					_upgradeScreen = null;
					_gameState = GameState.PrepareGame;
				}
				break;
			case GameState.Intro:
				if (_flightButton == null)
				{
					if (_flightButtonScene.Instance() is FlightButton flightButton)
					{
						flightButton.RectPosition = new Vector2(-250, 600);
						GetTree().CurrentScene.AddChild(flightButton);
						_flightButton = flightButton;
					}
				}

				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void ChangeGameState(GameState gameState)
	{
		_gameState = gameState;
	}

	private void CreateHud()
	{
		if (_hudScene.Instance() is HUD hud)
		{
			GetTree().CurrentScene.AddChild(hud);
			_hud = hud;
		}
	}

	private bool LevelIntroduction(float delta)
	{
		_levelWaitTime += delta;

		if (_levelWaitTime > 4)
		{
			_gameLevel.QueueFree();
			_gameLevel = null;
			return false;
		}
		if (_levelWaitTime > 1)
		{
			if (_gameLevel == null)
			{
				if (_gameLevelScene.Instance() is GameLevel gameLevel)
				{
					GetTree().CurrentScene.AddChild(gameLevel);
					gameLevel.SetLevel(_levelNumber);
					_gameLevel = gameLevel;
				}
			}
			return true;
		}

		return true;
	}

	private void UpdateFleet()
	{
		var fleetHealth = _player.FleetHealth;
		var maxFleetHealth = _player.MaxFleetHealth;
		var shipHealth = maxFleetHealth / 10;

		if (fleetHealth < maxFleetHealth - shipHealth)
		{
			if (fleet1 != null)
			{
				fleet1.QueueFree();
				fleet1 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 2 * shipHealth)
		{
			if (fleet2 != null)
			{
				fleet2.QueueFree();
				fleet2 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 3 * shipHealth)
		{
			if (fleet3 != null)
			{
				fleet3.QueueFree();
				fleet3 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 4 * shipHealth)
		{
			if (fleet4 != null)
			{
				fleet4.QueueFree();
				fleet4 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 5 * shipHealth)
		{
			if (fleet5 != null)
			{
				fleet5.QueueFree();
				fleet5 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 6 * shipHealth)
		{
			if (fleet6 != null)
			{
				fleet6.QueueFree();
				fleet6 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 7 * shipHealth)
		{
			if (fleet7 != null)
			{
				fleet7.QueueFree();
				fleet7 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 8 * shipHealth)
		{
			if (fleet8 != null)
			{
				fleet8.QueueFree();
				fleet8 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 9 * shipHealth)
		{
			if (fleet9 != null)
			{
				fleet9.QueueFree();
				fleet9 = null;
			}
		}
		
		if (fleetHealth < maxFleetHealth - 10 * shipHealth)
		{
			if (fleet10 != null)
			{
				fleet10.QueueFree();
				fleet10 = null;
			}
		}
	}

	private void _on_Player_tree_exited()
	{
		var children = GetNode("Level").GetChildren();

		foreach (var child in children)
		{
			if (child is Enemy enemy)
			{
				enemy.WarpAway();
				enemy.QueueFree();
			}
		}
		
		_gameState = GameState.GameOver;
	}

	private void ShowGameOver()
	{
		if (_gameOver == null)
		{
			UpdateFleet();
			
			if (_gameOverScene.Instance() is GameOver gameOver)
			{
				gameOver.RectGlobalPosition = new Vector2(-_viewRect.Size.x + 75, 100);
				GetTree().CurrentScene.AddChild(gameOver);
				_gameOver = gameOver;
			}
		}
	}

	private bool MoveCameraBack(float delta)
	{
		if (_camera2D.Position.x <= -_viewRect.Size.x / 2)
		{
			_camera2D.Position = new Vector2(-_viewRect.Size.x / 2, _viewRect.Size.y / 2);
			return true;
		}
		
		var direction = new Vector2(-1, 0);
		var velocity = direction * CameraSpeed;
		_camera2D.Position += velocity * delta;
		return false;
	}

	private bool MoveCameraForward(float delta)
	{
		if (_camera2D.Position.x >= _viewRect.Size.x / 2)
		{
			_camera2D.Position = new Vector2(_viewRect.Size.x / 2, _viewRect.Size.y / 2);
			return true;
		}
		
		var direction = new Vector2(1, 0);
		var velocity = direction * CameraSpeed;
		_camera2D.Position += velocity * delta;
		return false;
	}
}
