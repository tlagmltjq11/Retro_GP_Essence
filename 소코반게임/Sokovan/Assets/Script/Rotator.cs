using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    void Update()
    {
        //사양이 60프레임 일 경우 Time.deltaTime은 1/60초 이기 때문에, Update문이 1초에 60번 실행되면
        //결국 1초에 60도 씩 돌게되는 코드가 된다. -> 60/60 * 60번 = 60도
        transform.Rotate(60 * Time.deltaTime, 60 * Time.deltaTime, 60 * Time.deltaTime);
    }
}
