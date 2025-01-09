using UnityEngine;
using UnityEngine.UI; // UIコンポーネントを使用するために必要

public class PanelColorChanger : MonoBehaviour
{
    private Image panelImage;

    private bool isWhite = false;

    private void Start()
    {
        // アタッチされたPanelのImageコンポーネントを取得
        panelImage = GetComponent<Image>();
    }

    // 外部から呼び出せるメソッド
    public void ChangeColor()
    {
        StartCoroutine(ChangeColorCoroutine());
    }

    private System.Collections.IEnumerator ChangeColorCoroutine()
    {
        // パネルの色を白に変更
        panelImage.color = Color.white;
        isWhite = true;

        // 1秒待機
        yield return new WaitForSeconds(5f);

        // パネルの色を黒に変更
        panelImage.color = Color.black;
        isWhite = false;
    }

    public bool getIsWhite()
    {
        return isWhite;
    }
}
