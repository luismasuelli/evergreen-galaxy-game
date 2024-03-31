using System;
using AlephVault.Unity.Binary;
using AlephVault.Unity.NetRose.RefMapChars.Types.Models;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using AlephVault.Unity.WindRose.Authoring.Behaviours.Entities.Visuals;
using AlephVault.Unity.WindRose.RefMapChars.Authoring.Behaviours;
using AlephVault.Unity.WindRose.RefMapChars.Authoring.ScriptableObjects;
using AlephVault.Unity.WindRose.RefMapChars.Types;

namespace Models
{
    public class CharacterRefreshData : ISerializable, IRefMapSimpleModel
    {
        // This file is just a suggestion. Edit it at your convenience, but
        // always keep all the properties and methods defined, even if they
        // just return some sort of default values (e.g. you might want to
        // leave the skilled and dumb hand items out of your game: just
        // remove the SkilledHandItemIndex and DumbHandItemIndex and also
        // return null in the SkilledHandItem and DumbHandItem properties).
        
        // Finally, you might want to use this class by composition rather
        // than by inheritance. In this case, take this class as-is and,
        // in the wrapping class, have a field of this type and implement
        // the IRefMapSimpleModel by delegation (and also the ISerializable
        // would typically invoke this class' Serialize method as well).

        public RefMapBody.ColorCode? BodyColorCode;
        public RefMapBundle.SexCode? SexCode;
        public ItemPair HairPair;
        public ItemPair HatPair;
        public ushort? NecklaceIndex;
        public ushort? SkilledHandItemIndex;
        public ushort? DumbHandItemIndex;
        public ItemPair ClothPair;

        public RefMapBody.ColorCode? BodyColor => BodyColorCode;
        public RefMapBundle.SexCode? Sex => SexCode;
        public Tuple<ushort, RefMapAddOn.ColorCode> Hair => HairPair;
        public Tuple<ushort, RefMapAddOn.ColorCode> Hat => HatPair;
        public ushort? Necklace => NecklaceIndex;
        public ushort? SkilledHandItem => SkilledHandItemIndex;
        public ushort? DumbHandItem => DumbHandItemIndex;
        public Tuple<ushort, RefMapAddOn.ColorCode> Cloth => ClothPair;
                
        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref BodyColorCode);
            serializer.Serialize(ref SexCode);
            serializer.Serialize(ref HairPair);
            serializer.Serialize(ref HatPair);
            serializer.Serialize(ref NecklaceIndex);
            serializer.Serialize(ref SkilledHandItemIndex);
            serializer.Serialize(ref DumbHandItemIndex);
            serializer.Serialize(ref ClothPair);
        }
        
        public void ApplyInto(Visual v)
        {
            v.GetComponent<RefMapSimpleModelHolder>().BulkApply(this);
        }
        
        public void ApplyInto(MapObject o)
        {
            ApplyInto(o.MainVisual);
        }
    }
}
