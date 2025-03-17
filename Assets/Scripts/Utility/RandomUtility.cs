using UnityEngine;
using UnityEngine.AI;

public static class RandomUtilit 
{
    //https://gist.github.com/tansey/1444070 
    //가우시안 분포? 정규분포??
    //중앙 집중형 분포: 대부분의 탄은 목표 근처에 떨어지고,
    //멀리 떨어지는 탄은 적게 발생하는 특성(정규분포)을
    //구현하는 데 적합합니다.
    //실제 총기나 미사일의 발사는 정확히 목표지점에만 맞지 않습니다.
    //일부는 왼쪽, 일부는 오른쪽, 또는 위, 아래로 퍼집니다.
    //이처럼 무기의 발사나 다른 무작위적인 요소에서 발생하는 흩어짐을 자연스럽게 표현하기 위해,
    //정규분포(또는 탄착군 분포)를 사용합니다.
    //좀더 공부해볼 필요가 있음 

    // 가챠 확률 ??
    public static float GetRandomNormalDistribution(float _mean, float _standard)
    { 
        var x1 = Random.Range(0f,1f);
        var x2 = Random.Range(0f,1f);
        return _mean + _standard * (Mathf.Sqrt(-2.0f *Mathf.Log(x1)) *Mathf.Sin(2.0f *Mathf.PI *x2));
    }
}
