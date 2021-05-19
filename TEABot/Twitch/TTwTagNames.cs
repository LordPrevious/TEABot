using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.Twitch
{
    /// <summary>
    /// Known Twitch IRC tag name constants
    /// </summary>
    static class TTwTagNames
    {
        /// <summary>
        /// Badge metadata
        /// </summary>
        public static readonly string BADGE_INFO = "badge-info";
        /// <summary>
        /// User badges
        /// </summary>
        public static readonly string BADGES = "badges";
        /// <summary>
        /// Duration of a ban for CLEARCHAT messages
        /// </summary>
        public static readonly string BAN_DURATION = "ban-duration";
        /// <summary>
        /// Amount of bits for cheer messages
        /// </summary>
        public static readonly string BITS = "bits";
        /// <summary>
        /// Username of whoever intiiated CLEARMSG
        /// </summary>
        public static readonly string LOGIN = "login";
        /// <summary>
        /// User name or message color
        /// </summary>
        public static readonly string COLOR = "color";
        /// <summary>
        /// User's display name (w/ capitalization info, as opposed to all-lowercase IRC nick)
        /// </summary>
        public static readonly string DISPLAY_NAME = "display-name";
        /// <summary>
        /// List of emotes in a message
        /// </summary>
        public static readonly string EMOTES = "emotes";
        /// <summary>
        /// ROOMSTATE for emote only mode: 1 if enabled, 0 otherwise
        /// </summary>
        public static readonly string EMOTE_ONLY = "emote-only";
        /// <summary>
        /// Sets of emotes the bot user has access to, via GLOBALUSERSTATE or USERSTATE
        /// </summary>
        public static readonly string EMOTE_SETS = "emote-sets";
        /// <summary>
        /// ROOMSTATE for followers only mode: -1 when disabled, 0 when all followers can chat, >0 minimal follow duration in minutes
        /// </summary>
        public static readonly string FOLLOWERS_ONLY = "followers-only";
        /// <summary>
        /// Unique message ID
        /// </summary>
        public static readonly string ID = "id";
        /// <summary>
        /// The message
        /// </summary>
        public static readonly string MESSAGE = "message";
        /// <summary>
        /// 1 if the user is a mod, 0 otherwise
        /// </summary>
        public static readonly string MOD = "mod";
        /// <summary>
        /// USERNOTICE message ID:
        /// sub, resub, subgift, anonsubgift, submysterygift, giftpaidupgrade, rewardgift, anongiftpaidupgrade, raid, unraid, ritual, bitsbadgetier
        /// </summary>
        public static readonly string MSG_ID = "msg-id";
        /// <summary>
        /// USERNOTICE message parameters: Total number of months the user has subscribed
        /// for MSG_ID: sub, resub
        /// </summary>
        public static readonly string MSG_PARAM_CUMULATIVE_MONTHS = "msg-param-cumulative-months";
        /// <summary>
        /// USERNOTICE message parameters: Display name of the source user raiding the channel
        /// for MSG_ID: 
        /// </summary>
        public static readonly string MSG_PARAM_DISPLAYNAME = "msg-param-displayName";
        /// <summary>
        /// USERNOTICE message parameters: IRC nick of the source user raiding the channel
        /// for MSG_ID: raid
        /// </summary>
        public static readonly string MSG_PARAM_LOGIN = "msg-param-login";
        /// <summary>
        /// USERNOTICE message parameters: Total number of months the user has subscribed
        /// for MSG_ID: subgift, anonsubgift
        /// </summary>
        public static readonly string MSG_PARAM_MONTHS = "msg-param-months";
        /// <summary>
        /// USERNOTICE message parameters: Number of subscriptions the gifter has given during the promo event
        /// for MSG_ID: anongiftpaidupgrade, giftpaidupgrade
        /// </summary>
        public static readonly string MSG_PARAM_PROMO_GIFT_TOTAL = "msg-param-promo-gift-total";
        /// <summary>
        /// USERNOTICE message parameters: Subscription promo name, if any
        /// for MSG_ID: anongiftpaidupgrade, giftpaidupgrade
        /// </summary>
        public static readonly string MSG_PARAM_PROMO_NAME = "msg-param-promo-name";
        /// <summary>
        /// USERNOTICE message parameters: Recipient display name
        /// for MSG_ID: subgift, anonsubgift
        /// </summary>
        public static readonly string MSG_PARAM_RECIPIENT_DISPLAY_NAME = "msg-param-recipient-display-name";
        /// <summary>
        /// USERNOTICE message parameters: Recipient user ID
        /// for MSG_ID: subgift, anonsubgift
        /// </summary>
        public static readonly string MSG_PARAM_RECIPIENT_ID = "msg-param-recipient-id";
        /// <summary>
        /// USERNOTICE message parameters: Recipient IRC nick
        /// for MSG_ID: subgift, anonsubgift
        /// </summary>
        public static readonly string MSG_PARAM_RECIPIENT_USER_NAME = "msg-param-recipient-user-name";
        /// <summary>
        /// USERNOTICE message parameters: Original sub gifter IRC nick
        /// for MSG_ID: giftpaidupgrade
        /// </summary>
        public static readonly string MSG_PARAM_SENDER_LOGIN = "msg-param-sender-login";
        /// <summary>
        /// USERNOTICE message parameters: Original sub gifter display name
        /// for MSG_ID: giftpaidupgrade
        /// </summary>
        public static readonly string MSG_PARAM_SENDER_NAME = "msg-param-sender-name";
        /// <summary>
        /// USERNOTICE message parameters: Whether to show streak months (1 yes)
        /// for MSG_ID: sub, resub
        /// </summary>
        public static readonly string MSG_PARAM_SHOULD_SHARE_STREAK = "msg-param-should-share-streak";
        /// <summary>
        /// USERNOTICE message parameters: Number of months in subscription streak
        /// for MSG_ID: sub, resub
        /// </summary>
        public static readonly string MSG_PARAM_STREAK_MONTHS = "msg-param-streak-months";
        /// <summary>
        /// USERNOTICE message parameters: Type of subscription (Prime, 1000, 2000, 3000)
        /// for MSG_ID: sub, resub, subgift, anonsubgift
        /// </summary>
        public static readonly string MSG_PARAM_SUB_PLAN = "msg-param-sub-plan";
        /// <summary>
        /// USERNOTICE message parameters: Name of the subscription plan (may be channel defined)
        /// for MSG_ID: sub, resub, subgift, anonsubgift
        /// </summary>
        public static readonly string MSG_PARAM_SUB_PLAN_NAME = "msg-param-sub-plan-name";
        /// <summary>
        /// USERNOTICE message parameters: Number of raiders
        /// for MSG_ID: raid
        /// </summary>
        public static readonly string MSG_PARAM_VIEWERCOUNT = "msg-param-viewerCount";
        /// <summary>
        /// USERNOTICE message parameters: Name of the ritual
        /// for MSG_ID: ritual
        /// </summary>
        public static readonly string MSG_PARAM_RITUAL_NAME = "msg-param-ritual-name";
        /// <summary>
        /// USERNOTICE message parameters: Bits badge tier the user has earned
        /// for MSG_ID: bitsbadgetier
        /// </summary>
        public static readonly string MSG_PARAM_THRESHOLD = "msg-param-threshold";
        /// <summary>
        /// USERNOTICE message parameters: Number of months gifted
        /// for MSG_ID: subgift, anonsubgift
        /// </summary>
        public static readonly string MSG_PARAM_GIFT_MONTHS = "msg-param-gift-months";
        /// <summary>
        /// ROOMSTATE for r9k mode: 1 if enabled, 0 otherwise
        /// </summary>
        public static readonly string R9K = "r9k";
        /// <summary>
        /// ID of the room the message was received in
        /// </summary>
        public static readonly string ROOM_ID = "room-id";
        /// <summary>
        /// ROOMSTATE for slow mode: Number of seconds between messages
        /// </summary>
        public static readonly string SLOW = "slow";
        /// <summary>
        /// 1 of the user is a subscriber, 0 otherwise
        /// </summary>
        public static readonly string SUBSCRIBER = "subscriber";
        /// <summary>
        /// ROOMSTATE for subscriber only mode: 1 if enabled, 0 otherwise
        /// </summary>
        public static readonly string SUBS_ONLY = "subs-only";
        /// <summary>
        /// System message to print alongside a USERNOTICE
        /// </summary>
        public static readonly string SYSTEM_MSG = "system-msg";
        /// <summary>
        /// ID of the message to clear via CLEARMSG
        /// </summary>
        public static readonly string TARGET_MESSAGE_ID = "target-msg-id";
        /// <summary>
        /// Timestamp when the Twitch server received the message
        /// </summary>
        public static readonly string TMI_SENT_TS = "tmi-sent-ts";
        /// <summary>
        /// 1 of the user has Twitch turbo, 0 otherwise
        /// </summary>
        public static readonly string TURBO = "turbo";
        /// <summary>
        /// The user's ID
        /// </summary>
        public static readonly string USER_ID = "user-id";
        /// <summary>
        /// The user's type, e.g. "mod", "admin"
        /// </summary>
        public static readonly string USER_TYPE = "user-type";
    }
}
