using System;
using Godot;

public partial class Ball : RigidBody3D
{
	[Export] private float moveSpeed = 5.0f;
	[Export] private float moveSpeedCommon = 5.0f;
	[Export] private float moveSpeedMax = 12.0f;
	[Export] private CameraControl cam;
	[Export] private int Jumps = 0;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		this.ContactMonitor = true;
		this.MaxContactsReported = 8;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (cam == null) return;

		float deltaF = (float)delta;

		Vector3 camForward = cam.GetForward();
		Vector3 camRight = cam.GetRight();

		Vector2 input = Input.GetVector("LEFT", "RIGHT", "DOWN", "UP");

		Vector3 moveDirection =
			camForward * input.Y +
			camRight * input.X;

		if (moveDirection.Length() > 0.01f)
		{
			ApplyCentralImpulse(moveDirection.Normalized() * moveSpeed * deltaF);
		}

		if (Input.IsActionJustPressed("JUMP") && Jumps > 0)
		{
			ApplyCentralImpulse(Vector3.Up * this.moveSpeed * 1.25f);
			Jumps--;
		}

		if (Input.IsActionPressed("Shift"))
		{
			moveSpeed = moveSpeedMax;
		}
		else
		{
			moveSpeed = moveSpeedCommon;
		}

		float speed = MathF.Round(LinearVelocity.Length(), 1);
		GD.Print(speed);

	}

	private void OnBodyEntered(Node body)
	{
		// Проверка по группе
		if (body.IsInGroup("Floor"))
		{
			Jumps = 2;
		}


	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		int contactCount = state.GetContactCount();

		for (int i = 0; i < contactCount; i++)
		{
			var collider = state.GetContactColliderObject(i);

			if (collider is RigidBody3D otherBall && (otherBall.IsInGroup("Ball") || otherBall.IsInGroup("AiBall")))
			{
				Vector3 myVelocity = LinearVelocity;
				Vector3 otherVelocity = otherBall.LinearVelocity;

				Vector3 normal = state.GetContactLocalNormal(i);

				float mySpeed = myVelocity.Length();
				float otherSpeed = otherVelocity.Length();

				// если ты сильнее — толкаешь
				if (mySpeed > otherSpeed)
				{
					Vector3 pushDir = -normal;
					otherBall.ApplyCentralImpulse(pushDir * mySpeed * 2f);
				}
			}
		}
	}
}
