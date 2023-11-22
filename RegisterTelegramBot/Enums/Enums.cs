namespace RegBot2
{
    public class Enums
    {

        public enum RegistrationState
        {
            Start,
            Login,
            Email,
            Password,
            EndRegistration,
            Stop
        }
        public enum UserState
        {
            Start,
            InRegistration,
            ConfirmRegistaration,
            MakeLink,
        }
        public enum UserCommandState
        { 
            Default,
            ChangePassword

        }

        public enum UserCommandStateWithLink
        {
            Default,
            ChangeLink,
            StopSearch,
            ContinueSearch,
            CheckRequestHistory,
            ChooseLinkHistory,
            ChooseLinkSaved,
            AddLinkHandle

        }
        public enum UserCommandStateNoLink
        {
            Default,
            AddLink
        }
        public enum UserHasLink
        {
            Yes,
            No
        }
        public enum UserMakeLinkState
        {
            City,
            Name,
            PriceFrom,
            PriceTo,
            Complete

        }
        public enum UserActionsWithLinks
        {
            Default,
            DeleteOne,
            DeleteAll
        }

        public enum SwitchHistorySaved
        {
            Default,
            History,
            Saved
        }



    }
}
