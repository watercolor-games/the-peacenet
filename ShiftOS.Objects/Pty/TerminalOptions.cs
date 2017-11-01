namespace Plex.Objects.Pty
{
    public class TerminalOptions
    {
        /// <summary>
        /// Gets or sets the output flag
        /// </summary>
        /// <value>The OF lag.</value>
        public uint OFlag
        {
            set;
            get;
        }

        public uint LFlag
        {
            set;
            get;
        }

        public readonly byte[] C_cc = new byte[20];
    }
}
