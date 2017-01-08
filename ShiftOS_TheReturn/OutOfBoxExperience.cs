using Newtonsoft.Json;
using ShiftOS.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using ShiftOS.Objects.ShiftFS;

namespace ShiftOS.Engine
{
    public class OutOfBoxExperience
    {
        private static IOobe _oobe = null;

        public static void Init(IOobe oobe)
        {
            _oobe = oobe;
        }

        public static void Start(Save save)
        {

            if (_oobe == null)
                throw new InvalidOperationException("OOBE frontend not activated! This function can't be used! Please use OutOfBoxExperience.Init() passing an IOobe-implementing object to start the OOBE frontend.");


            _oobe.StartShowing(save);


        }

        public static void PromptForLogin()
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                _oobe.PromptForLogin();
            }));
        }

        public static void ShowSaveTransfer(Save save)
        {
            Desktop.InvokeOnWorkerThread(new Action(() =>
            {
                _oobe.ShowSaveTransfer(save);
                
            }));
        }
    }

    public interface IOobe
    {
        void StartShowing(Save save);
        void ShowSaveTransfer(Save save);
        void PromptForLogin();
    }
}
