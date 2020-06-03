using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldNumberController : MonoBehaviour
{
    public Text goldNumber;

    private void Start()
    {
        EventManager.Register<int>("SetGoldNumber", SetGoldNumber);
    }

    private void SetGoldNumber(int number)
    {
        goldNumber.text = number.ToString();
    }
}
