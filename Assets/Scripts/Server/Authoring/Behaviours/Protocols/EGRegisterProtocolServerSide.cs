using System;
using System.Threading.Tasks;
using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using Protocols;
using Protocols.Messages;

namespace Server.Authoring.Behaviours.Protocols
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;
    using AlephVault.Unity.Meetgard.Types;

    public class EGRegisterProtocolServerSide : SimpleRegisterProtocolServerSide<EGRegisterProtocolDefinition, Nothing, RegisterFailed>
    {
        /// <summary>
        ///   Makes the handlers for the register messages.
        /// </summary>
        protected override void SetRegisterMessageHandlers()
        {
            // For each defined register message in the protocol definition,
            // the handler must be created in this method. Since one register
            // message was defined in the protocol definition, one handler
            // is being created in this method.
            //
            // Typically, the handler to add would be for non-logged users.
            //
            // If this component is attached to the same object that has an
            // authentication protocol, you might want to wrap this handler
            // using .LogoutRequired from that component.
            AddRegisterMessageHandler<Register>("Default", async (register) => {
                // This method requires a totally custom implementation
                // from the user.
                //
                // Given the login details, they are either valid or
                // invalid. If the register is valid, then it must return:
                // - A successful response. By default, the successful
                //   response is defined of type Nothing, so the value
                //   will be Nothing.Instance.
                // 
                // return AcceptRegister(successfulReason);
                //
                // Otherwise, for invalid register attempts, a rejection
                // reason must be generated, of type: RegisterFailed.
                //
                // return RejectRegister(unsuccessfulReason);
                
                // WARNING: EVERY CALL TO AN EXTERNAL API OR USING A GAME OBJECT
                //          OR BEHAVIOUR MUST BE DONE IN THE CONTEXT OF A CALL TO
                //          RunInMainThread OR IT WILL SILENTLY FAIL.

                if (register.Password != register.PasswordConfirm)
                {
                    return RejectRegister(new RegisterFailed().WithPasswordMismatchError());
                }

                return RejectRegister(new RegisterFailed().WithNotImplementedReason());
            });
        }
    }
}
