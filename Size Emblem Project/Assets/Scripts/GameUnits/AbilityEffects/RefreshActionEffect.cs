﻿using SizeEmblem.Assets.Scripts.Containers;
using SizeEmblem.Assets.Scripts.Interfaces.GameUnits;
using SizeEmblem.Scripts.Extensions;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.GameUnits.AbilityEffects
{
    public class RefreshActionEffect : IAbilityEffect
    {
        public AbilityResultContainer PreviewResults(AbilityExecuteParameters parameters)
        {
            // This just always succeeds so mark our return container accordingly
            var result = new AbilityResultContainer();
            result.Successful = parameters.Target is IGameUnit;

            return result;
        }

        public AbilityResultContainer ExecuteEffect(AbilityExecuteParameters parameters)
        {
            var result = PreviewResults(parameters);
            if(result.Successful)
            {
                (parameters.Target as IGameUnit)?.ResetActionsConsumed();
            }

            return result;
        }
    }
}
