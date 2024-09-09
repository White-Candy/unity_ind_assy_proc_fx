
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceItem : MonoBehaviour
{
    public TextMeshProUGUI serial; // 序号: A, B, C
    public TextMeshProUGUI topic; // 题目内容
    public Toggle select; // 选择框

    public void Init() {}

    public void Init (int _serial, string _topic)
    {
        char cSerial = (char)(_serial + 65);
        string strSerial = "";
        strSerial += cSerial;

        serial.text = strSerial + ". ";
        topic.text = _topic;
    }

    public void Clear()
    {
        serial.text = "";
        topic.text = "";
        select.isOn = false;
    }
}