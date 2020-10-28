using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionsUI : MonoBehaviour
{
	private static InteractionsUI instance;

	private int maxNumberOfActiveButtons = 12, currentNumberOfActiveButtons = 0;
	[SerializeField]
	private List<Button> interactionButtonsList;


	public static InteractionsUI Instance => instance;


	private void Awake()
	{
		instance = this;
	}

	public void AddInteraction(Action action, Action<int> cancelAction, Sprite buttonIcon, GameObject player)
	{
		if(currentNumberOfActiveButtons < maxNumberOfActiveButtons) // Activate the new button.
		{
			InteractionButtonAction interactionButtonAction = interactionButtonsList[currentNumberOfActiveButtons].GetComponent<InteractionButtonAction>();
			interactionButtonAction.action = action;
			interactionButtonAction.cancelAction = cancelAction;

			interactionButtonsList[currentNumberOfActiveButtons].GetComponent<Image>().sprite = buttonIcon;
			interactionButtonsList[currentNumberOfActiveButtons].gameObject.SetActive(true);

			Animator playerAnimator = player.GetComponent<Animator>();
			if(currentNumberOfActiveButtons == 0 && (playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle") || playerAnimator.GetNextAnimatorStateInfo(0).IsTag("Idle")))
			{
				StartInteraction();
				player.GetComponent<AutoPlayer>().StopAutoPlayer();
			}

			currentNumberOfActiveButtons++;
		}
		else // Change the last button.
		{
			InteractionButtonAction interactionButtonAction = interactionButtonsList[currentNumberOfActiveButtons-1].GetComponent<InteractionButtonAction>();
			interactionButtonAction.action = action;
			interactionButtonAction.cancelAction = cancelAction;

			interactionButtonsList[currentNumberOfActiveButtons-1].GetComponent<Image>().sprite = buttonIcon;
		}
	}

	public void StartInteraction()
	{
		if(interactionButtonsList[0].gameObject.activeInHierarchy)
		{
			InteractionButtonAction interactionButtonAction = interactionButtonsList[0].GetComponent<InteractionButtonAction>();
			
			if(interactionButtonAction.WasUsed)
			{
				DisableButton();
				if(!interactionButtonsList[0].gameObject.activeInHierarchy)
				{
					GameObject.FindWithTag("Player").GetComponent<AutoPlayer>().StartAutoPlayer();
					return;
				}

				interactionButtonAction = interactionButtonsList[0].GetComponent<InteractionButtonAction>();
			}

			interactionButtonAction.action.Invoke();
			interactionButtonAction.WasUsed = true;
		}
		else
		{
			GameObject.FindWithTag("Player").GetComponent<AutoPlayer>().StartAutoPlayer();
		}
	}

	public void OnClickCancelInteractionButton(GameObject go)
	{
		Button clickedButton = go.GetComponent<Button>();
		int buttonIndex = interactionButtonsList.IndexOf(clickedButton);

		InteractionButtonAction interactionButtonAction = clickedButton.GetComponent<InteractionButtonAction>();
		interactionButtonAction.cancelAction.Invoke(buttonIndex);

		DisableButton(clickedButton, interactionButtonAction);
	}

	public void CancelWaitingInteractions(Animator playerAnimator, bool interruptFirstInteraction = false)
	{
		while(currentNumberOfActiveButtons > 0)
		{
			if(!interruptFirstInteraction)
			{
				bool playerIsUsingObject = (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle") && !playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"));
				if(currentNumberOfActiveButtons == 0 || currentNumberOfActiveButtons == 1 && playerIsUsingObject) // Don't cancel first interaction if is in use.
				{
					return;
				}
			}

			Button button = interactionButtonsList[currentNumberOfActiveButtons-1].GetComponent<Button>();
			int buttonIndex = interactionButtonsList.IndexOf(button);

			InteractionButtonAction interactionButtonAction = button.GetComponent<InteractionButtonAction>();
			interactionButtonAction.cancelAction.Invoke(buttonIndex);

			DisableButton(button, interactionButtonAction);
		}
	}

	private void DisableButton(Button button = null, InteractionButtonAction interactionButtonAction = null)
	{
		if(button == null)
		{
			button = interactionButtonsList[0];
			interactionButtonAction = button.GetComponent<InteractionButtonAction>();
		}

		interactionButtonAction.WasUsed = false;
		interactionButtonAction.action = null;
		interactionButtonAction.cancelAction = null;

		button.GetComponent<Image>().sprite = null;
		button.gameObject.SetActive(false);

		button.transform.SetAsLastSibling();
		interactionButtonsList.Remove(button);
		interactionButtonsList.Add(button);

		if(currentNumberOfActiveButtons > 0)
		{
			currentNumberOfActiveButtons--;
		}
	}
}
