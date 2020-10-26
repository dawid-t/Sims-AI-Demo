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
	protected Animator animator;
	protected Coroutine usingObjectCoroutine;


	public string ObjectInteractionInfo => objectInteractionInfo;
	public Transform StartUsingObjectPosition => startUsingObjectPosition;


	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public void StartUsingObject(GameObject player)
	{
		if(usingObjectCoroutine == null)
		{
			usingObjectCoroutine = StartCoroutine(UsingObject(player));
		}
	}

	protected IEnumerator UsingObject(GameObject player)
	{
		// Wait for the player to reach the correct position:
		while((player.transform.position - startUsingObjectPosition.position).magnitude > 0.1f)
		{
			yield return new WaitForSeconds(0.1f);
		}

		// Play the player animation:
		StartCoroutine(PlayAnimations(player, true));

		// You can override the method and add other animations of other objects:
		PlayOtherAnimations();

		// Change the player properties over time:
		yield return new WaitForSeconds(delayedUsableTime);
		int currentUsableTime = 0;
		while(currentUsableTime < usableTime)
		{
			yield return new WaitForSeconds(1);

			// update player properties and UI

			currentUsableTime++;
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
			
			StartCoroutine(PlayAnimations(player, false));
		}
	}

	private IEnumerator PlayAnimations(GameObject player, bool startUsingObject)
	{
		Animator playerAnimator = player.GetComponent<Animator>();
		playerAnimator.SetTrigger(startUsingObject ? startPlayerAnimationTriggerName : endPlayerAnimationTriggerName);

		if(!startUsingObject)
		{
			while(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle0"))
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

		player.GetComponent<PlayerInteraction>().IsPlayerUsingObject = startUsingObject;
	}

	protected virtual void PlayOtherAnimations()
	{

	}
}
