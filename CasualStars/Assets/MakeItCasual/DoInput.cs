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

		input.Game.Tap.performed += _ => { touch(); };



	}

	void touch()
	{
		Debug.Log("Test? Hörst du mich Welt, ich bin ein einsamer Mensch Rufst du mich überhaupt auf!!!!!!??????");
		RaycastHit raycastHit = new RaycastHit();

		Vector2 screensposition = input.Game.Position.ReadValue<Vector2>();

		Vector3 MousWorldPosition = Main.ScreenToWorldPoint(screensposition);

		if (Physics.Raycast(MousWorldPosition, Main.transform.forward, out raycastHit, 10000))
		{
			if (raycastHit.collider.gameObject.CompareTag("Player"))
			{

			}
			if (raycastHit.collider.gameObject.CompareTag("MapGrouznd"))
			{
				FliegJungeFlieg.move(MousWorldPosition);
			}
		}

	}

	// Start is called before the first frame update
	void Start()
	{

	}

}
