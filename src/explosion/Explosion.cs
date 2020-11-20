using Godot;

public class Explosion : Node2D
{
	private AnimatedSprite _explosionAnimation;
	private AudioStreamPlayer _explosionSound;
	
	public override void _Ready()
	{
		_explosionAnimation = GetNode<AnimatedSprite>("ExplosionAnimation");
		_explosionSound = GetNode<AudioStreamPlayer>("ExplosionSound");
		
		_explosionSound.Connect("finished", this, nameof(_on_ExplosionSound_finished));
		
		_explosionAnimation.Frame = 0;
		_explosionAnimation.Playing = true;
		
		_explosionSound.Play();
	}

	public void _on_ExplosionSound_finished()
	{
		QueueFree();
	}
	
}
