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
            NpcService service = RPG.Utils.EnumParse.StringToEnum<NpcService>(serviceString);
            Buttons.Add(service, null);
        }

        foreach(var childButton in childButtons)
        {
            if (childButton.name == "Quit")
                continue;

            // 현재 버튼은 판넬로 감싸져있음
            // 기본적으로 비활성화. Content Size Fitter 컴포넌트를 위해
            childButton.transform.parent.gameObject.SetActive(false);

            // 각각의 Enum 값의 문자열이 버튼의 이름에 포함되는지 => 다른지를 보기위해
            if (!System.Enum.GetNames(typeof(NpcService)).Any(childButton.name.Contains))
                continue;

            NpcService buttonService = RPG.Utils.EnumParse.StringToEnum<NpcService>(childButton.name);
            if (Buttons.ContainsKey(RPG.Utils.EnumParse.StringToEnum<NpcService>(childButton.name)))
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
            NpcService service = RPG.Utils.EnumParse.StringToEnum<NpcService>(serviceString);

            if (services.HasFlag(service) && Buttons[service] != null)
            {
                // 현재 버튼은 판넬로 감싸져있음
                Buttons[service].transform.parent.gameObject.SetActive(true);
            }
        }
    }

    public void Close()
    {
        foreach(KeyValuePair<NpcService, Button> item in Buttons)
        {
            // 현재 버튼은 판넬로 감싸져있음
            item.Value.transform.parent.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
}
