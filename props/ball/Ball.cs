using Godot;

public partial class Ball : RigidBody3D
{
	[Export] private float moveSpeed = 9.0f;
	[Export] private CameraControl cam;

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
	}
}
