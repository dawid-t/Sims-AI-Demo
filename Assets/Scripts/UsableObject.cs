using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UsableObject : MonoBehaviour
{
	[SerializeField]
	protected int usableTime, delayedUsableTime;
	[SerializeField] [Range(-5, 5)]
	protected int hungerChanger, entertainmentChanger, hygieneChanger, bladderChanger, energyChanger;
	[SerializeField]
	protected string objectInteractionInfo, startPlayerAnimationTriggerName, endPlayerAnimationTriggerName;
	[SerializeField]
	protected int startPlayerAnimationRotation;
	[SerializeField]
	protected Transform startUsingObjectPosition;
	[SerializeField]
	protected Sprite interactionButtonIcon;
	protected Animator animator;
	protected Coroutine usingObjectCoroutine;


	public string ObjectInteractionInfo => objectInteractionInfo;
	public Transform StartUsingObjectPosition => startUsingObjectPosition;
	public Sprite InteractionButtonIcon => interactionButtonIcon;


	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void StartUsingObject(GameObject player)
	{
		if(usingObjectCoroutine == null)
		{
			ResetPlayerAnimationTriggers(player);
			usingObjectCoroutine = StartCoroutine(UsingObject(player));
		}
	}

	private void ResetPlayerAnimationTriggers(GameObject player)
	{
		Animator playerAnimator = player.GetComponent<Animator>();
		playerAnimator.ResetTrigger(startPlayerAnimationTriggerName);
		playerAnimator.ResetTrigger(endPlayerAnimationTriggerName);
	}

	protected IEnumerator UsingObject(GameObject player)
	{
		// Wait for the player to reach the correct position:
		while((player.transform.position - startUsingObjectPosition.position).magnitude > 0.1f)
		{
			yield return new WaitForSeconds(0.1f);
		}
		player.transform.position = startUsingObjectPosition.position;

		// Play the player animation:
		StartCoroutine(PlayAnimation(player, true));

		// Player other animation of the usable object:
		PlayUsableObjectAnimation(true);

		// Change the player statistics over time:
		yield return new WaitForSeconds(delayedUsableTime);

		StatisticsUI statisticsUI = StatisticsUI.Instance;
		PlayerStatistics playerStatistics = player.GetComponent<PlayerStatistics>();
		int hungerChangeTime = (hungerChanger != 0) ? usableTime / hungerChanger : 0;
		int entertainmentChangeTime = (entertainmentChanger != 0) ? usableTime / entertainmentChanger : 0;
		int hygieneChangeTime = (hygieneChanger != 0) ? usableTime / hygieneChanger : 0;
		int bladderChangeTime = (bladderChanger != 0) ? usableTime / bladderChanger : 0;
		int energyChangeTime = (energyChanger != 0) ? usableTime / energyChanger : 0;

		int currentUsableTime = 0;
		while(currentUsableTime < usableTime)
		{
			yield return new WaitForSeconds(1);
			currentUsableTime++;

			#region Change the player statistics and their UI:
			if(hungerChangeTime != 0 && currentUsableTime % hungerChangeTime == 0)
			{
				if(hungerChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Hunger);
					statisticsUI.ChangeHungerUI(playerStatistics.Hunger, playerStatistics.MaxStatisticValue);
				}
				else if(hungerChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Hunger);
					statisticsUI.ChangeHungerUI(playerStatistics.Hunger, playerStatistics.MaxStatisticValue);
				}
			}

			if(entertainmentChangeTime != 0 && currentUsableTime % entertainmentChangeTime == 0)
			{
				if(entertainmentChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Entertainment);
					statisticsUI.ChangeEntertainmentUI(playerStatistics.Entertainment, playerStatistics.MaxStatisticValue);
				}
				else if(entertainmentChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Entertainment);
					statisticsUI.ChangeEntertainmentUI(playerStatistics.Entertainment, playerStatistics.MaxStatisticValue);
				}
			}

			if(hygieneChangeTime != 0 && currentUsableTime % hygieneChangeTime == 0)
			{
				if(hygieneChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Hygiene);
					statisticsUI.ChangeHygieneUI(playerStatistics.Hygiene, playerStatistics.MaxStatisticValue);
				}
				else if(hygieneChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Hygiene);
					statisticsUI.ChangeHygieneUI(playerStatistics.Hygiene, playerStatistics.MaxStatisticValue);
				}
			}

			if(bladderChangeTime != 0 && currentUsableTime % bladderChangeTime == 0)
			{
				if(bladderChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Bladder);
					statisticsUI.ChangeBladderUI(playerStatistics.Bladder, playerStatistics.MaxStatisticValue);
				}
				else if(bladderChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Bladder);
					statisticsUI.ChangeBladderUI(playerStatistics.Bladder, playerStatistics.MaxStatisticValue);
				}
			}

			if(energyChangeTime != 0 && currentUsableTime % energyChangeTime == 0)
			{
				if(energyChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Energy);
					statisticsUI.ChangeEnergyUI(playerStatistics.Energy, playerStatistics.MaxStatisticValue);
				}
				else if(energyChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Energy);
					statisticsUI.ChangeEnergyUI(playerStatistics.Energy, playerStatistics.MaxStatisticValue);
				}
			}
			#endregion Change the player statistics and their UI.
		}

		// End using the object:
		StopUsingObject(player);
	}

	public void StopUsingObject(GameObject player)
	{
		if(usingObjectCoroutine != null)
		{
			StopCoroutine(usingObjectCoroutine);
			usingObjectCoroutine = null;
			
			StartCoroutine(PlayAnimation(player, false));
		}
	}

	private IEnumerator PlayAnimation(GameObject player, bool startUsingObject)
	{
		Animator playerAnimator = player.GetComponent<Animator>();
		playerAnimator.SetTrigger(startUsingObject ? startPlayerAnimationTriggerName : endPlayerAnimationTriggerName);
		PlayUsableObjectAnimation(startUsingObject);

		if(!startUsingObject)
		{
			while(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
			{
				yield return null; // Wait for the Idle animation.
			}
		}

		player.GetComponent<NavMeshAgent>().enabled = !startUsingObject;
		playerAnimator.applyRootMotion = startUsingObject;

		if(startUsingObject)
		{
			player.transform.rotation = Quaternion.Euler(0, startPlayerAnimationRotation, 0);
		}

		if(!startUsingObject)
		{
			yield return null;
			InteractionsUI.Instance.StartInteraction();
		}

		Debug.Log("position: "+player.transform.position);
		Debug.Log("rotation: "+player.transform.rotation.eulerAngles);
	}

	private void PlayUsableObjectAnimation(bool startUsingObject)
	{
		if(animator != null)
		{
			animator.SetTrigger(startUsingObject ? "StartInteraction" : "EndInteraction");
		}
	}
}
