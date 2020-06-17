using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
			if (raycastHit.collider.CompareTag("Asteroid"))
			{
				detector.setTarget(raycastHit.collider.transform);
			}
		}
	}

	private void Update()
	{

	}

	private bool IsPointerOverUIObject()
	{
		/*PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = input.Game.Position.ReadValue<Vector2>();
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;*/

		if (EventSystem.current.IsPointerOverGameObject() ||
			EventSystem.current.currentSelectedGameObject != null)
		{
			return true;
		} else
		{
			return false;
		}
	}
}
