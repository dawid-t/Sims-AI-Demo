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
	private Coroutine StopWalkAnimationCoroutine;


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

			if(hoveredGameObject.CompareTag("WalkableGround"))
			{
				if(Input.GetMouseButtonUp(0))
				{
					WalkToClickedPoint(hit.point);
				}
			}
			else if(hoveredGameObject.CompareTag("UsableObject") || hoveredGameObject.CompareTag("Player"))
			{
				ShowInteractionInfo(hoveredGameObject);

				if(Input.GetMouseButtonUp(0))
				{
					DoInteraction(hoveredGameObject);
				}
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

		if(StopWalkAnimationCoroutine != null)
		{
			StopCoroutine(StopWalkAnimationCoroutine);
			StopWalkAnimationCoroutine = null;
		}
		StartCoroutine(StopWalkAnimation());
	}

	private IEnumerator StopWalkAnimation()
	{
		yield return null; // Need to wait 1 frame to update the "navMeshAgent.remainingDistance" value.
		while(navMeshAgent.remainingDistance > 0.5f)
		{
			yield return null;
		}

		animator.SetTrigger("Idle");
		StopWalkAnimationCoroutine = null;
	}

	private void ShowInteractionInfo(GameObject hoveredGameObject)
	{

	}

	private void DoInteraction(GameObject hoveredGameObject)
	{
		
	}
}
