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

	public GameObject submitPage;
	public GameObject nextPage;

	public GameObject toolTip;
	public GameObject EditPage;
	public PlayerData playerData;
	public bool toggle;

	public SceneManagement sm;

	List<GameObject> obj = new List<GameObject>();
	public void scoreboardzeichnen()
	{
		if(obj.Count > 0)
		{
			for(int i = 0; i < obj.Count; i++)
			{
				Destroy(obj[i]);
			}
			obj.Clear();
		}

		List<PlayerData> ScoreData = MasterManager.mm.playerData;

		foreach(PlayerData data in ScoreData)
		{
			GameObject objectS = Instantiate(scoreElement, transform);
			ScoreElement se = objectS.GetComponent<ScoreElement>();
			se.nameElement.text = data.name;
			se.scoreElement.text = data.score.ToString();
			se.id = data.id;

			obj.Add(objectS);
		}	
	}

	private void Start()
	{
		if(scoreboardOn)
		scoreboardzeichnen();

		if (showScore)
			text.text = "Your Score: " + MasterManager.mm.lastScore;
	}

	public void Submit()
	{
		List<PlayerData> ScoreData = MasterManager.mm.playerData;
		ScoreData.Add(new PlayerData(ScoreData.Count, input.text, MasterManager.mm.lastScore));
		ScoreData.Sort((x, y) => x.score.CompareTo(y.score));
		ScoreData.Reverse();
		SaveSystem.SavePlayer(ScoreData);
		nextPage.SetActive(true);
		submitPage.SetActive(false);
	}
	public void SubmitEdit()
	{
		playerData.name = input.text;
		List<PlayerData> ScoreData = MasterManager.mm.playerData;
		ScoreData[playerData.id] = playerData;
		ScoreData.Sort((x, y) => x.score.CompareTo(y.score));
		ScoreData.Reverse();
		SaveSystem.SavePlayer(ScoreData);
		scoreboardzeichnen();
		HideEditPage();
	}

	public void Cancel()
	{
		nextPage.SetActive(true);
		submitPage.SetActive(false);
	}

	public void ToggleToolTip(ScoreElement element)
	{
		if (!toggle)
		{
			toggle = true;
			List<PlayerData> ScoreData = MasterManager.mm.playerData;
			playerData = ScoreData[element.id];
		} else
		{
			toggle = false;
		}
		toolTip.SetActive(toggle);
	}
	public void ShowEditPage()
	{
		ToggleToolTip(null);
		input.text = playerData.name;
		EditPage.SetActive(true);
	}

	public void HideEditPage()
	{
		EditPage.SetActive(false);
	}

	public void DeleteElement()
	{
		MasterManager.mm.playerData.Remove(playerData);

		scoreboardzeichnen();
		ToggleToolTip(null);
	}
}
