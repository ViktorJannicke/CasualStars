using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoInput : MonoBehaviour
{
	public Movement FliegJungeFlieg;

	Camera Main;
	public CameraController controller;

	MakeItCasualInput input;

	public bool hyperdrive;

	private void Awake()
	{

		Main = Camera.main;
		input = new MakeItCasualInput();
		input.Game.Tap.performed += _ => touch();
		input.Enable();
	}

	public void setHyperdrive(bool val)
	{
		hyperdrive = val;
	}

	void touch()
	{
		RaycastHit raycastHit = new RaycastHit();

		Vector2 screensposition = input.Game.Position.ReadValue<Vector2>();

		if (Physics.Raycast(Main.ScreenPointToRay(screensposition), out raycastHit, 10000))
		{
			if (raycastHit.collider.gameObject.CompareTag("Player"))
			{
				if(raycastHit.collider.gameObject.transform != transform)
				{

				}
			}
			if (raycastHit.collider.gameObject.CompareTag("MapGrouznd"))
			{
				if (hyperdrive)
				{
					FliegJungeFlieg.hyperdrive(raycastHit.point);

					hyperdrive = false;
					controller.translateTowards();
				}
				else
				{
					FliegJungeFlieg.move(raycastHit.point);

				}
			}
		}
	}

	// Start is called before the first frame update
	private void OnDisable()
	{
		input.Disable();
	}
}
