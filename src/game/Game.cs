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

		_gameState = GameState.Menu;

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
				if (_levelNumber != 1)
				{
					_level.IncreaseDifficulty();
				}

				if (LevelIntroduction(delta))
				{
					return;
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
				MoveCameraBack(delta);
				break;
			case GameState.UpgradeMenu:
				break;
			case GameState.Menu:
				if (MoveCameraForward(delta))
				{
					_levelWaitTime = 0;
					_gameState = GameState.PrepareGame;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
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
