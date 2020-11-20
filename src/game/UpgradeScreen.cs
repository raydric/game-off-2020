using System.Globalization;
using Godot;

public class UpgradeScreen : Node2D
{
	private Player _player;
	private Label _metalScrapValue;
	private Label _healthValue;
	private Label _maxHealthValue;
	private Label _gunDamageValue;
	private Label _gunCooldownValue;
	private Label _missileDamageValue;
	private Label _missileCooldownValue;
	private Label _speedValue;

	private Button _healthButton;
	private Button _gunDamageButton;
	private Button _gunCooldownButton;
	private Button _missileDamageButton;
	private Button _missileCooldownButton;
	private Button _speedButton;
	private Button _repairButton;

	private AudioStreamPlayer _menuSound;
	
	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<Player>("./Player");
		
		_metalScrapValue = GetNode<Label>("MetalScrapAmount");
		_healthValue = GetNode<Label>("HealthValue");
		_maxHealthValue = GetNode<Label>("MaxHealthValue");
		_gunDamageValue = GetNode<Label>("GunDamageValue");
		_gunCooldownValue = GetNode<Label>("GunCooldownValue");
		_missileDamageValue = GetNode<Label>("MissileDamageValue");
		_missileCooldownValue = GetNode<Label>("MissileCooldownValue");
		_speedValue = GetNode<Label>("SpeedValue");
		
		_healthButton = GetNode<Button>("HealthButton");
		_gunDamageButton = GetNode<Button>("GunDamageButton");
		_gunCooldownButton = GetNode<Button>("GunCooldownButton");
		_missileDamageButton = GetNode<Button>("MissileDamageButton");
		_missileCooldownButton = GetNode<Button>("MissileCooldownButton");
		_speedButton = GetNode<Button>("SpeedButton");
		_repairButton = GetNode<Button>("RepairButton");
		
		_healthButton.Connect("pressed", this,nameof(_on_HealthButton_pressed));
		_gunDamageButton.Connect("pressed", this,nameof(_on_GunDamageButton_pressed));
		_gunCooldownButton.Connect("pressed", this,nameof(_on_GunCooldownButton_pressed));
		_missileDamageButton.Connect("pressed", this,nameof(_on_MissileDamageButton_pressed));
		_missileCooldownButton.Connect("pressed", this,nameof(_on_MissileCooldownButton_pressed));
		_speedButton.Connect("pressed", this,nameof(_on_SpeedButton_pressed));
		_repairButton.Connect("pressed", this,nameof(_on_RepairButton_pressed));
		
		_menuSound = GetNode<AudioStreamPlayer>("MenuSound");
	}

	public override void _Process(float delta)
	{
		_metalScrapValue.Text = _player.MetalScrap.ToString();
		_healthValue.Text = _player.Health.ToString();
		_maxHealthValue.Text = _player.MaxHealth.ToString();
		_gunDamageValue.Text = _player.GunDamage.ToString();
		_gunCooldownValue.Text = _player.GunCooldown.ToString(CultureInfo.InvariantCulture);
		_missileDamageValue.Text = _player.MissileDamage.ToString();
		_missileCooldownValue.Text = _player.MissileCooldown.ToString(CultureInfo.InvariantCulture);
		_speedValue.Text = _player.Speed.ToString();

		if (_player.MetalScrap >= _player.UpgradePrice)
		{
			_healthButton.Disabled = false;
			_gunDamageButton.Disabled = false;
			_gunCooldownButton.Disabled = false;
			_missileDamageButton.Disabled = false;
			_missileCooldownButton.Disabled = false;
			_speedButton.Disabled = false;
		}
		else
		{
			_healthButton.Disabled = true;
			_gunDamageButton.Disabled = true;
			_gunCooldownButton.Disabled = true;
			_missileDamageButton.Disabled = true;
			_missileCooldownButton.Disabled = true;
			_speedButton.Disabled = true;
		}

		if (_player.MetalScrap >= _player.RepairPrice)
		{
			_repairButton.Disabled = _player.Health == _player.MaxHealth;
		}
		else
		{
			_repairButton.Disabled = true;
		}
	}

	private void _on_HealthButton_pressed()
	{
		_menuSound.Play();
		_player.UpgradeHealth();
	}
	
	private void _on_GunDamageButton_pressed()
	{
		_menuSound.Play();
		_player.UpgradeGunDamage();
	}
	
	private void _on_GunCooldownButton_pressed()
	{
		_menuSound.Play();
		_player.UpgradeGunCooldown();
	}
	
	private void _on_MissileDamageButton_pressed()
	{
		_menuSound.Play();
		_player.UpgradeMissileDamage();
	}
	
	private void _on_MissileCooldownButton_pressed()
	{
		_menuSound.Play();
		_player.UpgradeMissileCooldown();
	}
	
	private void _on_SpeedButton_pressed()
	{
		_menuSound.Play();
		_player.UpgradeSpeed();
	}
	
	private void _on_RepairButton_pressed()
	{
		_menuSound.Play();
		_player.Repair();
	}
	
}
