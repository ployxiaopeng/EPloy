using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EPloy.UI;

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
                  GameModule.UI.CloseUIForm(UIName.TipsForm);
              });

        Image.SetSprite(1014);
        Text.text = Image.sprite.name;
        Image.SetNativeSize();
    }

    public override void Open(object userData)
    {

    }
}
