using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
	public static Scoreboard boardscore;
	public GameObject scoreElement;
	public List<PlayerData> ScoreData = new List<PlayerData>();
	List<GameObject> Jef = new List<GameObject>();

	public void scoreboardzeichnen()
	{

		foreach (GameObject data in Jef)
		{
			Destroy(data);
		}
		Jef.Clear();

			ScoreData.Sort((x, y) => x.score.CompareTo(y.score));

		foreach(PlayerData data in ScoreData)
		{
			GameObject objectS = Instantiate(scoreElement, transform);
			objectS.transform.GetChild(0).GetComponent<Text>().text = data.name;
			objectS.transform.GetChild(1).GetComponent<Text>().text = data.score.ToString();
			Jef.Add(objectS);
		}	
	}

	private void Start()
	{
		if (boardscore != null)
		{
			Destroy(this);
			return;
		}

		boardscore = this;
	}








}
