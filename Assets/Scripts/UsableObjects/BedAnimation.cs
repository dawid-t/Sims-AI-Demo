using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedAnimation : MonoBehaviour
{
	[SerializeField]
	private Transform daylight;
	private Vector3 daylightOriginalRotation;
	private UsableObject bedUsableObject;
	private Coroutine changeDaylightRotationCoroutine;


	private void Awake()
	{
		bedUsableObject = GetComponent<UsableObject>();
		daylightOriginalRotation = daylight.rotation.eulerAngles;
	}

	public void ChangeDaylightRotation()
	{
		ResetDaylightRotation();
		changeDaylightRotationCoroutine = StartCoroutine(ChangeDaylightRotationOverTime());
	}

	private IEnumerator ChangeDaylightRotationOverTime()
	{
		float interval = 0.02f;
		int changeTime = bedUsableObject.UsableTime - bedUsableObject.DelayedUsableTime; // Subtract because when the character wakes up, the sun has to be like in the beginning.
		int numberOfIntervals = (int)(changeTime / interval);
		float daylightAngleChanger = (float)360 / numberOfIntervals;

		yield return new WaitForSeconds(bedUsableObject.DelayedUsableTime); // Wait before the character goes to sleep.
		for(int i = 0; i < numberOfIntervals; i++)
		{
			yield return new WaitForSeconds(interval);

			daylight.Rotate(daylightAngleChanger, 0, 0);
		}
	}

	private void ResetDaylightRotation()
	{
		if(changeDaylightRotationCoroutine != null)
		{
			StopCoroutine(changeDaylightRotationCoroutine);
			changeDaylightRotationCoroutine = null;
		}
		daylight.rotation = Quaternion.Euler(daylightOriginalRotation);
	}
}
