using UnityEngine;
using System.Collections;

//this script is no longer in use
public class DifficultyManager : MonoBehaviour
{

	public static DifficultyManager instance;

	private float difficultySpeedAdditive;
	public float DifficultySpeedAdditive
	{
		get
		{
			return instance.difficultySpeedAdditive;
		}
		private set
		{
			instance.difficultySpeedAdditive = value;
		}
	}


	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}
	}


	void Start()
	{
		MiniGameEventManager.OnCorrectBoxPlacement += AdjustDifficulty;
	}

	private void AdjustDifficulty()
	{
		DifficultySpeedAdditive = ScoreManager.instance.Score * 0.05f;
	}

	void OnDestroy()
	{
		MiniGameEventManager.OnCorrectBoxPlacement -= AdjustDifficulty;
	}
}
