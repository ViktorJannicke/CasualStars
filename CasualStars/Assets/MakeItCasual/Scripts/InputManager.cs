﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{

	private NGameManager manager;

    private float cooldown;

    public Camera Main;
    public CameraController camController;

    MakeItCasualInput input;

    public bool hyperdrive;
	public Button hyperdriveButton;

	public LayerMask mask;

    private void Awake()
    {
        input = new MakeItCasualInput();
        input.Enable();
		manager = GetComponent<NGameManager>();
    }

    private void Start()
    {
        input.Game.Tap.performed += _ => { touch(); };
    }

    public void setHyperdrive(bool val)
    {
        if (cooldown <= 0)
        {
			camController.translateAway();
            hyperdrive = val;
        }
    }

	void touch()
	{
		RaycastHit raycastHit = new RaycastHit();

		Vector2 screensposition = input.Game.Position.ReadValue<Vector2>();

		if (Physics.Raycast(Main.ScreenPointToRay(screensposition), out raycastHit, 10000, mask))
		{

		}
	}

	bool updateVal = false;
	private void Update()
	{
		if (cooldown > 0)
		{
			updateVal = true;
			cooldown -= Time.deltaTime;
		} else if (cooldown <= 0 && updateVal)
		{
			hyperdriveButton.interactable = true;
			updateVal = false;
		}
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
