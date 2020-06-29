using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
	public GameObject scoreElement;

	public TMP_InputField input;
	public TextMeshProUGUI text;
	public bool scoreboardOn;
	public bool showScore;

	public SceneManagement sm;
	public void scoreboardzeichnen()
	{
		List<PlayerData> ScoreData = MasterManager.mm.playerData;
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
		if(scoreboardOn)
		scoreboardzeichnen();

		if (showScore)
			text.text = "Your Score: " + MasterManager.mm.lastScore;
	}

	public void SaveScore_Scoreboard()
	{
		MasterManager.mm.playerData.Add(new PlayerData(input.text, MasterManager.mm.lastScore));
		SaveSystem.SavePlayer(MasterManager.mm.playerData);
		sm.LoadScoreboardFromScoreSubmition();
	}
	public void SaveScore_newGame()
	{
		MasterManager.mm.playerData.Add(new PlayerData(input.text, MasterManager.mm.lastScore));
		SaveSystem.SavePlayer(MasterManager.mm.playerData);
		sm.LoadGameStartFromScoreSubmition();
	}
}
