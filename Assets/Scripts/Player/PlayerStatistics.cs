using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerStatistics : MonoBehaviour
{
	public enum Statistic { Hunger, Entertainment, Hygiene, Bladder, Energy }


	[SerializeField]
	private int maxStatisticValue = 10, startStatisticValue = 5;
	[SerializeField] [Tooltip("Set time in seconds.")]
	private int statisticsDecreaseTime = 30;
	private int hunger, entertainment, hygiene, bladder, energy;
	private Coroutine decreaseStatisticsOverTimeCoroutine;


	public int MaxStatisticValue => maxStatisticValue;
	public int Hunger => hunger;
	public int Entertainment => entertainment;
	public int Hygiene => hygiene;
	public int Bladder => bladder;
	public int Energy => energy;


	private void Start()
	{
		InitStatistics();
		decreaseStatisticsOverTimeCoroutine = StartCoroutine(DecreaseStatisticsOverTime());
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
		Dead();
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
			Dead();
		}
	}

	private void Dead()
	{
		if(hunger == 0 && entertainment == 0 && hygiene == 0 && bladder == 0 && energy == 0)
		{
			StartCoroutine(DelayedDead());
		}
	}

	private IEnumerator DelayedDead()
	{
		if(decreaseStatisticsOverTimeCoroutine != null)
		{
			StopCoroutine(decreaseStatisticsOverTimeCoroutine);
			decreaseStatisticsOverTimeCoroutine = null;
		}

		Animator playerAnimator = GetComponent<Animator>();
		InteractionsUI.Instance.CancelWaitingInteractions(playerAnimator, true);
		while(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle") && !playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) // If player is using the object then wait.
		{
			yield return null;
		}

		GetComponent<PlayerInteraction>().enabled = false;
		InteractionsUI.Instance.CancelWaitingInteractions(playerAnimator);
		yield return null;

		GetComponent<NavMeshAgent>().enabled = false;
		playerAnimator.applyRootMotion = true;

		playerAnimator.ResetTrigger("Idle");
		playerAnimator.ResetTrigger("Walk");
		yield return null;
		playerAnimator.SetTrigger("Dead");

		yield return new WaitForSeconds(5);

		ChangeSceneEffect.Instance.EndSceneEffect();
		yield return new WaitForSeconds(1.5f);

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
