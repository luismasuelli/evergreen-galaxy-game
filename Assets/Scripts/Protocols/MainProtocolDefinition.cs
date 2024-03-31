using AlephVault.Unity.Meetgard.Protocols;

namespace Protocols
{
    using Messages;
    
    public class MainProtocolDefinition : ProtocolDefinition
    {
        // Typically, one of these games involves the ability
        // to move in any of the 4 directions:
        public const string MoveDown = "Move:Down";
        public const string MoveLeft = "Move:Left";
        public const string MoveRight = "Move:Right";
        public const string MoveUp = "Move:Up";
        // It might also involve simple commands:
        public const string SomeSimpleCommand = "SimpleCommand:Some";
        // It might also involve commands in certain map & position:
        public const string SomeAimedCommand = "AimedCommand:Some";

        /// <summary>
        ///   Defines all the messages that can be sent from a client
        ///   to the server, and from the server to a client.
        /// </summary>
        protected override void DefineMessages()
        {
            // Typically, one of these games involves the ability
            // to move in any of the 4 directions:
            DefineClientMessage(MoveDown);
            DefineClientMessage(MoveLeft);
            DefineClientMessage(MoveRight);
            DefineClientMessage(MoveUp);
            // It might also involve simple commands:
            DefineClientMessage(SomeSimpleCommand);
            // It might also involve aimed commands:
            DefineClientMessage<AimType>(SomeAimedCommand);
        }
    }
}