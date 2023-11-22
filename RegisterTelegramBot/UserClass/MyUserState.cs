using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegBot2
{
    public class MyUserState
    {
        public Enums.RegistrationState registrationState { get; set; } = Enums.RegistrationState.Start;
        public Enums.UserState userState { get; set; } = Enums.UserState.Start;
        public Enums.UserCommandStateWithLink userCommandStateWithLink { get; set; }
        public Enums.UserCommandStateNoLink userCommandStateNoLink { get; set; }
        public Enums.UserHasLink userHasLink { get; set; } = Enums.UserHasLink.No;
        public Enums.UserMakeLinkState userMakeLinkState { get; set; }
        public Enums.UserCommandState userCommandState { get; set; } = Enums.UserCommandState.Default;
        public Enums.UserActionsWithLinks userActionsWithLinks { get; set; } = Enums.UserActionsWithLinks.Default;

        public Enums.SwitchHistorySaved SwitchHistorySaved { get; set; } = Enums.SwitchHistorySaved.Default;
        public Enums.UserActionsWithLinks UserActionsWithLinks { get; set; } = Enums.UserActionsWithLinks.Default;
    }
}
