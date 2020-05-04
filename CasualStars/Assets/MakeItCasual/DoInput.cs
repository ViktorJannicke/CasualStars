using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoInput : MonoBehaviour
{
	public Movement FliegJungeFlieg;
	Camera Main;
	MakeItCasualInput input;

	private void Awake()
	{
		Main = Camera.main;

		input = new MakeItCasualInput();

		input.Game.Tap.performed += _ => touch();

		input.Enable();

	}

	void touch()
	{
		RaycastHit raycastHit = new RaycastHit();

		Vector2 screensposition = input.Game.Position.ReadValue<Vector2>();

		if (Physics.Raycast(Main.ScreenPointToRay(screensposition), out raycastHit, 10000))
		{
			Debug.Log(raycastHit.point);

			if (raycastHit.collider.gameObject.CompareTag("Player"))
			{

			}
			if (raycastHit.collider.gameObject.CompareTag("MapGrouznd"))
			{
				FliegJungeFlieg.move(raycastHit.point);
			}
		}

	}

	// Start is called before the first frame update
	private void OnDisable()
	{
		input.Disable();
	}

}
