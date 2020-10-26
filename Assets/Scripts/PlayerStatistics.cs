using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
	public enum Statistic { Hunger, Entertainment, Hygiene, Bladder, Energy }


	[SerializeField]
	private int maxStatisticValue = 10, startStatisticValue = 5;
	[SerializeField] [Tooltip("Set time in seconds.")]
	private int statisticsDecreaseTime = 30;
	public int hunger, entertainment, hygiene, bladder, energy;


	public int Hunger => hunger;
	public int Entertainment => entertainment;
	public int Hygiene => hygiene;
	public int Bladder => bladder;
	public int Energy => energy;


	private void Start()
	{
		InitStatistics();
		StartCoroutine(DecreaseStatisticsOverTime());
	}

	private void OnValidate()
	{
		CheckStartStatisticValue();
	}

	private void InitStatistics()
	{
		CheckStartStatisticValue();

		hunger = startStatisticValue;
		entertainment = startStatisticValue;
		hygiene = startStatisticValue;
		bladder = startStatisticValue;
		energy = startStatisticValue;
	}

	private void CheckStartStatisticValue()
	{
		if(startStatisticValue > maxStatisticValue)
		{
			startStatisticValue = maxStatisticValue;
		}
	}

	public void IncreaseStatistic(Statistic statistic)
	{
		switch(statistic)
		{
			case Statistic.Hunger:
				if(hunger < maxStatisticValue)
				{
					hunger++;
				}
				break;
			case Statistic.Entertainment:
				if(entertainment < maxStatisticValue)
				{
					entertainment++;
				}
				break;
			case Statistic.Hygiene:
				if(hygiene < maxStatisticValue)
				{
					hygiene++;
				}
				break;
			case Statistic.Bladder:
				if(bladder < maxStatisticValue)
				{
					bladder++;
				}
				break;
			case Statistic.Energy:
				if(energy < maxStatisticValue)
				{
					energy++;
				}
				break;
		}
	}

	public void DecreaseStatistic(Statistic statistic)
	{
		switch(statistic)
		{
			case Statistic.Hunger:
				if(hunger > 0)
				{
					hunger--;
				}
				break;
			case Statistic.Entertainment:
				if(entertainment > 0)
				{
					entertainment--;
				}
				break;
			case Statistic.Hygiene:
				if(hygiene > 0)
				{
					hygiene--;
				}
				break;
			case Statistic.Bladder:
				if(bladder > 0)
				{
					bladder--;
				}
				break;
			case Statistic.Energy:
				if(energy > 0)
				{
					energy--;
				}
				break;
		}
	}

	private IEnumerator DecreaseStatisticsOverTime()
	{
		while(true)
		{
			yield return new WaitForSeconds(statisticsDecreaseTime);

			if(hunger > 0)
			{
				hunger--;
			}
			if(entertainment > 0)
			{
				entertainment--;
			}
			if(hygiene > 0)
			{
				hygiene--;
			}
			if(bladder > 0)
			{
				bladder--;
			}
			if(energy > 0)
			{
				energy--;
			}

			if(hunger == 0 && entertainment == 0 && hygiene == 0 && bladder == 0 && energy == 0)
			{
				// TODO: Play death anim and restart the scene.
				Debug.Log("DEAD!!!");
				break;
			}
		}
	}
}
