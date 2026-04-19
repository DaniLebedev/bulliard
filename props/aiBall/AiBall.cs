using Godot;
using System;

public partial class AiBall : Node
{
	[Export] private float moveSpeed = 9.0f;
	[Export] private float moveSpeedCommon = 9.0f;
	[Export] private float moveSpeedMax = 9.0f;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
