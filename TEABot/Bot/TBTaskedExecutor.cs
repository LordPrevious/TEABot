using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TEABot.TEAScript;

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
        private readonly object mRunLock = new object();

        /// <summary>
        /// Initialize a tasked executor
        /// </summary>
        /// <param name="a_ct">Used to cancel scheduled tasks</param>
        /// <param name="a_script">The script to execute</param>
        /// <param name="a_arguments">The arguments to the script's parameters</param>
        /// <param name="a_sender">The sender of the message that triggered script execution</param>
        public TBTaskedExecutor(CancellationToken a_ct, TBChannel a_channel, TSCompiledScript a_script, string a_arguments, string a_sender)
            : base(a_script, a_arguments)
        {
            if (a_ct == null)
            {
                throw new ArgumentNullException("a_ct");
            }
            if (a_channel == null)
            {
                throw new ArgumentNullException("a_channel");
            }
            mCt = a_ct;
            mChannel = a_channel;
            mSender = a_sender;
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
            a_context.Values["$sender"] = mSender;
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
