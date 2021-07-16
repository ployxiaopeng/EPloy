using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EPloy;

[UIAttribute(UIName.TipsForm)]
public class TipsForm : UIForm
{
    private Button btnClose;
    private Image Image;
    private Text Text;

    public override void Create()
    {
        Image = transform.Find("Image").GetComponent<Image>();
        Text = transform.Find("Text").GetComponent<Text>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        btnClose.onClick.AddListener(() =>
              {
                  GameEntry.UI.CloseUIForm(UIName.TipsForm);
              });

        Image.SetSprite(130142523);
        Text.text = Image.sprite.name;
        Image.SetNativeSize();
    }

    public override void Open(object userData)
    {

    }

    public override void Close(object userData)
    {

    }
}
