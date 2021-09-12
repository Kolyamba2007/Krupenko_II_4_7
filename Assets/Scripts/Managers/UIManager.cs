using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text endGameText;

    public IEnumerator ShowText(string text, float time)
    {

        endGameText.text = text;
        endGameText.enabled = true;

        yield return new WaitForSeconds(time);

        endGameText.enabled = false;
        yield break;
    }
}