using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TEABot.TEAScript
{
    /// <summary>
    /// Defines the script's control flow after a statement's execution
    /// </summary>
    public interface ITSControlFlow
    {
    }

    public static class TSFlow
    {
        /// <summary>
        /// Continue with next statement in the list
        /// </summary>
        readonly public static TSFlowNextStatement Next;

        /// <summary>
        /// End script execution.
        /// </summary>
        readonly public static TSFlowExit Exit;

        /// <summary>
        /// Jump to the given label
        /// </summary>
        /// <param name="a_label">The label to jump to, including prefix '@'</param>
        /// <returns>A jump control flow.</returns>
        public static TSFlowJumpToLabel JumpToLabel(string a_label)
        {
            return new TSFlowJumpToLabel(a_label);
        }

        /// <summary>
        /// Delay execution of next statement
        /// </summary>
        /// <param name="a_delayIntervalSeconds">The wait interval in seconds</param>
        /// <returns>A jump control flow.</returns>
        public static TSFlowDelayNextStatement DelayNext(long a_delayIntervalSeconds)
        {
            return new TSFlowDelayNextStatement(a_delayIntervalSeconds);
        }
    }

    /// <summary>
    /// Regular control flow: Move on to next statement
    /// </summary>
    readonly public struct TSFlowNextStatement : ITSControlFlow
    {
    }

    /// <summary>
    /// Exit control flow: End of script execution
    /// </summary>
    readonly public struct TSFlowExit : ITSControlFlow
    {
    }

    /// <summary>
    /// Jump control flow: Move on to the statement connected to the given label
    /// </summary>
    readonly public struct TSFlowJumpToLabel : ITSControlFlow
    {
        /// <summary>
        /// The label to jump to, including prefix '@'
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Jump to a specific label
        /// </summary>
        /// <param name="a_label">The label to jump to, including prefix '@'</param>
        public TSFlowJumpToLabel(string a_label)
        {
            Label = a_label;
        }
    }

    /// <summary>
    /// Delayed control flow: Wait a given time interval bevore moving on to next statement.
    /// </summary>
    readonly public struct TSFlowDelayNextStatement : ITSControlFlow
    {
        /// <summary>
        /// The wait interval in seconds
        /// </summary>
        public long DelayIntervalSeconds { get; }

        /// <summary>
        /// Delay execution of next statement
        /// </summary>
        /// <param name="a_delayIntervalSeconds">The wait interval in seconds</param>
        public TSFlowDelayNextStatement(long a_delayIntervalSeconds)
        {
            DelayIntervalSeconds = a_delayIntervalSeconds;
        }
    }
}
