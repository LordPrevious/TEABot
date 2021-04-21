using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Base class for tea script execution managers
    /// </summary>
    public abstract class TSExecutor
    {
        #region Public properties

        /// <summary>
        /// A broadcaster that's used during script executing. Listen on it to receive execution messages including flushes
        /// </summary>
        public readonly TSBroadcaster Broadcaster = new TSBroadcaster();

        #endregion

        #region Constructors, Destructor

        /// <summary>
        /// Initialize an executor for the given script
        /// </summary>
        /// <param name="a_script">The script to execute</param>
        /// <param name="a_arguments">The arguments to the script's parameters</param>
        protected TSExecutor(TSCompiledScript a_script, string a_arguments)
        {
            if (a_script == null)
            {
                throw new ArgumentNullException("a_script");
            }

            mScript = a_script;
            mArguments = a_arguments ?? String.Empty;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Initialize the executor before executing the script, ensures the context is properly set up with metadata and argument values
        /// </summary>
        /// <returns>True iff the executor was initialized properly, false on error, e.g. invalid execution arguments</returns>
        public bool InitializeContext()
        {
            mContext = new TSExecutionContext();
            mContext.Broadcaster = Broadcaster;

            // initialize parameter values from mScript parameter specifications and mArguments
            var argumentParser = new TSScriptArgumentParser(mScript, mArguments);
            if (!argumentParser.ParseIntoContext(mContext))
            {
                return false;
            }

            // initialize metadata values
            InitializeContextValues(mContext);

            return true;
        }

        /// <summary>
        /// Start script execution
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Delay further execution of the script by a given number of seconds.
        /// Should proceed with calling Run() after the delay.
        /// </summary>
        /// <param name="a_delaySeconds">Delay duration in seconds</param>
        public abstract void DelayExecution(long a_delaySeconds);

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Set the metadata values for script execution, e.g. $sender and $self,
        /// including the prefix
        /// </summary>
        /// <param name="a_context">The context in which to set metadata values</param>
        protected abstract void InitializeContextValues(TSExecutionContext a_context);

        /// <summary>
        /// Will be called when execution has completed
        /// </summary>
        protected abstract void ExecutionCompleted();

        #endregion

        #region Protected methods


        /// <summary>
        /// Start or continue execution.
        /// Executes script statements until the script is finished or execution
        /// has to be delayed.
        /// </summary>
        protected void Run()
        {
            while (true)
            {
                if (mNextStatementIndex >= mScript.Statements.Length)
                {
                    // end of script
                    Broadcaster.Error("Reached end of script without end statement at index {0}",
                        mNextStatementIndex);
                    ExecutionCompleted();
                    return;
                }

                // get statement to execute
                var currentStatement = mScript.Statements[mNextStatementIndex];

                // execute statement
                var flow = currentStatement.Execute(mContext);

                // follow control flow
                if (flow is TSFlowNextStatement)
                {
                    // increase index for next statement
                    ++mNextStatementIndex;
                }
                else if (flow is TSFlowDelayNextStatement)
                {
                    // increase index for next statement
                    ++mNextStatementIndex;
                    // delay execution
                    var delayFlow = (TSFlowDelayNextStatement)flow;
                    if (delayFlow.DelayIntervalSeconds > 0)
                    {
                        Broadcaster.Info("Delaying further execution for {0} seconds",
                            delayFlow.DelayIntervalSeconds);
                        DelayExecution(delayFlow.DelayIntervalSeconds);
                        return;
                    }
                }
                else if (flow is TSFlowJumpToLabel)
                {
                    var labelFlow = (TSFlowJumpToLabel)flow;
                    int labelJumpIndex;
                    if (!mScript.Labels.TryGetValue(labelFlow.Label, out labelJumpIndex))
                    {
                        Broadcaster.Error("Encountered jump to unknown label {0} at index {1}, aborting execution",
                            labelFlow.Label,
                            mNextStatementIndex);
                        return;
                    }
                    Broadcaster.Info("Jumping to label {0}, from index {1} to {2}",
                        labelFlow.Label,
                        mNextStatementIndex,
                        labelJumpIndex);
                    mNextStatementIndex = labelJumpIndex;
                }
                else if (flow is TSFlowExit)
                {
                    // orderly script exit
                    Broadcaster.Info("Completed script execution");
                    ExecutionCompleted();
                    return;
                }
                else
                {
                    // unknown control flow
                    Broadcaster.Error("Encountered unsupported control flow {0} at index {1}, aborting execution",
                        flow.GetType(),
                        mNextStatementIndex);
                    ExecutionCompleted();
                    return;
                }
            }
        }

        #endregion

        #region Private data

        /// <summary>
        /// The script to execute
        /// </summary>
        protected readonly TSCompiledScript mScript;

        /// <summary>
        /// The arguments to the script's parameters
        /// </summary>
        protected readonly string mArguments;

        /// <summary>
        /// Index of the next statement in the script from which execution will
        /// proceed upon calling Run()
        /// </summary>
        private int mNextStatementIndex = 0;

        /// <summary>
        /// The execution context to track values and such
        /// </summary>
        private TSExecutionContext mContext = null;

        #endregion
    }
}
