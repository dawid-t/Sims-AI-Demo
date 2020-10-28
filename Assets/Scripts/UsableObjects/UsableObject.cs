using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UsableObject : MonoBehaviour
{
	[SerializeField]
	private int usableTime, delayedUsableTime;
	[SerializeField] [Range(-5, 5)]
	private int hungerChanger, entertainmentChanger, hygieneChanger, bladderChanger, energyChanger;
	[SerializeField]
	private string objectInteractionInfo, startPlayerAnimationTriggerName, endPlayerAnimationTriggerName;
	[SerializeField]
	private int startPlayerAnimationRotation;
	[SerializeField]
	private Transform startUsingObjectPosition;
	[SerializeField]
	private Sprite interactionButtonIcon;
	private Animator animator;
	private Coroutine usingObjectCoroutine;


	public int UsableTime => usableTime;
	public int DelayedUsableTime => delayedUsableTime;
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
			ResetAnimationTriggers(player);
			usingObjectCoroutine = StartCoroutine(UsingObject(player));
		}
	}

	private void ResetAnimationTriggers(GameObject player)
	{
		Animator playerAnimator = player.GetComponent<Animator>();
		playerAnimator.ResetTrigger(startPlayerAnimationTriggerName);
		playerAnimator.ResetTrigger(endPlayerAnimationTriggerName);

		if(animator != null)
		{
			animator.ResetTrigger("StartInteraction");
			animator.ResetTrigger("EndInteraction");
		}
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

			#region Change the player statistics:
			if(hungerChangeTime != 0 && currentUsableTime % hungerChangeTime == 0)
			{
				if(hungerChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Hunger);
				}
				else if(hungerChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Hunger);
				}
			}

			if(entertainmentChangeTime != 0 && currentUsableTime % entertainmentChangeTime == 0)
			{
				if(entertainmentChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Entertainment);
				}
				else if(entertainmentChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Entertainment);
				}
			}

			if(hygieneChangeTime != 0 && currentUsableTime % hygieneChangeTime == 0)
			{
				if(hygieneChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Hygiene);
				}
				else if(hygieneChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Hygiene);
				}
			}

			if(bladderChangeTime != 0 && currentUsableTime % bladderChangeTime == 0)
			{
				if(bladderChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Bladder);
				}
				else if(bladderChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Bladder);
				}
			}

			if(energyChangeTime != 0 && currentUsableTime % energyChangeTime == 0)
			{
				if(energyChanger > 0)
				{
					playerStatistics.IncreaseStatistic(PlayerStatistics.Statistic.Energy);
				}
				else if(energyChanger < 0)
				{
					playerStatistics.DecreaseStatistic(PlayerStatistics.Statistic.Energy);
				}
			}
			#endregion Change the player statistics.
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
	}

	private void PlayUsableObjectAnimation(bool startUsingObject)
	{
		if(animator != null)
		{
			animator.SetTrigger(startUsingObject ? "StartInteraction" : "EndInteraction");
		}
	}
}
