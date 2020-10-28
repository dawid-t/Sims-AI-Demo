using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour
{
	private Camera mainCamera;
	private Animator animator;
	private NavMeshAgent navMeshAgent;
	private Coroutine walkToClickedPointCoroutine, stopWalkAnimationCoroutine;
	[SerializeField]
	private UsableObject bed, fridge, toilet, bath, washBasin, tv;
	[SerializeField]
	private Sprite walkInteractionIcon;
	private AudioSource audioSource;


	private void Start()
	{
		mainCamera = Camera.main;
		animator = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if(EventSystem.current.IsPointerOverGameObject()) // If pointer is over the UI then don't do anything.
		{
			return;
		}

		CheckHoveredObjectAndInteract(); // Do something if hovered object is interactable.
	}

	private void CheckHoveredObjectAndInteract()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit))
		{
			GameObject hoveredGameObject = hit.transform.gameObject;

			if(hoveredGameObject.CompareTag("WalkableGround"))
			{
				if(Input.GetMouseButtonUp(0))
				{
					InteractionsUI.Instance.AddInteraction(() =>
					{
						if(walkToClickedPointCoroutine != null)
						{
							StopCoroutine(walkToClickedPointCoroutine);
							StopWalkAnimationCoroutine();
						}
						walkToClickedPointCoroutine = StartCoroutine(WalkToClickedPoint(hit.point, true));
					},
					(interactionButtonIndex) =>
					{
						StopInteraction(null, interactionButtonIndex);
					},
					walkInteractionIcon, gameObject);
				}
			}
			else if(hoveredGameObject.CompareTag("UsableObject"))
			{
				UsableObject lastUsableObject = hoveredGameObject.GetComponent<UsableObject>();
				ShowInteractionInfo(lastUsableObject);
				if(Input.GetMouseButtonUp(0))
				{
					AddInteraction(lastUsableObject);
				}
			}
		}
	}

	public void AddInteraction(UsableObject usableObject)
	{
		InteractionsUI.Instance.AddInteraction(() =>
		{
			if(walkToClickedPointCoroutine != null)
			{
				StopCoroutine(walkToClickedPointCoroutine);
				StopWalkAnimationCoroutine();
			}
			walkToClickedPointCoroutine = StartCoroutine(WalkToClickedPoint(usableObject.StartUsingObjectPosition.position, false));
			StartInteraction(usableObject);
		},
		(interactionButtonIndex) =>
		{
			StopInteraction(usableObject, interactionButtonIndex);
		},
		usableObject.InteractionButtonIcon, gameObject);
	}

	private IEnumerator WalkToClickedPoint(Vector3 clickedPoint, bool isFreeWalk)
	{
		while(!navMeshAgent.enabled)
		{
			yield return null;
		}

		navMeshAgent.destination = clickedPoint;
		if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") || animator.GetNextAnimatorStateInfo(0).IsTag("Idle"))
		{
			animator.SetTrigger("Walk");
		}

		walkToClickedPointCoroutine = null;
		StopWalkAnimationCoroutine();
		stopWalkAnimationCoroutine = StartCoroutine(StopWalkAnimation(isFreeWalk));
	}

	private IEnumerator StopWalkAnimation(bool isFreeWalk)
	{
		yield return null; // Need to wait 1 frame to update the "navMeshAgent.remainingDistance" value.
		while(navMeshAgent.enabled && navMeshAgent.remainingDistance > 0.5f)
		{
			yield return null;
		}

		animator.SetTrigger("Idle");
		stopWalkAnimationCoroutine = null;

		if(isFreeWalk)
		{
			yield return null; // Need to wait 1 frame to update the next animator state info.
			InteractionsUI.Instance.StartInteraction();
		}
	}

	private void StopWalkAnimationCoroutine()
	{
		if(stopWalkAnimationCoroutine != null)
		{
			StopCoroutine(stopWalkAnimationCoroutine);
			stopWalkAnimationCoroutine = null;
		}
	}

	private void ShowInteractionInfo(UsableObject usableObject)
	{
		//show usableObject.ObjectInteractionInfo
	}

	private void StartInteraction(UsableObject usableObject)
	{
		usableObject.StartUsingObject(gameObject);
	}

	private void StopInteraction(UsableObject usableObject, int interactionButtonIndex)
	{
		if(interactionButtonIndex == 0)
		{
			usableObject?.StopUsingObject(gameObject);

			// Stop the walking player:
			if(navMeshAgent.enabled)
			{
				navMeshAgent.destination = transform.position;
			}
		}
	}

	public void PrioritizeInteractions()
	{
		InteractionsUI.Instance.CancelWaitingInteractions(animator);

		PlayerStatistics playerStatistics = GetComponent<PlayerStatistics>();
		Dictionary<float, UsableObject> usableObjectsDictionary = new Dictionary<float, UsableObject>();

		#region Sort by the player statistics and add usable objects to the dictionary:
		// Hunger:
		float key = playerStatistics.Hunger;
		float[] keys = new float[5];
		
		keys[0] = key;
		usableObjectsDictionary.Add(keys[0], fridge);

		// Entertainment:
		key = playerStatistics.Entertainment;
		while(key == keys[0])
		{
			key += 0.1f;
		}
		keys[1] = key;
		usableObjectsDictionary.Add(keys[1], tv);

		// Hygiene:
		key = playerStatistics.Hygiene;
		while(key == keys[0] || key == keys[1])
		{
			key += 0.1f;
		}
		keys[2] = key;
		usableObjectsDictionary.Add(keys[2], (Random.Range(0, 2) == 0) ? washBasin : bath);

		// Bladder:
		key = playerStatistics.Bladder;
		while(key == keys[0] || key == keys[1] || key == keys[2])
		{
			key += 0.1f;
		}
		keys[3] = key;
		usableObjectsDictionary.Add(keys[3], toilet);

		// Energy:
		key = playerStatistics.Energy;
		while(key == keys[0] || key == keys[1] || key == keys[2] || key == keys[3])
		{
			key += 0.1f;
		}
		keys[4] = key;
		usableObjectsDictionary.Add(keys[4], bed);
		#endregion Sort by the player statistics and add usable objects to the dictionary.

		System.Array.Sort(keys);
		for(int i = 0; i < usableObjectsDictionary.Count; i++)
		{
			AddInteraction(usableObjectsDictionary[keys[i]]);
		}
	}

	private void PlayAnimationSound()
	{
		Debug.Log("StartBath: "+animator.GetCurrentAnimatorStateInfo(0).IsName("StartBath"));
		Debug.Log("EndBath: "+animator.GetCurrentAnimatorStateInfo(0).IsName("EndBath"));

		if(audioSource != null)
		{
			if(animator.GetCurrentAnimatorStateInfo(0).IsName("StartBath"))
			{
				audioSource.Play();
			}
			else
			{
				audioSource.Stop();
			}
		}
	}
}
