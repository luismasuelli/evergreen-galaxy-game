using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.Meetgard.Protocols;

namespace Protocols
{
    using Messages;
    
    public class EGGameProtocolDefinition : ProtocolDefinition
    {
        // Movement in 4 directions.
        public const string MoveDown = "Move:Down";
        public const string MoveLeft = "Move:Left";
        public const string MoveRight = "Move:Right";
        public const string MoveUp = "Move:Up";
        // Saying something.
        public const string Say = "Say";
        // Rotating the cloth.
        public const string ClothRotate = "Cloth:Rotate";

        /// <summary>
        ///   Defines all the messages that can be sent from a client
        ///   to the server, and from the server to a client.
        /// </summary>
        protected override void DefineMessages()
        {
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
