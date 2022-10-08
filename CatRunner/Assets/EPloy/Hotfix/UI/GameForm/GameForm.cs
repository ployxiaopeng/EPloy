using DG.Tweening;
using EPloy.Hotfix;
using UnityEngine;
using UnityEngine.EventSystems;
using EPloy.Table;
using EPloy.UI;
using EPloy.Timer;
using EPloy.ECS;
using EPloy.Event;

[UIAttribute(UIName.GameForm)]
public class GameForm : UIForm
{
    private GameFormCode bindingCode;
    private InputCpt inputCpt;
    private float radius = 256;
    private GameFormData formData;
    private IDPack CDUpdateTimer;
    public override void Create()
    {
        bindingCode = GameFormCode.Binding(transform);
        UIEventListener.Get(bindingCode.btnMove).onArgDrag = MoveDrag;
        UIEventListener.Get(bindingCode.btnMove).onClickUp = MoveUp;
        UIEventListener.Get(bindingCode.btnAtt).onClick = AttClick;
        UIEventListener.Get(bindingCode.imgSkill1.gameObject).onClick = Skill1Click;
        UIEventListener.Get(bindingCode.imgSkill2.gameObject).onClick = Skill2Click;
        UIEventListener.Get(bindingCode.imgSkill3.gameObject).onClick = Skill3Click;
        inputCpt = ECSModule.GameScene.GetSingleCpt<InputCpt>();
        formData = GameModule.DataStore.GetDataStore<GameFormData>();

        UIEventListener.Get(bindingCode.btnLogout).onClick = (go) =>
             {
                 GameModule.UI.OpenUIForm(UIName.LoadingForm, UIGroupName.Level1);
                 GameModule.Event.Fire(SwitchSceneEvt.Create("login"));
             };

        UIEventListener.Get(bindingCode.btnAStart).onClick = (go) =>
        {
            inputCpt.inputType = UserClrType.Pathfinding;
            inputCpt.targetPos = GameObject.Find("Cube").transform.localPosition;
        };
    }
    public override void Open(object userData)
    {
        inputCpt.inputType = UserClrType.None;
        SetSkill();

    }

    private void SetSkill()
    {
        bindingCode.imgSkill0.SetSprite(formData.Skills[0].Icon);
        bindingCode.imgSkill1.SetSprite(formData.Skills[1].Icon);
        bindingCode.imgMask1.fillAmount = 0;
        bindingCode.imgSkill2.SetSprite(formData.Skills[2].Icon);
        bindingCode.imgMask2.fillAmount = 0;
        bindingCode.imgSkill3.SetSprite(formData.Skills[3].Icon);
        bindingCode.imgMask3.fillAmount = 0;

        CDUpdateTimer = GameModule.Timer.InTimer(0.2f, SkillCDUpdate, int.MaxValue);
    }

    private void AttClick(GameObject gameObject)
    {
        SkillClickTween(gameObject.transform);
        if (!SkillCDCheck(formData.AttData)) return;
        inputCpt.inputType = UserClrType.Att;
        inputCpt.clickTime = Time.time;
    }
    private void Skill1Click(GameObject gameObject)
    {
        if (!SkillCDCheck(formData.Skills[1])) return;
        SkillClickTween(gameObject.transform.parent);
        inputCpt.inputType = UserClrType.Skill1;
        inputCpt.clickTime = Time.time;
    }
    private void Skill2Click(GameObject gameObject)
    {
        if (!SkillCDCheck(formData.Skills[2])) return;
        SkillClickTween(gameObject.transform.parent);
        inputCpt.inputType = UserClrType.Skill2;
        inputCpt.clickTime = Time.time;
    }
    private void Skill3Click(GameObject gameObject)
    {
        if (!SkillCDCheck(formData.Skills[3])) return;
        SkillClickTween(gameObject.transform.parent);
        inputCpt.inputType = UserClrType.Skill3;
        inputCpt.clickTime = Time.time;
    }

    private void SkillCDUpdate(int time)
    {
        bindingCode.imgMask1.fillAmount = 1 - (Time.time - formData.skillCDs[formData.Skills[1].Id]) / formData.Skills[1].CDTime;
        bindingCode.imgMask2.fillAmount = 1 - (Time.time - formData.skillCDs[formData.Skills[2].Id]) / formData.Skills[2].CDTime;
        bindingCode.imgMask3.fillAmount = 1 - (Time.time - formData.skillCDs[formData.Skills[3].Id]) / formData.Skills[3].CDTime;
    }

    private bool SkillCDCheck(DRSkillData skillData)
    {
        if (Time.time - formData.skillCDs[skillData.Id] < skillData.CDTime)
        {
            return false;
        }
        formData.skillCDs[skillData.Id] = Time.time;
        inputCpt.skillId = skillData.Id;
        return true;
    }

    private void SkillClickTween(Transform transform)
    {
        transform.DOScale(Vector3.one * 0.8f, 0.1f).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.1f);
        });
    }

    private void MoveDrag(GameObject gameObject, PointerEventData pointer)
    {
        Vector2 vector;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.transform as RectTransform, pointer.position, GameModule.UI.UICamera, out vector);
        if (vector.magnitude > radius)
        {
            vector = vector.normalized * radius;
        }
        inputCpt.inputType = UserClrType.Move;
        inputCpt.direction = vector;
        bindingCode.imgMove.transform.localPosition = vector;
        inputCpt.clickTime = Time.time;
    }

    private void MoveUp(GameObject gameObject)
    {
        inputCpt.inputType = UserClrType.Move;
        inputCpt.direction = Vector3.zero;
        bindingCode.imgMove.transform.localPosition = Vector3.zero;
        inputCpt.clickTime = Time.time;
    }

    public override void Close(object userData)
    {
        base.Close(userData);
        GameModule.Timer.DelTimer(CDUpdateTimer.id);
    }

    public override void Clear()
    {
        base.Clear();
        UIEventListener.RemoveListener(bindingCode.imgMove.gameObject);
        GameFormCode.UnBinding(bindingCode);
    }
}
