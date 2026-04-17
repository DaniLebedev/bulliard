using Godot;

public partial class FreeCam : Camera3D
{
	[Export] private float speed = 10f;
	[Export] private float boostMultiplier = 3f;
	[Export] private float mouseSensitivity = 0.002f;

	private float yaw = 0f;
	private float pitch = 0f;

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

			pitch = Mathf.Clamp(pitch, -1.5f, 1.5f);
		}

		if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public override void _Process(double delta)
	{
		float deltaF = (float)delta;

		// ✅ Сначала обновляем вращение
		Basis rotation = new Basis(Vector3.Up, yaw) * new Basis(Vector3.Right, pitch);
		GlobalTransform = new Transform3D(rotation, GlobalTransform.Origin);

		// ✅ Берём актуальные направления из камеры
		Vector3 forward = -GlobalTransform.Basis.Z;
		Vector3 right = GlobalTransform.Basis.X;
		Vector3 up = GlobalTransform.Basis.Y;

		Vector3 direction = Vector3.Zero;

		if (Input.IsActionPressed("UP")) direction += forward;
		if (Input.IsActionPressed("DOWN")) direction -= forward;
		if (Input.IsActionPressed("LEFT")) direction -= right;
		if (Input.IsActionPressed("RIGHT")) direction += right;

		if (Input.IsActionPressed("ui_accept")) direction += up;
		if (Input.IsActionPressed("ui_select")) direction -= up;

		float currentSpeed = speed;

		if (Input.IsKeyPressed(Key.Shift))
			currentSpeed *= boostMultiplier;

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			GlobalPosition += direction * currentSpeed * deltaF;
		}
	}
}
