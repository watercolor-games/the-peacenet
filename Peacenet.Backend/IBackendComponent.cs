using System;
using Plex.Objects;

namespace Peacenet.Backend
{
    public interface IBackendComponent
    {
        /// <summary>
        /// Initialize any resources needed by this component such as files into cache. 
        /// </summary>
        void Initiate();

        /// <summary>
        /// Ran every so often and when the backend is shut down. This should be where you save any files etc to disk in case things go rye and we crash.
        /// </summary>
        void SafetyCheck();
         
        /// <summary>
        /// Unload any resources used by the component to free up RAM. This is run when the backend is shut down.
        /// </summary>
        void Unload();
    }
}
