using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardLink : MonoBehaviour
{
    Scoreboard board;

    // Start is called before the first frame update
    void Start()
    {
        board = GetComponentInParent<Scoreboard>();
    }
	public void ToggleToolTip(ScoreElement element)
	{
        board.ToggleToolTip(element);
	}

    public void Cancel()
    {
        board.Cancel();
    }
}
