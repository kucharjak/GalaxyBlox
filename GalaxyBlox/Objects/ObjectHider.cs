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
using GalaxyBlox.Models;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Objects
{
    class ObjectHider : GameObject
    {
        public int HideTimePeriod = 500; // in ms
        public float HideAlpha = 0f;

        public bool IsAllHidden;

        List<HideInfo> objectsToHide;

        public event EventHandler AllHidden;
        protected virtual void OnAllHidden(EventArgs e)
        {
            EventHandler handler = AllHidden;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler AllShown;
        protected virtual void OnAllShown(EventArgs e)
        {
            EventHandler handler = AllShown;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public ObjectHider(Room parentRoom) : base(parentRoom)
        {
            objectsToHide = new List<HideInfo>();
            IsAllHidden = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < objectsToHide.Count(); i++)
            {
                var hideInfo = objectsToHide[i];
                var wasHidding = false;

                if (hideInfo.HideAction == HideAction.Nothing)
                    continue;

                if (hideInfo.HideAction == HideAction.Hidding)
                {
                    wasHidding = true;
                    hideInfo.Timer -= gameTime.ElapsedGameTime.Milliseconds;
                    if (hideInfo.Timer <= 0)
                        hideInfo.HideAction = HideAction.Nothing;
                }

                if (hideInfo.HideAction == HideAction.Showing)
                {
                    wasHidding = false;
                    hideInfo.Timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (hideInfo.Timer >= HideTimePeriod)
                        hideInfo.HideAction = HideAction.Nothing;
                }
                
                hideInfo.Object.Alpha = MathHelper.Clamp(HideAlpha + (hideInfo.ShowAlpha - HideAlpha) * (hideInfo.Timer / (float)HideTimePeriod), HideAlpha, hideInfo.ShowAlpha);

                Vector2 newPos = new Vector2();
                switch (hideInfo.HidePlace)
                {
                    case HidePlace.Top:
                        newPos = new Vector2(hideInfo.ShowPosition.X, (float)(hideInfo.ShowPosition.Y - Math.Abs(hideInfo.HidePosition.Y - hideInfo.ShowPosition.Y) * Math.Pow(1 - hideInfo.Timer / (double)HideTimePeriod, 2))); break;
                    case HidePlace.Bottom:
                        newPos = new Vector2(hideInfo.ShowPosition.X, (float)(hideInfo.ShowPosition.Y + Math.Abs(hideInfo.HidePosition.Y - hideInfo.ShowPosition.Y) * Math.Pow(1 - hideInfo.Timer / (double)HideTimePeriod, 2))); break;
                    case HidePlace.Left:
                        newPos = new Vector2((float)(hideInfo.ShowPosition.X - Math.Abs(hideInfo.HidePosition.X - hideInfo.ShowPosition.X) * Math.Pow(1 - hideInfo.Timer / (float)HideTimePeriod, 2)), hideInfo.ShowPosition.Y); break;
                    case HidePlace.Right:
                        newPos = new Vector2((float)(hideInfo.ShowPosition.X + Math.Abs(hideInfo.HidePosition.X - hideInfo.ShowPosition.X) * Math.Pow(1 - hideInfo.Timer / (float)HideTimePeriod, 2)), hideInfo.ShowPosition.Y); break;

                }
                hideInfo.Object.Position = newPos;

                objectsToHide[i] = hideInfo;

                if (objectsToHide.All(obj => obj.HideAction == HideAction.Nothing))
                {
                    if (wasHidding)
                        OnAllHidden(new EventArgs());
                    else
                        OnAllShown(new EventArgs());
                }
            }
        }

        public void HideObject(GameObject objectToHide, HidePlace hidePlace)
        {
            var hideInfo = new HideInfo()
            {
                Object = objectToHide,
                ShowAlpha = objectToHide.Alpha,
                ShowPosition = objectToHide.Position,
                HidePlace = hidePlace,
                HideAction = HideAction.Nothing,
                Timer = 0
            };

            var hideRect = new Rectangle(
                (int)(ParentRoom.Position.X - ParentRoom.InGameOffsetX),
                (int)(ParentRoom.Position.Y - ParentRoom.InGameOffsetY),
                (int)(ParentRoom.Size.X + 2 * ParentRoom.InGameOffsetX),
                (int)(ParentRoom.Size.Y + 2 * ParentRoom.InGameOffsetY));

            var hideOffset = 10;
            switch (hidePlace)
            {
                case HidePlace.Top:
                    hideInfo.HidePosition = new Vector2(objectToHide.Position.X, hideRect.Y - objectToHide.Size.Y - hideOffset); break;
                case HidePlace.Bottom:
                    hideInfo.HidePosition = new Vector2(objectToHide.Position.X, hideRect.Y + hideRect.Size.Y + hideOffset); break;
                case HidePlace.Left:
                    hideInfo.HidePosition = new Vector2(hideRect.X - objectToHide.Size.X - hideOffset, objectToHide.Position.Y); break; 
                case HidePlace.Right:
                    hideInfo.HidePosition = new Vector2(hideRect.X + hideRect.Size.X + hideOffset, objectToHide.Position.Y); break;
            }

            objectsToHide.Add(hideInfo);
        }

        public void Clear(GameObject objectToClear = null)
        {
            if (objectToClear == null)
            {
                Show(false);
                objectsToHide.Clear();
            }
            else
            {
                if (objectsToHide.Where(info => info.Object == objectToClear).Count() > 0)
                {
                    var objInfo = objectsToHide.Where(info => info.Object == objectToClear).First();

                    objInfo.Object.Alpha = objInfo.ShowAlpha;
                    objInfo.Object.Position = objInfo.ShowPosition;
                    objectsToHide.Remove(objInfo);
                }
            }
        }

        public void Hide(bool animate)
        {
            if (animate)
            {
                for (int i = 0; i < objectsToHide.Count(); i ++)
                {
                    var hideInfo = objectsToHide[i];
                    hideInfo.HideAction = HideAction.Hidding;
                    hideInfo.Timer = HideTimePeriod;
                    objectsToHide[i] = hideInfo;
                }
            } 
            else
            {
                foreach(var obj in objectsToHide)
                {
                    obj.Object.Position = obj.HidePosition;
                    obj.Object.Alpha = HideAlpha;
                }

                OnAllHidden(new EventArgs());
            }

            IsAllHidden = true;
        }

        public void Show(bool animate)
        {
            if (animate)
            {
                for (int i = 0; i < objectsToHide.Count(); i++)
                {
                    var hideInfo = objectsToHide[i];
                    hideInfo.HideAction = HideAction.Showing;
                    hideInfo.Timer = 0;
                    objectsToHide[i] = hideInfo;
                }
            }
            else
            {
                foreach (var obj in objectsToHide)
                {
                    obj.Object.Position = obj.ShowPosition;
                    obj.Object.Alpha = obj.ShowAlpha;
                }

                OnAllShown(new EventArgs());
            }

            IsAllHidden = false;
        }
    }

    struct HideInfo
    {
        public GameObject Object;
        public Vector2 ShowPosition;
        public float ShowAlpha;
        public Vector2 HidePosition;
        public HidePlace HidePlace;
        public HideAction HideAction;
        public int Timer;
    }

    enum HideAction
    {
        Nothing,
        Hidding,
        Showing
    }

    enum HidePlace
    {
        Left,
        Right,
        Top,
        Bottom
    }
}