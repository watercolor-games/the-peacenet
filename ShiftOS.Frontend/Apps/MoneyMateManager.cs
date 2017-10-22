using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plex.Engine;
using Plex.Frontend.GUI;
using Plex.Extras;
using Plex.Frontend.GraphicsSubsystem;
using Microsoft.Xna.Framework;
using Plex.Objects;
using Newtonsoft.Json;
using System.Threading;

namespace Plex.Frontend.Apps
{
    [Launcher("MoneyMate Manager", false, null, "MoneyMate")]
    [WinOpen("moneymatemgr")]
    [DefaultTitle("MoneyMate Manager")]
    [Installer("MoneyMate Manager", "Manage your MoneyMate account, transactions, and much more with a simple graphical user interface for Plexgate! Lifetime satisfaction guaranteed!", 1400 * 1024)]
    public class MoneyMateManager : Control, IPlexWindow
    {
        public PictureBox _moneymate = new PictureBox();
        public ItemGroup _buttonList = new ItemGroup();
        public Button _myAccount = new Button();
        public TextControl _accountTitle = new TextControl();
        public TextControl _accountBalance = new TextControl();
        public ListBox _transactions = new ListBox();
        public Button _transactionsButton = new Button();

        private static CashTransaction[] tdata = null;

        [ClientMessageHandler("moneymate_transactions"), AsyncExecution]
        public static void CashTransactionReceive(string content, string ip)
        {
            tdata = JsonConvert.DeserializeObject<CashTransaction[]>(content);
        }

        public static CashTransaction[] GetCashTransactions()
        {
            //            tdata = null;
            //            ServerManager.SendMessage("moneymate_gettransactions", "");
            //            while (tdata == null)
            //                Thread.Sleep(10);
            //            return tdata;
            return null; //Protocol doesn't support this yet
        }

        private int UIState = 0;

        public MoneyMateManager()
        {
            Width = 600;
            Height = 400;
            AddControl(_moneymate);
            AddControl(_buttonList);
            _buttonList.AddControl(_myAccount);
            _buttonList.AddControl(_transactionsButton);
            _transactionsButton.Text = "Transactions";
            _transactionsButton.AutoSize = true;
            _buttonList.AutoSize = true;
            _myAccount.Text = "My account";
            _myAccount.AutoSize = true;
            AddControl(_accountTitle);
            _accountBalance.AutoSize = true;
            AddControl(_accountBalance);
            _accountTitle.AutoSize = true;
            _myAccount.Click += () => { UIState = 0; };
            _transactionsButton.Click += () =>
            {
                UIState = 1;
                _transactions.ClearItems();

                CashTransaction[] transactions = GetCashTransactions();

                if (transactions.Length == 0)
                {
                    _transactions.AddItem("No transactions to display.");
                    return;
                }
                foreach (var transaction in transactions.OrderByDescending(x => x.Date))
                {
                    _transactions.AddItem($"{transaction.Date}: ${((double)transaction.Amount) / 100} - {transaction.From} -> {transaction.To}");
                }
            };
            AddControl(_transactions);
        }

        public void OnLoad()
        {
            _moneymate.Image = Properties.Resources.moneymate_transparent.ToTexture2D(UIManager.GraphicsDevice);
            _moneymate.ImageLayout = System.Windows.Forms.ImageLayout.Zoom;
        }

        public void OnSkinLoad()
        {
        }

        protected override void OnLayout(GameTime gameTime)
        {
            _moneymate.X = 0;
            _moneymate.Y = 0;
            _moneymate.Width = Width;
            _moneymate.Height = 125;
            _buttonList.MaxWidth = Width - 30;
            _buttonList.X = 15;
            _buttonList.Y = _moneymate.Y + _moneymate.Height + 5;

            _accountTitle.Y = _buttonList.Y + _buttonList.Height + 10;
            _accountTitle.X = 30;
            _accountTitle.MaxWidth = Width - 60;
            _accountTitle.AutoSize = true;
            _accountTitle.Font = SkinEngine.LoadedSkin.HeaderFont;
            _accountTitle.Text = SaveSystem.GetUsername();
            _accountBalance.Visible = UIState == 0;
            _transactions.Visible = UIState == 1;
            if (_accountBalance.Visible)
            {
                _accountBalance.Text = $"Account balance: ${((double)SaveSystem.GetCash()) / 100}";
                _accountBalance.AutoSize = true;
                _accountBalance.Font = SkinEngine.LoadedSkin.Header3Font;
                _accountBalance.Y = _accountTitle.Y + _accountTitle.Height + 5;
                _accountBalance.X = 30;
                _accountBalance.MaxWidth = Width - 60;
            }
            if(_transactions.Visible == true)
            {
                _transactions.X = 30;
                _transactions.Width = Width - 60;
                _transactions.Y = _accountTitle.Y + _accountTitle.Height + 10;
                _transactions.Height = (Height - _transactions.Y) - 30;
            }
            base.OnLayout(gameTime);
        }

        public bool OnUnload()
        {
            return true;
        }

        public void OnUpgrade()
        {
        }
    }
}
