using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionButtonAction : MonoBehaviour
{
	public Action action;
	public Action<int> cancelAction;
	private bool wasUsed = false;


	public bool WasUsed { get => wasUsed; set => wasUsed = value; }
}
