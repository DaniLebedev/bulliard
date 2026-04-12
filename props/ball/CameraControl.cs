using Godot;

public partial class CameraControl : Camera3D
{
	[Export] public Node3D Target;

	[Export] private float mouseSensitivity = 0.003f;
	[Export] private float distance = 5.0f;
	[Export] private float height = 2.0f;

	private float yaw = 0.0f;
	private float pitch = -0.3f;

	private bool firstPerson = false;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			yaw -= motion.Relative.X * mouseSensitivity;
			pitch -= motion.Relative.Y * mouseSensitivity;

			pitch = Mathf.Clamp(pitch, -0.4f, 0.3f);
		}

		if (@event.IsActionPressed("switch_camera"))
		{
			firstPerson = !firstPerson;
		}
	}

	public override void _Process(double delta)
{
	if (Target == null) return;

	Vector3 targetPos = Target.GlobalTransform.Origin;

	Basis rotation = new Basis(Vector3.Up, yaw) * new Basis(Vector3.Right, pitch);

	if (firstPerson)
	{
		GlobalTransform = new Transform3D(
			rotation,
			targetPos + new Vector3(0, 0.0f, 0)
		);
	}
	else
	{
		// фиксированная позиция без сглаживания
		Vector3 offset = rotation * new Vector3(0, height, distance);
		Vector3 cameraPos = targetPos + offset;

		GlobalTransform = new Transform3D(
			rotation,
			cameraPos
		);
	}
}

	public Vector3 GetForward()
	{
		Vector3 forward = -GlobalTransform.Basis.Z;
		forward.Y = 0;
		return forward.Normalized();
	}

	public Vector3 GetRight()
	{
		Vector3 right = GlobalTransform.Basis.X;
		right.Y = 0;
		return right.Normalized();
	}
}
