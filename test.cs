using System;

private float sec = 0;

void Update()
{
    sec += Time.deltaTime;
    if (sec >= 3f)
    {
        Sample1();

        sec = 0; //�o�ߎ��Ԃ����Z�b�g
    }
}

private void Sample1()
{
    Debug.Log("3�b���o�߂��܂���");
}