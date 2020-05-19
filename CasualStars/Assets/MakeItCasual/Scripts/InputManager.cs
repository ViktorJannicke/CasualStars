using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
	private NGameManager manager;

    private float cooldown;

    private Camera Main;
    public CameraController controller;

    MakeItCasualInput input;

    public bool hyperdrive;

    private void Awake()
    {
        Main = Camera.main;
        input = new MakeItCasualInput();
        input.Enable();
		manager = GetComponent<NGameManager>();
    }

    private void Start()
    {
        input.Game.Tap.performed += _ => { if (!IsPointerOverUIObject()) touch(); };
    }

    public void setHyperdrive(bool val)
    {
        if (cooldown <= 0)
        {
            controller.translateAway();
            hyperdrive = val;
        }
    }

	void touch()
	{
		Debug.Log("test");
		RaycastHit raycastHit = new RaycastHit();

		Vector2 screensposition = input.Game.Position.ReadValue<Vector2>();

		if (Physics.Raycast(Main.ScreenPointToRay(screensposition), out raycastHit, 10000))
		{
			if (raycastHit.collider.gameObject.CompareTag("Player"))
			{
				if (raycastHit.collider.gameObject.transform != transform)
				{

				}
			}
			if (raycastHit.collider.gameObject.CompareTag("MapGround"))
			{
				if (hyperdrive)
				{
					cooldown = 30f;
					manager.ExecuteHyperDrive(raycastHit.point);

					hyperdrive = false;
					controller.translateTowards();
				}
				else
				{
					manager.ExecuteMove(raycastHit.point);
				}
			}
		}
	}

	private void Update()
	{
		if (cooldown > 0)
		{
			cooldown -= Time.deltaTime;
		}
	}

	private bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = input.Game.Position.ReadValue<Vector2>();
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
