using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using AlephVault.Unity.RemoteStorage.Types.Results;
using AlephVault.Unity.Support.Generic.Authoring.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server.Authoring.Behaviours.External
{
    using Models;
    
    public class MultiCharAccountAPIClient
    {
        private class DropData
        {
            [JsonProperty("from")]
            public uint From;

            [JsonProperty("drops")]
            public int[][][] Drops;
        }

        /**
         * BEWARE! USE THESE FUNCTIONS ONLY IN THE CONTEXT OF THE MAIN THREAD.
         * As an example, inside a NetRose protocol's RunInMainThread method.
         */
        
        private const string TOKEN = "sample-abcdef";
        private Root Root;
        private ListResource<MultiCharAccount, MultiCharAccount> Accounts;
        private ListResource<Character, Character> Characters;
        private ListResource<Scope, Scope> Scopes;
        private ListResource<Map, Map> Maps;

        public MultiCharAccountAPIClient(string endpoint)
        {
            Root = new Root(endpoint, new Authorization("Bearer", TOKEN));
            Accounts = (ListResource<MultiCharAccount, MultiCharAccount>)Root.GetList<MultiCharAccount, MultiCharAccount>("accounts");
            Characters = (ListResource<Character, Character>)Root.GetList<Character, Character>("characters");
            Scopes = (ListResource<Scope, Scope>)Root.GetList<Scope, Scope>("scopes");
            Maps = (ListResource<Map, Map>)Root.GetList<Map, Map>("maps");
        }

        /// <summary>
        ///   Attempts to find a user by its login field.
        /// </summary>
        /// <param name="login">The login to lookup by</param>
        /// <returns>A storage account-related result</returns>
        /// <remarks>
        ///   On success, the resulting account will also bring its password.
        ///   Take enough care with it (e.g. ensure it is encrypted or hashed).
        /// </remarks>
        public Task<Result<MultiCharAccount, string>> FindAccountByLogin(string login)
        {
            return Accounts.ViewTo<MultiCharAccount>("by-login", new Dictionary<string, string>
            {
                {"login", login}
            });
        }

        /// <summary>
        ///   Attempts to register a new user.
        /// </summary>
        /// <param name="account">The account data</param>
        /// <returns>A storage account-related result</returns>
        /// <remarks>
        ///   The account body contains a password. Using a secure connection
        ///   for your game is strongly advised.
        /// </remarks>
        public Task<Result<MultiCharAccount, string>> RegisterAccount(MultiCharAccount account)
        {
            return Accounts.Create(account);
        }
        
        /// <summary>
        ///   Attempts to update an existing user.
        /// </summary>
        /// <param name="accountId">The id of the account to update</param>
        /// <param name="changes">The changes to apply</param>
        /// <returns>A storage account-related results (actually, the account is never sent)</returns>
        /// <remarks>
        ///   The changes may contains a password. Using a secure connection
        ///   for your game is strongly advised.
        /// </remarks>
        public Task<Result<MultiCharAccount, string>> UpdateAccount(string accountId, JObject changes)
        {
            return Accounts.Update(accountId, changes);
        }

        /// <summary>
        ///   Attempts to delete an existing user.
        /// </summary>
        /// <param name="accountId">The account to delete</param>
        /// <returns>A storage account-related result (actually, the account is never sent)</returns>
        public Task<Result<MultiCharAccount, string>> DeleteAccount(string accountId)
        {
            return Accounts.Delete(accountId);
        }

        /// <summary>
        ///   Attempts to read an existing account.
        /// </summary>
        /// <param name="accountId">The id of the account to read</param>
        /// <returns>A storage account-related result</returns>
        public Task<Result<MultiCharAccount, string>> GetAccount(string accountId)
        {
            return Accounts.Read(accountId);
        }

        /// <summary>
        ///   Attempts to list all the characters of an account.
        /// </summary>
        /// <param name="lookup">The lookup, either the account id or login</param>
        /// <param name="useId">Whether to lookup by id or by login</param>
        /// <returns>A storage characters-related result</returns>
        public Task<Result<Character[], string>> ListCharacters(string lookup, bool useId = false)
        {
            return Accounts.ViewTo<Character[]>("by-account", new Dictionary<string, string>
            {
                {useId ? "id" : "login", lookup}
            });
        }

        /// <summary>
        ///   Attempts to create a character for an account.
        /// </summary>
        /// <param name="data">The character data</param>
        /// <returns>A storage character-related result (actually, the character is never sent)</returns>
        public Task<Result<Character, string>> CreateCharacter(Character data)
        {
            return Characters.Create(data);
        }

        /// <summary>
        ///   Attempts to update a character data.
        /// </summary>
        /// <param name="characterId">The id of the character to update</param>
        /// <param name="changes">The changes to apply</param>
        /// <returns>A storage character-related result (actually, the character is never sent)</returns>
        public Task<Result<Character, string>> UpdateCharacter(string characterId, JObject changes)
        {
            return Characters.Update(characterId, changes);
        }

        /// <summary>
        ///   Attempts to delete a character.
        /// </summary>
        /// <param name="characterId">The id of the character to delete</param>
        /// <returns>A storage character-related result (actually, the character is never sent)</returns>
        public Task<Result<Character, string>> DeleteCharacter(string characterId)
        {
            return Characters.Delete(characterId);
        }

        /// <summary>
        ///   Attempts to read a character.
        /// </summary>
        /// <param name="characterId">The id of the character to read</param>
        /// <returns>A storage character-related result</returns>
        public Task<Result<Character, string>> GetCharacter(string characterId)
        {
            return Characters.Read(characterId);
        }

        /// <summary>
        ///   Attempts to create a scope (typically, a dynamic/extra one).
        /// </summary>
        /// <param name="data">The scope data</param>
        /// <returns>A storage scope-related result (actually, the scope is never sent)</returns>
        public Task<Result<Scope, string>> CreateScope(Scope data)
        {
            return Scopes.Create(data);
        }

        /// <summary>
        ///   Attempts to update a scope.
        /// </summary>
        /// <param name="scopeId">The id of the scope to update</param>
        /// <param name="changes">The changes to apply</param>
        /// <returns>A storage scope-related result (actually, the scope is never sent)</returns>
        public Task<Result<Scope, string>> UpdateScope(string scopeId, JObject changes)
        {
            return Scopes.Update(scopeId, changes);
        }

        /// <summary>
        ///   Attempts to delete a scope (typically, a dynamic/extra one).
        /// </summary>
        /// <param name="scopeId">The id of the scope to delete</param>
        /// <returns>A storage scope-related result (actually, the scope is never sent)</returns>
        public Task<Result<Scope, string>> DeleteScope(string scopeId)
        {
            return Scopes.Delete(scopeId);
        }

        /// <summary>
        ///   Attempts to read a scope. It does not include map information.
        /// </summary>
        /// <param name="scopeId">The id of the scope to read</param>
        /// <returns>The scope data</returns>
        public Task<Result<Scope, string>> GetScope(string scopeId)
        {
            return Scopes.Read(scopeId);
        }

        /// <summary>
        ///   Attempts to get all the maps references for a given scope.
        ///   By default, only an array (sorted-up by map index) will
        ///   be returned containing bare documents {index, _id}. No
        ///   other information will be retrieved from each map.
        /// </summary>
        /// <param name="lookup">The lookup, either the scope id or key</param>
        /// <param name="useId">Whether to lookup by id or by key</param>
        /// <returns>A storage maps-related result</returns>
        public Task<Result<Map[], string>> ListScopeMaps(string lookup, bool useId = false)
        {
            return Scopes.ViewTo<Map[]>("list-maps", new Dictionary<string, string>
            {
                {useId ? "id" : "scope", lookup}
            });
        }

        /// <summary>
        ///   Attempts to create a map, in a certain scope.
        ///   The creation should never include drop info.
        /// </summary>
        /// <param name="data">The map data</param>
        /// <returns>A storage map-related result (actually, the map is never sent)</returns>
        public Task<Result<Map, string>> CreateMap(Map data)
        {
            return Maps.Create(data);
        }

        /// <summary>
        ///   Attempts to update a map, in a certain scope.
        ///   The update should never include drop info, if the map is big.
        /// </summary>
        /// <param name="mapId">The id of the map to update</param>
        /// <param name="changes">The changes to apply</param>
        /// <returns>A storage map-related result (actually, the map is never sent)</returns>
        public Task<Result<Map, string>> UpdateMap(string mapId, JObject changes)
        {
            return Maps.Update(mapId, changes);
        }

        /// <summary>
        ///   Attempts to delete a map.
        /// </summary>
        /// <param name="mapId">The id of the map to delete</param>
        /// <returns>A storage map-related result (actually, the map is never sent)</returns>
        public Task<Result<Map, string>> DeleteMap(string mapId)
        {
            return Maps.Delete(mapId);
        }

        /// <summary>
        ///   Attempts to read a map. The map will come with the drop layer.
        /// </summary>
        /// <param name="mapId">The id of the map to read</param>
        /// <returns>A storage map-related result</returns>
        public Task<Result<Map, string>> GetMap(string mapId)
        {
            return Maps.Read(mapId);
        }

        /// <summary>
        ///   Attempts to update the map drop. The layout of the data is:
        ///     data[cellIdx][depthIdx] = [itemIdx, amount].
        /// </summary>
        /// <param name="mapId">The id of the map to update</param>
        /// <param name="from">The cell index to start updating from</param>
        /// <param name="data">The data to send</param>
        public Task<Result<JObject, string>> SetMapDrop(string mapId, uint from, int[][][] data)
        {
            return Maps.OperationToJson("set-drop", new System.Collections.Generic.Dictionary<string, string>
            {
                {"id", mapId},
            }, new DropData() { Drops = data, From = from });
        }
    }
}
