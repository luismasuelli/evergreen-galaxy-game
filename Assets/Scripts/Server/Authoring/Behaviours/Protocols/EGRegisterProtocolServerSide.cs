using System;
using System.Threading.Tasks;
using AlephVault.Unity.Meetgard.Auth.Protocols.Simple;
using AlephVault.Unity.RemoteStorage.Types.Results;
using Protocols;
using Protocols.Messages;
using Server.Authoring.Behaviours.External;
using Server.Authoring.Behaviours.External.Models;

namespace Server.Authoring.Behaviours.Protocols
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;
    using AlephVault.Unity.Meetgard.Types;

    public class EGRegisterProtocolServerSide : SimpleRegisterProtocolServerSide<EGRegisterProtocolDefinition, Nothing, RegisterFailed>
    {
        private MultiCharAccountAPIClient client = new MultiCharAccountAPIClient(
            "http://localhost:8080"
        );

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
                if (register.Password != register.PasswordConfirm)
                {
                    return RejectRegister(new RegisterFailed().WithPasswordMismatchError());
                }

                Result<MultiCharAccount, string> result = await RunInMainThread(() =>
                    client.RegisterAccount(new MultiCharAccount
                    {
                        Login = register.Username.ToLower().Trim(),
                        Password = register.Password
                    })
                );
                if (result.Code == ResultCode.Ok || result.Code == ResultCode.Created)
                {
                    return AcceptRegister(Nothing.Instance);
                }
                if (result.Code == ResultCode.ValidationError)
                {
                    return RejectRegister(new RegisterFailed().WithValidationError());
                }
                if (result.Code == ResultCode.DuplicateKey)
                {
                    return RejectRegister(new RegisterFailed().WithAccountAlreadyExistsError());
                }
                return RejectRegister(new RegisterFailed().WithUnknownError());
            });
        }
    }
}
