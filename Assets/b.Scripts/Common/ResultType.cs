using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;

namespace RPG.Common
{
    public enum ResultType
    {
        Success = 0,
        NoEmptySlot,
        WrongSlotIndex,
        WrongItemId,
        // 상점
        // 보유 수보다 판매 수가 더 많음
        SellFaillLackCount,
        RequestInProgress,
        RequestSuccess,
        RequestConnectionError,
        RequestProtocolError,
        RequestDataProcessingError,
        RequestUnknownCase,
        SkillNotEnoughMP,
        SkillOnCoolTime,
        SkillSuccess,

        MouseEventOnObject,

    }
}