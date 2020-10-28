using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayer : MonoBehaviour
{
	[SerializeField]
	private int autoPlayTime = 5;
	[SerializeField]
	private UsableObject bed, fridge, toilet, bath, washBasin, tv;
	private PlayerInteraction playerInteraction;
	private PlayerStatistics playerStatistics;
	private Coroutine autoPlayCoroutine;


	private void Start()
	{
		playerInteraction = GetComponent<PlayerInteraction>();
		playerStatistics = GetComponent<PlayerStatistics>();
		StartAutoPlayer();
	}

	public void StartAutoPlayer()
	{
		StopAutoPlayer();
		autoPlayCoroutine = StartCoroutine(AutoPlay());
	}

	private IEnumerator AutoPlay()
	{
		while(true)
		{
			yield return new WaitForSeconds(autoPlayTime);

			// The larger the deficits, the greater the chance for proper interaction:
			int hungerChance = playerStatistics.MaxStatisticValue - playerStatistics.Hunger;
			int entertainmentChance = playerStatistics.MaxStatisticValue - playerStatistics.Entertainment;
			int hygieneChance = playerStatistics.MaxStatisticValue - playerStatistics.Hygiene;
			int bladderChance = playerStatistics.MaxStatisticValue - playerStatistics.Bladder;
			int energyChance = playerStatistics.MaxStatisticValue - playerStatistics.Energy;

			int maxRandomValue = hungerChance + entertainmentChance + hygieneChance + bladderChance + energyChance;
			int randomValue = Random.Range(0, maxRandomValue);

			// Set interaction:
			if(randomValue < hungerChance)
			{
				playerInteraction.AddInteraction(fridge);
			}
			else if(randomValue < hungerChance+entertainmentChance)
			{
				playerInteraction.AddInteraction(tv);
			}
			else if(randomValue < hungerChance+entertainmentChance+hygieneChance)
			{
				playerInteraction.AddInteraction((Random.Range(0, 2) == 0) ? washBasin : bath);
			}
			else if(randomValue < hungerChance+entertainmentChance+hygieneChance+bladderChance)
			{
				playerInteraction.AddInteraction(toilet);
			}
			else if(randomValue < hungerChance+entertainmentChance+hygieneChance+bladderChance+energyChance)
			{
				playerInteraction.AddInteraction(bed);
			}
		}
	}

	public void StopAutoPlayer()
	{
		if(autoPlayCoroutine != null)
		{
			StopCoroutine(autoPlayCoroutine);
			autoPlayCoroutine = null;
		}
	}
}
