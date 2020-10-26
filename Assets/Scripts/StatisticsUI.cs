using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsUI : MonoBehaviour
{
	private static StatisticsUI instance;

	[SerializeField]
	private TextMeshProUGUI hungerPercentsText, entertainmentPercentsText, hygienePercentsText, bladderPercentsText, energyPercentsText;
	[SerializeField]
	private Image hungerBarImage, entertainmentBarImage, hygieneBarImage, bladderBarImage, energyBarImage;


	public static StatisticsUI Instance => instance;


	private void Awake()
	{
		instance = this;
	}

	public void ChangeHungerUI(int statisticValue, int maxStatisticValue)
	{
		float normalizedPercents = GetNormalizedPercents(statisticValue, maxStatisticValue);
		ChangePercents(normalizedPercents, hungerPercentsText);
		ChangeBarColor(normalizedPercents, hungerBarImage);
	}

	public void ChangeEntertainmentUI(int statisticValue, int maxStatisticValue)
	{
		float normalizedPercents = GetNormalizedPercents(statisticValue, maxStatisticValue);
		ChangePercents(normalizedPercents, entertainmentPercentsText);
		ChangeBarColor(normalizedPercents, entertainmentBarImage);
	}

	public void ChangeHygieneUI(int statisticValue, int maxStatisticValue)
	{
		float normalizedPercents = GetNormalizedPercents(statisticValue, maxStatisticValue);
		ChangePercents(normalizedPercents, hygienePercentsText);
		ChangeBarColor(normalizedPercents, hygieneBarImage);
	}

	public void ChangeBladderUI(int statisticValue, int maxStatisticValue)
	{
		float normalizedPercents = GetNormalizedPercents(statisticValue, maxStatisticValue);
		ChangePercents(normalizedPercents, bladderPercentsText);
		ChangeBarColor(normalizedPercents, bladderBarImage);
	}

	public void ChangeEnergyUI(int statisticValue, int maxStatisticValue)
	{
		float normalizedPercents = GetNormalizedPercents(statisticValue, maxStatisticValue);
		ChangePercents(normalizedPercents, energyPercentsText);
		ChangeBarColor(normalizedPercents, energyBarImage);
	}

	private float GetNormalizedPercents(int statisticValue, int maxStatisticValue)
	{
		return (float)statisticValue / maxStatisticValue;
	}

	private void ChangePercents(float normalizedPercents, TextMeshProUGUI statisticText)
	{
		statisticText.text = (normalizedPercents*100)+"%";
	}

	private void ChangeBarColor(float normalizedPercents, Image statisticbarImage)
	{
		float hsvColorPercent = normalizedPercents * 0.32f; // 0% (min) = red, 16% (mid) = yellow, 32% (max) = green.
		statisticbarImage.color = Color.HSVToRGB(hsvColorPercent, 1, 1);
	}
}
