using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static GalaxyBlox.Static.SettingOptions;

namespace GalaxyBlox.Objects
{
    class GameBonus
    {
        public string Name;
        public string SpecialText;

        public BonusType Type;

        public bool Enabled;

        public delegate void Activate(GameBonus bonus);
        Activate activate;

        public delegate void Action();
        Action action;

        public delegate void Update();
        Update update;

        public delegate void Deactivate();
        Deactivate deactivate;

        public GameBonus(Activate activate, Action action, Update update, Deactivate deactivate)
        {
            this.activate = activate;
            this.action = action;
            this.update = update;
            this.deactivate = deactivate;
        }

        public void OnActivate()
        {
            activate?.Invoke(this);
        }

        public void OnAction()
        {
            action?.Invoke();
        }

        public void OnUpdate()
        {
            update?.Invoke();
        }

        public void OnDeactivate()
        {
            deactivate?.Invoke();
        }
    }
}