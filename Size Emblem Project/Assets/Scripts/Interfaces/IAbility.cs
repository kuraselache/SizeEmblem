using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SizeEmblem.Scripts.Interfaces
{
    public interface IAbility
    {
        uint ID { get; }

        string Name { get; }

        ILocalizationString NameLocal { get; }



    }

}