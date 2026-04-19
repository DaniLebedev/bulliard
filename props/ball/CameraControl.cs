using Godot;

public partial class CameraControl : Camera3D
{
	[Export] public PhysicsBody3D Target;

	[Export] private float mouseSensitivity = 0.003f;
	[Export] private float DistanceMax = 10f;
	[Export] private float DistanceMin = 1.0f;
	[Export] private float DistanceStep = 0.1f;


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
			if (firstPerson)
			{
				pitch = Mathf.Clamp(pitch, -1.5f, 1.5f);
			}
			else
			{
				pitch = Mathf.Clamp(pitch, -1.25f, 0.5f);
			}
		}
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed)
			{
				if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
				{
					distance = Mathf.Min(distance + DistanceStep, DistanceMax);
				}
				else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
				{
					distance = Mathf.Max(distance - DistanceStep, DistanceMin);
				}
			}
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
			Vector3 offset = rotation * new Vector3(0, height, distance);
			Vector3 desiredPos = targetPos + offset;
			height = distance / 3f;
			// луч от шара к камере
			PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

			PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
				targetPos,
				desiredPos
			);

			// исключаем сам шар (чтобы не ловить себя)
			query.Exclude = new Godot.Collections.Array<Rid> { Target.GetRid() };

			Godot.Collections.Dictionary result = spaceState.IntersectRay(query);

			Vector3 finalPos = desiredPos;

			if (result.Count > 0)
			{
				Vector3 hitPos = (Vector3)result["position"];

				// чуть отодвигаем камеру от стены (чтобы не залипала)
				Vector3 dir = (desiredPos - targetPos).Normalized();
				finalPos = hitPos - dir * 0.3f;
			}

			GlobalTransform = new Transform3D(
				rotation,
				finalPos
			);
		}
		if (Input.IsActionPressed("CamUp"))
		{
			distance = Mathf.Min(distance + DistanceStep, DistanceMax);


		}
		else if (Input.IsActionPressed("CamDown"))
		{
			distance = Mathf.Max(distance - DistanceStep, DistanceMin);

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
