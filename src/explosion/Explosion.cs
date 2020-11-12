using Godot;

public class Explosion : AnimatedSprite
{
	public override void _Ready()
	{
		Connect("animation_finished", this, nameof(_on_Explosion_animation_finished));
		
		Frame = 0;
		Playing = true;
	}

	public void _on_Explosion_animation_finished()
	{
		QueueFree();
	}
	
}
