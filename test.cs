using System;

private float sec = 0;

void Update()
{
    sec += Time.deltaTime;
    if (sec >= 3f)
    {
        Sample1();

        sec = 0; //経過時間をリセット
    }
}

private void Sample1()
{
    Debug.Log("3秒が経過しました");
}