using System;
using Godot;

public class Game : Node2D
{
	// INTERNAL
	private readonly PackedScene _playerScene = ResourceLoader.Load("res://player/Player.tscn") as PackedScene;
	private readonly PackedScene _gameOverScene = ResourceLoader.Load("res://game/GameOver.tscn") as PackedScene;
	private readonly PackedScene _levelScene = ResourceLoader.Load("res://game/Level.tscn") as PackedScene;
	private readonly PackedScene _hudScene = ResourceLoader.Load("res://hud/Hud.tscn") as PackedScene;
	private readonly PackedScene _gameLevelScene = ResourceLoader.Load("res://game/GameLevel.tscn") as PackedScene;
	private readonly PackedScene _fleetScene = ResourceLoader.Load("res://fleet/Fleet.tscn") as PackedScene;
	private readonly PackedScene _fleetScene2 = ResourceLoader.Load("res://fleet/Fleet2.tscn") as PackedScene;
	private readonly PackedScene _flightButtonScene = ResourceLoader.Load("res://game/FlightButton.tscn") as PackedScene;
	private readonly PackedScene _menuButtonScene = ResourceLoader.Load("res://game/MenuButton.tscn") as PackedScene;

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
		
		if (_fleetScene.Instance() is Fleet fleet)
		{
			fleet.Position = new Vector2(-200, 250);
			GetTree().CurrentScene.AddChild(fleet);
		}
		
		if (_fleetScene.Instance() is Fleet fleet2)
		{
			fleet2.Position = new Vector2(-200, 450);
			GetTree().CurrentScene.AddChild(fleet2);
		}
		
		if (_fleetScene.Instance() is Fleet fleet3)
		{
			fleet3.Position = new Vector2(-500, 150);
			GetTree().CurrentScene.AddChild(fleet3);
		}
		
		if (_fleetScene.Instance() is Fleet fleet4)
		{
			fleet4.Position = new Vector2(-500, 350);
			GetTree().CurrentScene.AddChild(fleet4);
		}
		
		if (_fleetScene.Instance() is Fleet fleet5)
		{
			fleet5.Position = new Vector2(-500, 550);
			GetTree().CurrentScene.AddChild(fleet5);
		}
		
		if (_fleetScene2.Instance() is Fleet2 fleet6)
		{
			fleet6.Position = new Vector2(-800, 250);
			GetTree().CurrentScene.AddChild(fleet6);
		}
		
		if (_fleetScene2.Instance() is Fleet2 fleet7)
		{
			fleet7.Position = new Vector2(-800, 450);
			GetTree().CurrentScene.AddChild(fleet7);
		}
		
		if (_fleetScene2.Instance() is Fleet2 fleet8)
		{
			fleet8.Position = new Vector2(-1100, 150);
			GetTree().CurrentScene.AddChild(fleet8);
		}
		
		if (_fleetScene2.Instance() is Fleet2 fleet9)
		{
			fleet9.Position = new Vector2(-1100, 350);
			GetTree().CurrentScene.AddChild(fleet9);
		}
		
		if (_fleetScene2.Instance() is Fleet2 fleet10)
		{
			fleet10.Position = new Vector2(-1100, 550);
			GetTree().CurrentScene.AddChild(fleet10);
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
				if (_flightButton == null)
				{
					if (_flightButtonScene.Instance() is FlightButton flightButton)
					{
						flightButton.RectPosition = new Vector2(-256, 600);
						GetTree().CurrentScene.AddChild(flightButton);
						_flightButton = flightButton;
					}
				}
				
				break;
			case GameState.Start:
				if (MoveCameraForward(delta))
				{
					_levelWaitTime = 0;
					_flightButton = null;
					_gameState = GameState.PrepareGame;
				}
				break;
			case GameState.Intro:
				if (_flightButton == null)
				{
					if (_flightButtonScene.Instance() is FlightButton flightButton)
					{
						flightButton.RectPosition = new Vector2(-256, 600);
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
