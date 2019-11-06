using SizeEmblem.Scripts.Constants;
using SizeEmblem.Scripts.Interfaces;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Scripts.GameUnits
{
    [CreateAssetMenu(fileName = "New Unit Data", menuName = "Game Data/Unit Data", order = 1)]
    public class GameUnitData : ScriptableObject, IUnitData
    {
        // Base unit name
        public string unitName;
        public string UnitName { get { return unitName; } }


        // Unit categorization
        [Header("Categories")]
        public SizeCategory sizeCategory;
        public SizeCategory SizeCategory { get { return sizeCategory; } }

        public UnitCategory unitCategory;
        public UnitCategory UnitCategory { get { return unitCategory; } }

        public WeaponAdvantageCategory weaponType;
        public WeaponAdvantageCategory WeaponType { get { return weaponType; } }

        // Movement attributes
        [Header("Movement")]
        public MovementType baseMovementType;
        public MovementType BaseMovementType { get { return baseMovementType; } }



        [Range(0,100)]
        public int movementSpeed;
        public int MovementSpeed { get { return movementSpeed; } }


        // Base Attributes
        [Header("Base Attributes")]
        [Range(1, 100)]
        public int initialLevel;
        public int InitialLevel { get { return initialLevel; } }

        [Range(1, 10000)]
        public int hp;
        public int HP { get { return hp; } }

        [Range(1, 10000)]
        public int sp;
        public int SP { get { return sp; } }

        [Range(0, 10000)]
        public int strength;
        public int Strength { get { return strength; } }

        [Range(0, 10000)]
        public int magic;
        public int Magic { get { return magic; } }

        [Range(0, 10000)]
        public int skill;
        public int Skill { get { return Skill; } }

        [Range(0, 10000)]
        public int speed;
        public int Speed { get { return speed; } }

        [Range(0, 10000)]
        public int defense;
        public int Defense { get { return defense; } }

        [Range(0, 10000)]
        public int resistance;
        public int Resistance { get { return resistance; } }

        [Range(0, 10000)]
        public int luck;
        public int Luck { get { return luck; } }

        [Range(0, 10000)]
        public int bonusWalkingDamage;
        public int BonusWalkingDamage { get { return bonusWalkingDamage; } }

        [Range(0, 10000)]
        public int bonusWalkingDamageRange;
        public int BonusWalkingDamageRange { get { return bonusWalkingDamage; } }

        // Details
        [Header("Details")]

        [Tooltip("Height in meters")]
        public double height;
        public double Height { get { return height; } }


        // Unit base dimensions
        [Header("Dimensions")]
        [Range(1, 10)]
        public int tileWidth;
        public int TileWidth { get { return tileWidth; } }


        [Range(1, 10)]
        public int tileHeight;
        public int TileHeight { get { return tileHeight; } }

        // Graphics
        public Sprite unitSprite;
    }
}
