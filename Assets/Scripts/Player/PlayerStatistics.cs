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
	private int hunger, entertainment, hygiene, bladder, energy;


	public int MaxStatisticValue => maxStatisticValue;
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

		StatisticsUI statisticsUI = StatisticsUI.Instance;
		statisticsUI.ChangeHungerUI(hunger, maxStatisticValue);
		statisticsUI.ChangeEntertainmentUI(entertainment, maxStatisticValue);
		statisticsUI.ChangeHygieneUI(hygiene, maxStatisticValue);
		statisticsUI.ChangeBladderUI(bladder, maxStatisticValue);
		statisticsUI.ChangeEnergyUI(energy, maxStatisticValue);
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
					StatisticsUI.Instance.ChangeHungerUI(hunger, maxStatisticValue);
				}
				break;
			case Statistic.Entertainment:
				if(entertainment < maxStatisticValue)
				{
					entertainment++;
					StatisticsUI.Instance.ChangeEntertainmentUI(entertainment, maxStatisticValue);
				}
				break;
			case Statistic.Hygiene:
				if(hygiene < maxStatisticValue)
				{
					hygiene++;
					StatisticsUI.Instance.ChangeHygieneUI(hygiene, maxStatisticValue);
				}
				break;
			case Statistic.Bladder:
				if(bladder < maxStatisticValue)
				{
					bladder++;
					StatisticsUI.Instance.ChangeBladderUI(bladder, maxStatisticValue);
				}
				break;
			case Statistic.Energy:
				if(energy < maxStatisticValue)
				{
					energy++;
					StatisticsUI.Instance.ChangeEnergyUI(energy, maxStatisticValue);
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
					StatisticsUI.Instance.ChangeHungerUI(hunger, maxStatisticValue);
				}
				break;
			case Statistic.Entertainment:
				if(entertainment > 0)
				{
					entertainment--;
					StatisticsUI.Instance.ChangeEntertainmentUI(entertainment, maxStatisticValue);
				}
				break;
			case Statistic.Hygiene:
				if(hygiene > 0)
				{
					hygiene--;
					StatisticsUI.Instance.ChangeHygieneUI(hygiene, maxStatisticValue);
				}
				break;
			case Statistic.Bladder:
				if(bladder > 0)
				{
					bladder--;
					StatisticsUI.Instance.ChangeBladderUI(bladder, maxStatisticValue);
				}
				break;
			case Statistic.Energy:
				if(energy > 0)
				{
					energy--;
					StatisticsUI.Instance.ChangeEnergyUI(energy, maxStatisticValue);
				}
				break;
		}
	}

	private IEnumerator DecreaseStatisticsOverTime()
	{
		StatisticsUI statisticsUI = StatisticsUI.Instance;
		while(true)
		{
			yield return new WaitForSeconds(statisticsDecreaseTime);

			if(hunger > 0)
			{
				hunger--;
				statisticsUI.ChangeHungerUI(hunger, maxStatisticValue);
			}
			if(entertainment > 0)
			{
				entertainment--;
				statisticsUI.ChangeEntertainmentUI(entertainment, maxStatisticValue);
			}
			if(hygiene > 0)
			{
				hygiene--;
				statisticsUI.ChangeHygieneUI(hygiene, maxStatisticValue);
			}
			if(bladder > 0)
			{
				bladder--;
				statisticsUI.ChangeBladderUI(bladder, maxStatisticValue);
			}
			if(energy > 0)
			{
				energy--;
				statisticsUI.ChangeEnergyUI(energy, maxStatisticValue);
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
