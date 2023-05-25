using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UIFramework
{

    public abstract class UIWIndow : UIPanel
    {
        protected abstract void InitWindow<T>(T? obj) where T : struct;

        public void init<T>(T? obj) where T : struct
        {
            InitWindow(obj);
        }

        public override void Open()
        {
            base.Open();
        }

        public override void Hide(bool isImmediateDestroy = false)
        {
            base.Hide(isImmediateDestroy);

        }

        public void ToCloseWindow()
        {
            UICtrl.CloseWindow(this);
        }
    }
}

