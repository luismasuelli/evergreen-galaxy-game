using AlephVault.Unity.NetRose.Authoring.Behaviours.Server;
using Server.Authoring.Behaviours.NetworkObjects;

namespace Server.Authoring.Behaviours.Protocols
{
    public class PlayerProtocolServerSide : PrincipalObjectsNetRoseProtocolServerSide<CharacterServerSide>
    {
        /**
         * Some method this class provides, and some tips on how to use them.
         *
         * - InstantiatePrincipal(
         *       ulong connectionId, int prefabIndex|string prefabKey, Func<Map> toMap, ushort x, ushort y,
         *       Action<T> beforeAttach = null, Action<T> afterAttach = null
         *   )
         *
         *   This method should be used when the account is ready to have its own principal object
         *   living on a particular scope & map. Typically, this method will be invoked:
         *   - When the game involves an account with only one object, sub-profile or character,
         *     and the user successfully logs in.
         *   - When the game involves an account with many objects, sub-profiles or characters,
         *     and the user -which is already logged in- successfully picks one of them.
         *
         *   The semantics are these:
         *   - This method is executed for a single connectionId. This must occur only once (i.e.
         *     in order to execute it another time, then a prior call to RemovePrincipal(connectionId)
         *     is needed - otherwise, an error will occur) per connectionId.
         *   - There are two versions of this call. One allows identifying the prefab to pick by its
         *     index in the scope-managing protocol server side (and corresponding mirror in the
         *     client side), and the other one allows identifying the prefab to pick by its key name
         *     (this is OPTIONAL for the prefabs to have, but they may).
         *     - The obtained prefab MUST have a behaviour of type: CharacterServerSide.
         *   - A map must be used to attach the new principal to. The map SHOULD belong to a scope
         *     already loaded (be it dynamic scope, or static scope), otherwise the client will
         *     never see the object, and live in Limbo scope).
         *   - Also, the in-map positions must be used, and must be valid. Otherwise, an error will
         *     occur while doing the attachment: the object will never be attached, it will never
         *     belong to a scope, and the client will never be notified of its existence (this CAN be
         *     circumvented, for GetPrincipal(connectionId) will still allow to retrieve the object
         *     and then the back-end can still try attaching it to another map later).
         *   - Two OPTIONAL callbacks can be given to the object.
         *     - One to initialize the object prior to attaching it.
         *     - One to do something after the object was attached.
         *
         *   Notes: InstantiatePrincipal launches tasks that run in the main thread.
         *          When the principal goes into a scope's map, the client updates that scope
         *          accordingly.
         *          This method is PROTECTED. You might want to expose it in your custom mean.
         *
         * - RemovePrincipal(ulong connectionId)
         *
         *   This method removes the principal object for a connection. This must be called only when
         *   the connection already has a principal created for it (e.g. when logging out, or leaving
         *   a character off, perhaps to create a new character or switch to another one). Once the
         *   principal is removed, its connection goes back to Limbo scope.
         *
         *   Notes: This method is PROTECTED. You might want to expose it in your custom mean.
         *
         * - GetPrincipal(ulong connectionId)
         *
         *   Gets the principal for a connection. It is an error if the connection does not have a
         *   principal object.
         *
         * - TryGetPrincipal(ulong connectionId, out CharacterServerSide result)
         *
         *   Tries getting a principal for a connection. It returns true on success (and changes the
         *   value of the result out parameter) and returns false on failure.
         *
         * - HasPrincipal(ulong connectionId)
         *
         *   Tells whether a principal exists for a connection.
         *
         * - Move[Left|Up|Right|Down](ulong connectionId[, bool queue = true])
         *
         *   Starts a movement of the principal of a connection. It is an error if there is no principal
         *   for the connection. If the queue argument is true, and the principal is already moving, it
         *   will keep in memory that an additional movement is requested when the current one ends.
         *   If two or more movements are "queued" like this, only the last will be remembered. If the
         *   queue argument is false and the principal is already moving, then this call will do nothing.
         */
    }
}