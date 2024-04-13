using System;
using AlephVault.Unity.Binary;
using AlephVault.Unity.WindRose.RefMapChars.Authoring.ScriptableObjects;
using Core.Types.Characters;

namespace Models
{
    /// <summary>
    ///   This is the spawn message for the characters.
    ///   It includes all the visual data, and the name.
    /// </summary>
    public class CharacterSpawnData : CharacterCommonData
    {
        /// <summary>
        ///   The short haircut for male.
        /// </summary>
        public ushort MaleShort = 3;

        /// <summary>
        ///   The middle haircut for male.
        /// </summary>
        public ushort MaleMiddle = 1;

        /// <summary>
        ///   The long haircut for male.
        /// </summary>
        public ushort MaleLong = 7;
        
        /// <summary>
        ///   The short haircut for female.
        /// </summary>
        public ushort FemaleShort = 1;

        /// <summary>
        ///   The middle haircut for female.
        /// </summary>
        public ushort FemaleMiddle = 4;

        /// <summary>
        ///   The long haircut for female.
        /// </summary>
        public ushort FemaleLong = 3;
        
        /// <summary>
        ///   The display name.
        /// </summary>
        public string DisplayName;

        /// <summary>
        ///   The sex.
        /// </summary>
        public SexType SexValue;

        /// <summary>
        ///   The race.
        /// </summary>
        public RaceType RaceValue;

        /// <summary>
        ///   The hairstyle.
        /// </summary>
        public HairType HairValue;

        /// <summary>
        ///   The hair color.
        /// </summary>
        public HairColorType HairColorValue;
        
        /// <summary>
        ///   Properly converts the sex code to a RefMap
        ///   sex code.
        /// </summary>
        public override RefMapBundle.SexCode? Sex
        {
            get
            {
                switch (SexValue)
                {
                    case SexType.Female:
                        return RefMapBundle.SexCode.Female;
                    default:
                        return RefMapBundle.SexCode.Male;
                }
            }
        }

        /// <summary>
        ///   Properly converts the race value to a RefMap
        ///   body color. Three body colors are supported.
        /// </summary>
        public override RefMapBody.ColorCode? BodyColor
        {
            get
            {
                switch (RaceValue)
                {
                    case RaceType.White:
                        return RefMapBody.ColorCode.White;
                    case RaceType.Brown:
                        return RefMapBody.ColorCode.Orange;
                    default:
                        return RefMapBody.ColorCode.Black;
                }
            }
        }

        /// <summary>
        ///   Properly mixes the hairstyle and color into a single
        ///   hair field.
        /// </summary>
        public override Tuple<ushort, RefMapAddOn.ColorCode> Hair
        {
            get
            {
                RefMapAddOn.ColorCode hairColor;
                switch (HairColorValue)
                {
                    case HairColorType.Blonde:
                        hairColor = RefMapAddOn.ColorCode.Yellow;
                        break;
                    case HairColorType.Brown:
                        hairColor = RefMapAddOn.ColorCode.DarkBrown;
                        break;
                    default:
                        hairColor = RefMapAddOn.ColorCode.Black;
                        break;
                }

                if (SexValue == SexType.Female)
                {
                    switch (HairValue)
                    {
                        case HairType.Long:
                            return new(FemaleLong, hairColor);
                        case HairType.Middle:
                            return new(FemaleMiddle, hairColor);
                        default:
                            return new(FemaleShort, hairColor);
                    }
                }
                // Male cases here.
                switch (HairValue)
                {
                    case HairType.Long:
                        return new(MaleLong, hairColor);
                    case HairType.Middle:
                        return new(MaleMiddle, hairColor);
                    default:
                        return new(MaleShort, hairColor);
                }
            }
        }

        public override void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref DisplayName);
            serializer.Serialize(ref SexValue);
            serializer.Serialize(ref RaceValue);
            serializer.Serialize(ref HairValue);
            serializer.Serialize(ref HairColorValue);
            serializer.Serialize(ref ClothColorValue);
        }
    }
}
