using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EPloy;

[UIAttribute(UIName.TipsForm)]
public class TipsForm : UIForm
{
    private Button btnClose;

    public override void Create()
    {
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        btnClose.onClick.AddListener(() =>
              {
                  GameEntry.UI.CloseUIForm(UIName.TipsForm);
              });
    }

    public override void Open(object userData)
    {

    }

    public override void Close(object userData)
    {

    }
}
