using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
	public GameObject scoreElement;

	public void scoreboardzeichnen()
	{
		List<PlayerData> ScoreData = MasterManager.mm.playerDatas;
		ScoreData.Sort((x, y) => x.score.CompareTo(y.score));
		ScoreData.Reverse();

		foreach(PlayerData data in ScoreData)
		{
			GameObject objectS = Instantiate(scoreElement, transform);
			ScoreElement se = objectS.GetComponent<ScoreElement>();
			se.nameElement.text = data.name;
			se.scoreElement.text = data.score.ToString();
		}	
	}

	private void Start()
	{
		scoreboardzeichnen();
	}








}
