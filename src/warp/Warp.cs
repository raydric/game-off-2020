using Godot;

public class Warp : AnimatedSprite
{
	public override void _Ready()
	{
		Connect("animation_finished", this, nameof(_on_Warp_animation_finished));
		
		Frame = 0;
		Playing = true;
	}

	public void _on_Warp_animation_finished()
	{
		QueueFree();
	}
}
