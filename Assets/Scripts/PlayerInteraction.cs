using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour
{
	private bool isPlayerUsingObject = false;
	private Camera mainCamera;
	private Animator animator;
	private NavMeshAgent navMeshAgent;
	private Coroutine stopWalkAnimationCoroutine;
	private UsableObject lastUsableObject;


	public bool IsPlayerUsingObject { get => isPlayerUsingObject; set => isPlayerUsingObject = value; }


	private void Start()
	{
		mainCamera = Camera.main;
		animator = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		/*if(EventSystem.current.IsPointerOverGameObject()) // If pointer is over the UI then don't do anything.
		{
			return;
		}*/

		CheckHoveredObjectAndInteract(); // Do something if hovered object is interactable.
	}

	private void CheckHoveredObjectAndInteract()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit))
		{
			GameObject hoveredGameObject = hit.transform.gameObject;

			if(!isPlayerUsingObject)
			{
				if(hoveredGameObject.CompareTag("WalkableGround"))
				{
					if(Input.GetMouseButtonUp(0))
					{
						WalkToClickedPoint(hit.point);
					}
				}
				else if(hoveredGameObject.CompareTag("UsableObject") || hoveredGameObject.CompareTag("Player"))
				{
					lastUsableObject = hoveredGameObject.GetComponent<UsableObject>();
					ShowInteractionInfo(lastUsableObject);
					if(Input.GetMouseButtonUp(0))
					{
						WalkToClickedPoint(lastUsableObject.StartUsingObjectPosition.position);
						DoInteraction(lastUsableObject);
					}
				}
			}
			else if(Input.GetMouseButtonUp(0))
			{
				StopInteraction(lastUsableObject); // TEraz sie nadpisuje a ma dodawac sie do kolejki!
			}
		}
	}

	private void WalkToClickedPoint(Vector3 clickedPoint)
	{
		navMeshAgent.destination = clickedPoint;
		if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk") || animator.GetNextAnimatorStateInfo(0).IsName("Idle0"))
		{
			animator.SetTrigger("Walk");
		}

		if(stopWalkAnimationCoroutine != null)
		{
			StopCoroutine(stopWalkAnimationCoroutine);
			stopWalkAnimationCoroutine = null;
		}
		stopWalkAnimationCoroutine = StartCoroutine(StopWalkAnimation());
	}

	private IEnumerator StopWalkAnimation()
	{
		yield return null; // Need to wait 1 frame to update the "navMeshAgent.remainingDistance" value.
		while(navMeshAgent.enabled && navMeshAgent.remainingDistance > 0.5f)
		{
			yield return null;
		}

		animator.SetTrigger("Idle");
		stopWalkAnimationCoroutine = null;
	}

	private void ShowInteractionInfo(UsableObject usableObject)
	{
		//show usableObject.ObjectInteractionInfo
	}

	private void DoInteraction(UsableObject usableObject)
	{
		usableObject.StartUsingObject(gameObject);
	}

	private void StopInteraction(UsableObject usableObject)
	{
		usableObject?.StopUsingObject(gameObject);
	}
}
