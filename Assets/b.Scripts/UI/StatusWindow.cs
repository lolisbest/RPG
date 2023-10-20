using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Common;

public class StatusWindow : MonoBehaviour
{
    public Player @Player;

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI LevelText;

    public TextMeshProUGUI StrText;
    public TextMeshProUGUI EndText;
    public TextMeshProUGUI StaText;
    public TextMeshProUGUI MagText;

    public TextMeshProUGUI AtkText;
    public TextMeshProUGUI DefText;
    public TextMeshProUGUI MaxHpText;
    public TextMeshProUGUI MaxMpText;

    public TextMeshProUGUI LefPointsText;

    private readonly string LevelBaseString = "Level : {0}";

    private readonly string StrBaseString = "Str : {0}";
    private readonly string EndBaseString = "End : {0}";
    private readonly string StaBaseString = "Sta : {0}";
    private readonly string MagBaseString = "Mag : {0}";

    private readonly string AtkBaseString = "Attack : {0}";
    private readonly string DefBaseString = "Defence : {0}";
    private readonly string MaxHpBaseString = "MaxHp : {0}";
    private readonly string MaxMpBaseString = "MaxMp : {0}";

    private readonly string LeftPointsBaseString = "남은 스텟 포인트 : {0}";

    private StructStatus _tempStatus;
    private StructHumanEquipSlots _tempHumanEquipSlots;
    private StructRealStatus _tempRealStatus;

    

    private bool IsChangedTempStatus;

    //private int _formerLeftStatusPoints;

    void Start()
    {
        @Player = Player.Instance;
        IsChangedTempStatus = false;
    }

    public void Open()
    {
        if(@Player == null)
        {
            @Player = Player.Instance;
        }
        UpdateStatus();
        UpdateWindow();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(@Player.IsChangedStatus)
        {
            Debug.Log("UpdateStatusInfo");
            UpdateStatus();
            UpdateWindow();
            @Player.IsChangedStatus = false;
        }

        if(IsChangedTempStatus)
        {
            UpdateWindow();
            IsChangedTempStatus = false;
        }
    }

    private void UpdateStatus()
    {
        _tempStatus = @Player.Status;
        _tempHumanEquipSlots = @Player.HumanEquipSlots;
        _tempRealStatus = IStatus.UpdateRealStatus(_tempStatus, @Player.CurrentEquips());
        IsChangedTempStatus = true;
    }

    private void UpdateWindow()
    {
        NameText.text = _tempStatus.Name;
        LevelText.text = string.Format(LevelBaseString, _tempStatus.Level);

        _tempRealStatus = IStatus.UpdateRealStatus(_tempStatus, @Player.CurrentEquips());

        StrText.text = string.Format(StrBaseString, _tempRealStatus.Str);
        EndText.text = string.Format(EndBaseString, _tempRealStatus.End);
        StaText.text = string.Format(StaBaseString, _tempRealStatus.Sta);
        MagText.text = string.Format(MagBaseString, _tempRealStatus.Mag);

        AtkText.text = string.Format(AtkBaseString, _tempRealStatus.Atk);
        DefText.text = string.Format(DefBaseString, _tempRealStatus.Def);
        MaxHpText.text = string.Format(MaxHpBaseString, _tempRealStatus.MaxHp);
        MaxMpText.text = string.Format(MaxMpBaseString, _tempRealStatus.MaxMp);

        LefPointsText.text = string.Format(LeftPointsBaseString, _tempStatus.LeftStatusPoints);
        //LefPointsText.text = string.Format(LeftPointsBaseString, );
    }

    public void IncreaseStr()
    {
        if (_tempStatus.LeftStatusPoints <= 0)
            return;

        _tempStatus.LeftStatusPoints -= 1;
        StructStatus newStatus = _tempStatus;
        newStatus.Str += 1;
        _tempStatus = newStatus;
        IsChangedTempStatus = true;
    }

    public void DecreaseStr()
    {
        if(@Player.Status.Str < _tempStatus.Str)
        {
            StructStatus newStatus = _tempStatus;
            newStatus.Str -= 1;
            _tempStatus = newStatus;
            _tempStatus.LeftStatusPoints += 1;
            IsChangedTempStatus = true;
        }
    }

    public void IncreaseEnd()
    {
        if (_tempStatus.LeftStatusPoints <= 0)
            return;

        _tempStatus.LeftStatusPoints -= 1;
        StructStatus newStatus = _tempStatus;
        newStatus.End += 1;
        _tempStatus = newStatus;
        IsChangedTempStatus = true;
    }

    public void DecreaseEnd()
    {
        if (@Player.Status.End < _tempStatus.End)
        {
            StructStatus newStatus = _tempStatus;
            newStatus.End -= 1;
            _tempStatus = newStatus;
            _tempStatus.LeftStatusPoints += 1;
            IsChangedTempStatus = true;
        }
    }

    public void IncreaseSta()
    {
        if (_tempStatus.LeftStatusPoints <= 0)
            return;

        _tempStatus.LeftStatusPoints -= 1;
        StructStatus newStatus = _tempStatus;
        newStatus.Sta += 1;
        _tempStatus = newStatus;
        IsChangedTempStatus = true;
    }

    public void DecreaseSta()
    {
        if (@Player.Status.Sta < _tempStatus.Sta)
        {
            StructStatus newStatus = _tempStatus;
            newStatus.Sta -= 1;
            _tempStatus = newStatus;
            _tempStatus.LeftStatusPoints += 1;
            IsChangedTempStatus = true;
        }
    }

    public void IncreaseMag()
    {
        if (_tempStatus.LeftStatusPoints <= 0)
            return;

        _tempStatus.LeftStatusPoints -= 1;
        StructStatus newStatus = _tempStatus;
        newStatus.Mag += 1;
        _tempStatus = newStatus;
        IsChangedTempStatus = true;
    }

    public void DecreaseMag()
    {
        if (@Player.Status.Mag < _tempStatus.Mag)
        {
            StructStatus newStatus = _tempStatus;
            newStatus.Mag -= 1;
            _tempStatus = newStatus;
            _tempStatus.LeftStatusPoints += 1;
            IsChangedTempStatus = true;
        }
    }

    public void ApplyNewStatus()
    {
        @Player.SetStatus(_tempStatus);
    }
}
