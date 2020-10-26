using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatistics : MonoBehaviour
{
	[SerializeField]
	private int maxStatisticValue = 10, startStatisticValue = 5;
	[SerializeField] [Tooltip("Set time in seconds.")]
	private int statisticsDecreaseTime = 30;
	private int hunger, entertainment, hygiene, bladder, energy;


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
