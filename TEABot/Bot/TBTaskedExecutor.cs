using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TEABot.TEAScript;
using TEABot.Twitch;

namespace TEABot.Bot
{
    /// <summary>
    /// Tea script executor using System.Theading.Tasks.Task for scheduling
    /// </summary>
    public class TBTaskedExecutor : TSExecutor
    {

        /// <summary>
        /// Event handler for execution completion
        /// </summary>
        /// <param name="sender">The executor who sent the event</param>
        /// <param name="channel">The channel for which this executer ran</param>
        /// <param name="script">The script that was executed</param>
        public delegate void ExecutionCompletedHandler(TBTaskedExecutor sender, TBChannel channel, TSCompiledScript script);

        /// <summary>
        /// Event will be raised when script execution is completed
        /// </summary>
        public event ExecutionCompletedHandler OnExecutionCompleted;

        /// <summary>
        /// Cancellation token to pass to tasks so they can be cancelled
        /// </summary>
        private readonly CancellationToken mCt;

        /// <summary>
        /// The channel for which the script is being executed
        /// </summary>
        private readonly TBChannel mChannel;

        /// <summary>
        /// The sender of the message that triggered script execution
        /// </summary>
        private readonly string mSender;

        /// <summary>
        /// Prevent simultanous execution of tasks, e.g. when setting up a delayed execution
        /// </summary>
        private readonly object mRunLock = new();

        /// <summary>
        /// A storage provider for access to persistent data
        /// </summary>
        private readonly ITSStorage mStorage;

        /// <summary>
        /// Hurler provider
        /// </summary>
        private readonly ITSHurler mHurler;

        /// <summary>
        /// List provider
        /// </summary>
        private readonly ITSListProvider mLists;

        /// <summary>
        /// Optional twitch tags with additional sender or message metadata
        /// </summary>
        private readonly TTwMessageTags mTwitchTags;

        #region Additional list names

        public static readonly string cListNameBadges = TSConstants.SpecialListPrefix + "badges";
        public static readonly string cListNameEmotes = TSConstants.SpecialListPrefix + "emotes";

        #endregion

        /// <summary>
        /// Initialize a tasked executor
        /// </summary>
        /// <param name="a_storage">Storage provider</param>
        /// <param name="a_hurler">Hurler provider</param>
        /// <param name="a_lists">List provider</param>
        /// <param name="a_twitchTags">Optional twitch tags, may be null</param>
        /// <param name="a_channel">Channel the script is executed for</param>
        /// <param name="a_script">The script to execute</param>
        /// <param name="a_arguments">The arguments to the script's parameters</param>
        /// <param name="a_sender">The sender of the message that triggered script execution</param>
        /// <param name="a_ct">Used to cancel scheduled tasks</param>
        public TBTaskedExecutor(ITSStorage a_storage, ITSHurler a_hurler, ITSListProvider a_lists, TTwMessageTags a_twitchTags,
            TBChannel a_channel, TSCompiledScript a_script, string a_arguments, string a_sender,
            CancellationToken a_ct)
            : base(a_script, a_arguments)
        {
            mCt = a_ct;
            mChannel = a_channel ?? throw new ArgumentNullException(nameof(a_channel));
            mSender = a_sender;
            mStorage = a_storage;
            mHurler = a_hurler;
            mLists = a_lists;
            mTwitchTags = a_twitchTags;
        }

        public override void Execute()
        {
            _ = ExecuteTasked();
        }

        public override void DelayExecution(long a_delaySeconds)
        {
            _ = ExecuteTaskDelayed(a_delaySeconds);
        }

        protected override void InitializeContextValues(TSExecutionContext a_context)
        {
            //  context from channel, settings, and triggering message
            a_context.Values["$self"] = mChannel.Configuration.Self;
            a_context.Values["$channel"] = mChannel.Name;
            a_context.Values["$isSuperUser"] = mChannel.Configuration.SuperUsers.Any(su => su.Equals(mSender, StringComparison.InvariantCultureIgnoreCase));
            a_context.Values["$sender"] = mSender;
            a_context.Values["$timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // additional context items
            a_context.Storage = mStorage;
            a_context.Hurler = mHurler;

            // list manager for this execution
            a_context.Lists = new TSListManager(mLists, a_context.Values);
            // here, additional lists for this executor only can be added to a_context.Lists

            // optional display name to override sender name
            a_context.Values["$sender.displayName"] = mTwitchTags?.DisplayName ?? mSender;
            // check for twitch tags
            if (mTwitchTags != null)
            {

                // user badges
                if (mTwitchTags.Badges != null)
                {
                    a_context.Values["$sender.isBroadcaster"] = mTwitchTags.Badges.IsBroadcaster;
                    a_context.Values["$sender.isMod"] = mTwitchTags.Badges.IsModerator;
                    a_context.Values["$sender.isPrivileged"] = mTwitchTags.Badges.IsPrivileged;

                    // list of badges
                    TSValueList badgeList = new();
                    foreach (var badge in mTwitchTags.Badges.Badges)
                    {
                        badgeList.Add(new()
                        {
                            { "name", badge.Name },
                            { "version", badge.Version },
                            { "info", badge.Info }
                        });
                    }
                    a_context.Lists.AddAdditionalList(cListNameBadges, badgeList);
                }

                // check for user color
                if (mTwitchTags.UserColor.HasValue)
                {
                    a_context.Values["$sender.color"] = String.Format("#{0:X2}{1:X2}{2:X2}",
                        mTwitchTags.UserColor.Value.R,
                        mTwitchTags.UserColor.Value.G,
                        mTwitchTags.UserColor.Value.B);
                }

                // message emotes
                if (mTwitchTags.Emotes != null)
                {
                    // list of emotes sorted by occurrence in original message
                    TSValueList emoteList = new();
                    var sortedEmotes = mTwitchTags.Emotes.Emotes.OrderBy(e => e.Start).ToList();
                    foreach (var emote in sortedEmotes)
                    {
                        emoteList.Add(new()
                        {
                            { "id", emote.Id },
                            { "name", emote.Name },
                            { "start", emote.Start },
                            { "end", emote.End }
                        });
                    }
                    a_context.Lists.AddAdditionalList(cListNameEmotes, emoteList);
                }

                // message id
                if (mTwitchTags.MessageId != null)
                {
                    a_context.Values["$message.id"] = mTwitchTags.MessageId;
                }

                // user id
                if (mTwitchTags.UserId != null)
                {
                    a_context.Values["$sender.id"] = mTwitchTags.UserId;
                }
            }
        }

        private void RunLocked()
        {
            lock (mRunLock)
            {
                Run();
            }
        }

        private async Task ExecuteTasked()
        {
            await Task.Run(RunLocked, mCt);
        }

        private async Task ExecuteTaskDelayed(long a_delaySeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(a_delaySeconds), mCt);
            RunLocked();
        }

        protected override void ExecutionCompleted()
        {
            OnExecutionCompleted?.Invoke(this, mChannel, mScript);
        }
    }
}
