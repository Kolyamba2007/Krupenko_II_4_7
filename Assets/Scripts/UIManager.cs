using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text endGameText;

    private void Start()
    {
        StartCoroutine(ShowText("ABOBA"));
    }

    private IEnumerator ShowText(string text)
    {
        //ActionMap.enabled = false;

        endGameText.text = text;
        endGameText.enabled = true;

        yield return new WaitForSeconds(3);

        endGameText.enabled = false;
        //ActionMap.enabled = true;
        yield break;
    }
}
