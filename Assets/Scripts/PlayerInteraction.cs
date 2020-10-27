using System.Collections;
using System.Collections.Generic;
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
	protected Sprite walkInteractionIcon;


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
			InteractionsUI interactionsUI = InteractionsUI.Instance;
			GameObject hoveredGameObject = hit.transform.gameObject;

			if(hoveredGameObject.CompareTag("WalkableGround"))
			{
				if(Input.GetMouseButtonUp(0))
				{
					interactionsUI.AddInteraction(() =>
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
			else if(hoveredGameObject.CompareTag("UsableObject"))// || hoveredGameObject.CompareTag("Player"))
			{
				UsableObject lastUsableObject = hoveredGameObject.GetComponent<UsableObject>();
				ShowInteractionInfo(lastUsableObject);
				if(Input.GetMouseButtonUp(0))
				{
					interactionsUI.AddInteraction(() =>
					{
						if(walkToClickedPointCoroutine != null)
						{
							StopCoroutine(walkToClickedPointCoroutine);
							StopWalkAnimationCoroutine();
						}
						walkToClickedPointCoroutine = StartCoroutine(WalkToClickedPoint(lastUsableObject.StartUsingObjectPosition.position, false));
						StartInteraction(lastUsableObject);
					},
					(interactionButtonIndex) =>
					{
						StopInteraction(lastUsableObject, interactionButtonIndex);
					},
					lastUsableObject.InteractionButtonIcon, gameObject);
				}
			}
		}
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
		usableObject?.StopUsingObject(gameObject);

		// Stop the walking player:
		if(navMeshAgent.enabled && interactionButtonIndex == 0)
		{
			navMeshAgent.destination = transform.position;
		}
	}
}
