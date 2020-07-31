using System.Collections.Generic;
using System.Data;
using System.Text;
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
	public PlayerData DataSet;
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


		foreach(PlayerData data in MasterManager.mm.playerData)
		{
			GameObject objectS = Instantiate(scoreElement, transform);
			ScoreElement se = objectS.GetComponent<ScoreElement>();
			se.nameElement.text = data.name;
			se.scoreElement.text = data.score;
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
		MasterManager.mm.playerData.Add(new PlayerData(MasterManager.mm.playerData.Count, input.text, new StringBuilder()
			.Append(MasterManager.mm.lastScore)
			.Append(" / ").Append(MasterManager.mm.maxScore)
			.Append(" (").Append(((float)MasterManager.mm.lastScore / (float)MasterManager.mm.maxScore * 100f))
			.Append("% )").ToString()));
		MasterManager.mm.playerData.Sort((x, y) => x.score.CompareTo(y.score));
		MasterManager.mm.playerData.Reverse();
		SaveSystem.SavePlayer(MasterManager.mm.playerData);
		nextPage.SetActive(true);
		submitPage.SetActive(false);
	}
	public void SubmitEdit()
	{
		DataSet.name = input.text;
		MasterManager.mm.playerData[DataSet.id] = DataSet;
		SaveSystem.SavePlayer(MasterManager.mm.playerData);
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
			DataSet = MasterManager.mm.playerData[getElement(MasterManager.mm.playerData, element.id)];
		} else
		{
			toggle = false;
		}
		toolTip.SetActive(toggle);
	}
	public void ToggleToolTip()
	{
		if (!toggle)
		{
			toggle = true;
		}
		else
		{
			toggle = false;
		}
		toolTip.SetActive(toggle);
	}

	public void ShowEditPage()
	{
		ToggleToolTip(null);
		input.text = DataSet.name;
		EditPage.SetActive(true);
	}

	public void HideEditPage()
	{
		EditPage.SetActive(false);
	}

	public void DeleteElement()
	{
		MasterManager.mm.playerData.Remove(DataSet);
		ToggleToolTip(null);
		scoreboardzeichnen();
		
	}

	int getElement(List<PlayerData> datas, int id)
    {
		for (int i = 0; i < datas.Count; i++)
		{
			if (datas[i].id == id)
            {
				return i;
            }
        }
		return 0;
    }
}
