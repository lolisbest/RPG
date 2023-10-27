using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Common;
using System.Linq;
using TMPro;

public class NpcServiceSelectionWindow : MonoBehaviour
{
    //public List<Button> Buttons;
    public Dictionary<NpcService, Button> Buttons;
    public TextMeshProUGUI NpcNameText;
    public GameObject ButtonsRoot;

    public void Initialize()
    {
        Debug.Log("NpcServiceSelectionWindow.Initialize");

        Buttons = new();
        Button[] childButtons = ButtonsRoot.GetComponentsInChildren<Button>();

        foreach (var serviceString in System.Enum.GetNames(typeof(NpcService)))
        {
            NpcService service = Utils.StringToEnum<NpcService>(serviceString);
            Buttons.Add(service, null);
        }

        foreach(var childButton in childButtons)
        {
            if (childButton.name == "Quit")
                continue;

            // 기본적으로 비활성화. Content Size Fitter 컴포넌트를 위해
            childButton.gameObject.SetActive(false);

            // 각각의 Enum 값의 문자열이 버튼의 이름에 포함되는지 => 다른지를 보기위해
            if (!System.Enum.GetNames(typeof(NpcService)).Any(childButton.name.Contains))
                continue;

            NpcService buttonService = Utils.StringToEnum<NpcService>(childButton.name);
            if (Buttons.ContainsKey(Utils.StringToEnum<NpcService>(childButton.name)))
            {
                Buttons[buttonService] = childButton;
            }
        }
    }

    public void SetNpcName(string name)
    {
        NpcNameText.text = name;
    }

    public void SetServices(NpcService services)
    {
        foreach(var serviceString in System.Enum.GetNames(typeof(NpcService)))
        {
            NpcService service = Utils.StringToEnum<NpcService>(serviceString);

            if (services.HasFlag(service) && Buttons[service] != null)
            {
                Debug.Log($"{service} active");
                Buttons[service].gameObject.SetActive(true);
            }
        }
    }

    public void Close()
    {
        foreach(KeyValuePair<NpcService, Button> item in Buttons)
        {
            item.Value.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
}
