using Godot;

public partial class DeadZone : Area3D
{
	public override void _Ready()
	{
		this.Monitorable = true;
		this.Monitoring = true;
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		GD.Print("Entered: ", body.Name);

		if (body is RigidBody3D ball)
		{
			ball.GlobalTransform = new Transform3D(
				ball.GlobalTransform.Basis,
				new Vector3(0, 2, 0)
			);

			ball.LinearVelocity = Vector3.Zero;
			ball.AngularVelocity = Vector3.Zero;
		}
	}
}
