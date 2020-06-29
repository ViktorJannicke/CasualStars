using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
	Camera main;

    MakeItCasualInput input;
	public EnemyDetector detector;

	public LayerMask mask;

    private void Awake()
    {
        input = new MakeItCasualInput();
        input.Enable();
    }

    private void Start()
    {
		main = Camera.main;
		input.Game.Tap.performed += _ => { touch(); };
    }

	void touch()
	{
		RaycastHit raycastHit = new RaycastHit();

		Vector2 screensposition = input.Game.Position.ReadValue<Vector2>();

		if (Physics.Raycast(main.ScreenPointToRay(screensposition), out raycastHit, 10000, mask))
		{
			if (raycastHit.collider.CompareTag("AsteroidIn") || raycastHit.collider.CompareTag("AsteroidOut"))
			{
				detector.setTarget(raycastHit.collider.transform);
			}
		}
	}

	private void OnDisable()
	{
		input.Disable();
	}
	private void Update()
	{

	}
}
