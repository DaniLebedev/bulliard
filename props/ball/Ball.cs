using Godot;

public partial class Ball : RigidBody3D
{
	[Export] private float moveSpeed = 9.0f;
	[Export] private float moveSpeedCommon = 9.0f;

	[Export] private float moveSpeedMax = 9.0f;

	[Export] private CameraControl cam;
	[Export] private int Jumps = 0;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		this.ContactMonitor = true;
		this.MaxContactsReported = 8;
		moveSpeed = moveSpeedCommon;
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

		if (Input.IsActionPressed("Shift") && Jumps > 0)
		{
			moveSpeed = moveSpeedMax;
		}
		else
		{
			moveSpeed = moveSpeedCommon;
		}
	}

	private void OnBodyEntered(Node body)
	{
		// Проверка по группе
		if (body.IsInGroup("Floor"))
		{
			Jumps = 2;
		}
		if (body.IsInGroup("Ball"))
		{

		}

	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		int contactCount = state.GetContactCount();

		for (int i = 0; i < contactCount; i++)
		{
			var collider = state.GetContactColliderObject(i);

			if (collider is RigidBody3D otherBall && otherBall.IsInGroup("Ball"))
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
					otherBall.ApplyCentralImpulse(pushDir * mySpeed * 4f);
				}
			}
		}
	}
}
