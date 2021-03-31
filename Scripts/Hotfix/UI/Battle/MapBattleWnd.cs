using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using ETModel;
using System.Collections.Generic;

namespace ETHotfix
{
    public class MapBattleWnd : UIWndLogic
    {
        public override UIWnd UIWnd
        {
            get
            {
                return UIWnd.MapWnd;
            }
        }

        private Transform Input;
        private GameObject Attbtn;
        private GameObject Defbtn;
        private GameObject Skillbtn;
        private GameObject Itembtn;
        private GameObject Runbtn;

        public Transform Monster { get; private set; }
        public BattleRole battleRole  { get; private set; }

        public override void Find()
        {
            Input = transform.Find("Input");
            Attbtn = Input.Find("Att").gameObject;
            Defbtn = Input.Find("Def").gameObject;
            Skillbtn = Input.Find("Skill").gameObject;
            Itembtn = Input.Find("Item").gameObject;
            Runbtn = Input.Find("Run").gameObject;
            Monster = transform.Find("Monster/monster2");
            battleRole = new BattleRole(transform.Find("Role/Role"), this);
        }

        public override void Event()
        {
            UIEventListener.Get(Attbtn).onClick = AttBtnOnclick;
            UIEventListener.Get(Skillbtn).onClick = SkillBtnOnclick;
            UIEventListener.Get(Runbtn).onClick = RunBtnOnclick;
            UIEventListener.Get(Defbtn).onClick = CloseUI;
        }

        public override void OnOpen(object userdata)
        {
            
        }

        public override void OnUpdate()
        {
           
        }

        public void AttBtnOnclick(GameObject go)
        {
            battleRole.Att();
        }
        public void SkillBtnOnclick(GameObject go)
        {
            battleRole.isCanAction = true;
            battleRole.CanAction();
        }
        public void RunBtnOnclick(GameObject go)
        {
            battleRole.isCanAction = false;
        }
    }
}