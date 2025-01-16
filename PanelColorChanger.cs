using UnityEngine;
using UnityEngine.UI; // UI�R���|�[�l���g���g�p���邽�߂ɕK�v

public class PanelColorChanger : MonoBehaviour
{
    private Image panelImage;

    private void Start()
    {
        // �A�^�b�`���ꂽPanel��Image�R���|�[�l���g���擾
        panelImage = GetComponent<Image>();
    }

    // �O������Ăяo���郁�\�b�h
    public void ChangeColor()
    {
        StartCoroutine(ChangeColorCoroutine());
    }

    private System.Collections.IEnumerator ChangeColorCoroutine()
    {
        // �p�l���̐F�𔒂ɕύX
        panelImage.color = Color.white;

        // 1�b�ҋ@
        yield return new WaitForSeconds(3f);

        // �p�l���̐F�����ɕύX
        panelImage.color = Color.black;
    }
}
