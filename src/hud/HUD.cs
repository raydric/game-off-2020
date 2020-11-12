using Godot;

public class HUD : MarginContainer
{

	private TextureProgress _healthBar;
	private TextureProgress _fleetHealthBar;
	private Label _gunDamage;
	private Label _missileDamage;
	private Label _metalScrap;
	
	public override void _Ready()
	{
		_healthBar = GetParent().GetNode<TextureProgress>("HUD/MainContainer/HealthContainer/HealthBar");
		_fleetHealthBar = GetParent().GetNode<TextureProgress>("HUD/MainContainer/FleetHealthContainer/FleetHealthBar");
		_gunDamage = GetParent().GetNode<Label>("HUD/MainContainer/GunDamageContainer/GunDamage");
		_missileDamage = GetParent().GetNode<Label>("HUD/MainContainer/MissileDamageContainer/MissileDamage");
		_metalScrap = GetParent().GetNode<Label>("HUD/MainContainer/MetalScrapContainer/MetalScrap");
	}

	public void InitHealthBar(int max, int value)
	{
		_healthBar.MaxValue = max;
		_healthBar.Value = value;
	}
	
	public void SetHealth(int health)
	{
		_healthBar.Value = health;
	}
	
	public void InitFleetHealthBar(int max, int value)
	{
		_fleetHealthBar.MaxValue = max;
		_fleetHealthBar.Value = value;
	}
	
	public void SetFleetHealth(int health)
	{
		_fleetHealthBar.Value = health;
	}

	public void SetGunDamage(string label)
	{
		_gunDamage.Text = label;
	}
	
	public void SetMissileDamage(string label)
	{
		_missileDamage.Text = label;
	}
	
	public void SetMetalScrap(string label)
	{
		_metalScrap.Text = label;
	}
}
