using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.Meetgard.Protocols;

namespace Protocols
{
    using Messages;
    
    public class EGGameProtocolDefinition : ProtocolDefinition
    {
        // Character management commands.

        // Characters listing.
        public const string CharacterList = "Character:List";
        public const string CharacterListOk = "Character:List:Ok";
        public const string CharacterListError = "Character:List:Error";
        
        // Character picking.
        public const string CharacterPick = "Character:Pick";
        public const string CharacterPickOk = "Character:Pick:Ok";
        public const string CharacterPickError = "Character:Pick:Error";

        // Character releasing.
        public const string CharacterRelease = "Character:Release";
        public const string CharacterReleaseResponse = "Character:Release:Response";
        
        // Character creation.
        public const string CharacterCreate = "Character:Create";
        public const string CharacterCreateOk = "Character:Create:Ok";
        public const string CharacterCreateError = "Character:Create:Error";
        
        // One-character commands.
        
        // Movement in 4 directions.
        public const string MoveDown = "Move:Down";
        public const string MoveLeft = "Move:Left";
        public const string MoveRight = "Move:Right";
        public const string MoveUp = "Move:Up";
        // Saying something.
        public const int MaxSayLength = 256;
        public const string Say = "Say";
        // Rotating the cloth.
        public const string ClothRotate = "Cloth:Rotate";

        /// <summary>
        ///   Defines all the messages that can be sent from a client
        ///   to the server, and from the server to a client.
        /// </summary>
        protected override void DefineMessages()
        {
            // Characters listing.
            DefineClientMessage(CharacterList);
            DefineServerMessage<CharactersNamesList>(CharacterListOk);
            DefineServerMessage(CharacterListError);
            
            // Character picking.
            DefineClientMessage<UInt>(CharacterPick);
            DefineServerMessage(CharacterPickOk);
            DefineServerMessage<CharacterPickError>(CharacterPickError);
            
            // Character releasing.
            DefineClientMessage(CharacterRelease);
            DefineServerMessage<Bool>(CharacterReleaseResponse);
            
            // Character creation.
            DefineClientMessage<CharacterCreationData>(CharacterCreate);
            DefineServerMessage(CharacterCreateOk);
            DefineServerMessage<CharacterCreateError>(CharacterCreateError);
            
            // Commands to move in any of the 4 directions:
            DefineClientMessage(MoveDown);
            DefineClientMessage(MoveLeft);
            DefineClientMessage(MoveRight);
            DefineClientMessage(MoveUp);
            // Command to say something:
            DefineClientMessage<String>(Say);
            // Command to rotate the cloth:
            DefineClientMessage(ClothRotate);
        }
    }
}
