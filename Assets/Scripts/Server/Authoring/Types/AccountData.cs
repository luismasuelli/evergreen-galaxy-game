namespace Server.Authoring.Types
{
    using AlephVault.Unity.Binary;
    using AlephVault.Unity.Binary.Wrappers;
    using AlephVault.Unity.Meetgard.Auth.Types;
    using Protocols.Messages;

    public class AccountData : IRecordWithPreview<string, AccountPreviewData>
    {
        /**
         * The account full data contains the data that is relevant
         * to the game, but is not needed in client front-end at all.
         *
         * This data typically holds the same data as the preview,
         * and also more data, depending on the needs. It also has
         * an ID (for internal purposes), typically related to some
         * sort of external storage.
         */
         
        public string Username;

        public string GetID()
        {
            // This method NEEDS to be implemented!
            return default(string);
        }

        public AccountPreviewData GetProfileDisplayData()
        {
            // This method can be changed (actually, it must depending
            // on the changes applied to AccountPreviewData.
            return new AccountPreviewData() { Username = Username };
        }
    }
}
