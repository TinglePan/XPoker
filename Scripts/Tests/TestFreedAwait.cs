using Godot;

namespace XCardGame.Tests;

public partial class TestFreedAwait : Node2D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Run();
		TimedSelfFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	protected async void TimedSelfFree()
	{
		var timer = GetTree().CreateTimer(2);
		await ToSignal(timer, Timer.SignalName.Timeout);
		GD.Print("timeout");
		QueueFree();
	}
	
	protected async void Run()
	{
		var tween = CreateTween();
		var newPosition = Position + new Vector2(100, 100);
		tween.TweenProperty(this, "position", newPosition, 5);
		await ToSignal(tween, Tween.SignalName.Finished);
		GD.Print("Tween finished");
	}
}