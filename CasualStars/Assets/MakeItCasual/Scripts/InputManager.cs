using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
	Camera main;

    //MakeItCasualInput input;
	public EnemyDetector detector;

	public LayerMask mask;
	public int[] touchDelay;//Frames
    private void Awake()
    {
        //input = new MakeItCasualInput();
        //input.Enable();
    }

    private void Start()
    {
		main = Camera.main;
		//input.Game.Tap.performed += _ => { touch(); };
    }

	void touch(Vector3 screensposition)
	{
		RaycastHit raycastHit = new RaycastHit();

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
		//input.Disable();
	}
	int stop;
	private void Update()
	{
		if(Input.touchCount >= 1)
		{
			if (stop < touchDelay[MasterManager.mm.difficulty])
			{
				stop++;
			}
			else
			{
				stop = 0;
				touch(Input.GetTouch(0).position);
			}
		}
	}
}
