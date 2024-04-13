using System;
using AlephVault.Unity.Binary;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using AlephVault.Unity.WindRose.RefMapChars.Authoring.Behaviours;
using AlephVault.Unity.WindRose.RefMapChars.Authoring.ScriptableObjects;
using AlephVault.Unity.WindRose.RefMapChars.Types;
using Core.Types.Characters;

namespace Models
{
    /// <summary>
    ///   This is common data for both messages.
    ///   It includes a definition of the cloth.
    /// </summary>
    public abstract class CharacterCommonData : ISerializable, IRefMapSimpleModel
    {
        /// <summary>
        ///   The demo cloth. Feel free to modify this.
        /// </summary>
        public const ushort DemoCloth = 1;

        /// <summary>
        ///   The cloth color.
        /// </summary>
        public ClothColorType ClothColorValue;
        
        public virtual RefMapBody.ColorCode? BodyColor => null;
        public virtual RefMapBundle.SexCode? Sex => null;
        public virtual Tuple<ushort, RefMapAddOn.ColorCode> Hair => null;
        public Tuple<ushort, RefMapAddOn.ColorCode> Hat => null;
        public ushort? Necklace => null;
        public ushort? SkilledHandItem => null;
        public ushort? DumbHandItem => null;

        /// <summary>
        ///   Properly converts the cloth color to a specific cloth
        ///   and its color. It also might render no cloth.
        /// </summary>
        public Tuple<ushort, RefMapAddOn.ColorCode> Cloth
        {
            get
            {
                switch (ClothColorValue)
                {
                    case ClothColorType.Black:
                        return new(DemoCloth, RefMapAddOn.ColorCode.Black);
                    case ClothColorType.Blue:
                        return new(DemoCloth, RefMapAddOn.ColorCode.Blue);
                    case ClothColorType.Green:
                        return new(DemoCloth, RefMapAddOn.ColorCode.Green);
                    case ClothColorType.Red:
                        return new(DemoCloth, RefMapAddOn.ColorCode.Red);
                    case ClothColorType.White:
                        return new(DemoCloth, RefMapAddOn.ColorCode.White);
                    default:
                        return new (IRefMapBaseModel.Nothing, RefMapAddOn.ColorCode.Black);
                }
            }
        }
        
        public void ApplyInto(Visual v)
        {
            v.GetComponent<RefMapSimpleModelHolder>().BulkApply(this);
        }
        
        public void ApplyInto(MapObject o)
        {
            ApplyInto(o.MainVisual);
        }

        public abstract void Serialize(Serializer serializer);
    }
}
