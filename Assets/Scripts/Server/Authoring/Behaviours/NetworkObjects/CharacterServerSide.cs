using AlephVault.Unity.Meetgard.Types;
using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.NetRose.Authoring.Behaviours.Server;
using AlephVault.Unity.NetRose.Types.Models;
using Protocols.Messages;
using Models;

namespace Server.Authoring.Behaviours.NetworkObjects
{
    public class CharacterServerSide : OwnedNetRoseModelServerSide<CharacterSpawnData, CharacterRefreshData>
    {
        // THIS BEHAVIOUR IS MEANT TO BE ATTACHED TO MAP OBJECTS
        // THAT ARE MEANT TO BE SYNCHRONIZED THROUGH THE NETWORK.
        // THIS MEANS: OBJECTS THAT SHOULD SEND REAL-TIME UPDATES
        // TO THE CONNECTED CLIENTS.
        //
        // This is a synchronized model for WindRose (NetRose)
        // Map Objects. There are two data types involved here:
        // - The data type used to spawn a new object.
        //   - CharacterSpawnData, in this case.
        // - The data type used to refresh the object.
        //   - CharacterSpawnData, in this case.
        //
        // Typically, one maintains two separate instances for
        // the spawn data type and refresh data type, but it
        // might happen that those references are the same or
        // they have common object references in their structure.
        // Users are totally free to define which instances are
        // used to set the spawn data or the refresh data here,
        // as long as non-null instances are used in each case.
        //
        // Objects are spawned automatically when added to maps
        // in scopes that happen to also be networked.
        //
        // Please, bear in mind: Updating the properties of this
        // object never triggers an automatic updated through
        // the network, but the data is available for the next
        // connection who requests a spawn of the objects, or
        // the next time this object is changed to a new scope.
        // In particular, when the value to be meant as refresh
        // data happens to represent something different (even
        // when the direct reference of the full data doesn't
        // change), a full refresh should be triggered for this
        // model, so the new data is refreshed.
        //
        // Refresh();
        //
        // Or, if it involves a specific refresh context, then:
        //
        // Refresh("some-context");
        
        // It is not forced that two separate instances exist for
        // the data. Actually, the only thing to care about is to
        // return the proper data in the following Get*Data methods.

        public readonly CharacterSpawnData SpawnData = new CharacterSpawnData();
        private OwnedModel<CharacterSpawnData> ownedSpawnData;
        private OwnedModel<CharacterSpawnData> notOwnedSpawnData;
        public CharacterRefreshData RefreshData = new CharacterRefreshData();
        
        protected void Awake()
        {
            base.Awake();
            ownedSpawnData = new OwnedModel<CharacterSpawnData>(true, SpawnData);
            notOwnedSpawnData = new OwnedModel<CharacterSpawnData>(false, SpawnData);
        }
        
        protected override OwnedModel<CharacterSpawnData> GetInnerFullData(ulong connectionId)
        {
            // The spawn data is typically one instance, changing
            // but always holding the whole synchronized object
            // data.
            //
            // This spawn data, still, might be different depending
            // on the target connection (e.g. for owned & private
            // data), depending on whether the target connection
            // is the owner or not.
            return GetOwner() == connectionId ? ownedSpawnData : notOwnedSpawnData;
        }

        protected override CharacterRefreshData GetInnerRefreshData(ulong connectionId, string context)
        {
            // In this example, the whole refresh data is sent, but
            // the implementation might send different data values
            // depending on the context. Typically, the "" context
            // means a *full* refresh, but another context may be
            // chosen (and understood by the client!), and in the
            // data type a custom logic might exist to allow some
            // fields to be null (for the refresh records, their
            // serialization and de-serialization) so partial data
            // may be serialized, depending on the context being
            // specified.
            //
            // In this implementation, the RefreshData does not
            // have a custom logic depending on the context. It
            // is allowed to return different values on different
            // conditions (being consistent on the structure of
            // the values being returned on different contexts!)
            // instead of this full instance for all the contexts.
            //
            // The refresh data, also, might be different depending
            // on the target connection (e.g. for owned & private
            // data), depending on whether the target connection
            // is the owner or not.
            return RefreshData;
        }
    }
}

