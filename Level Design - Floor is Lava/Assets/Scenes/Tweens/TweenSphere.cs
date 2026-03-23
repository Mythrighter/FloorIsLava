using UnityEngine;
using DG.Tweening;

public class TweenSphere : MonoBehaviour
{
    public Transform PointA;
    public Transform PointB;

    private MeshRenderer render;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render = GetComponent<MeshRenderer>();
        render.material.color = Color.blue;

        //Move the sphere to the starting position.
        transform.position = PointA.position;

        //Create the tweener to move to Point B.
        //transform.DOMove(PointB.position, 2)
        //    .SetLoops(-1, LoopType.Yoyo)
        //    .SetEase(Ease.InOutSine);

        ////Create the tweener to change colors.
        //render.material.DOColor(Color.green, 2)
        //    .SetLoops(-1, LoopType.Yoyo)
        //    .SetEase(Ease.InOutSine);

        Sequence my_sequence = DOTween.Sequence();
        my_sequence.Append(transform.DOMove(PointB.position, 2).SetEase(Ease.InOutSine));
        my_sequence.Append(render.material.DOColor(Color.green, 2).SetEase(Ease.InOutSine));
        my_sequence.Append(transform.DOScale(Vector3.one * 2, 2).SetEase(Ease.InOutSine));


        //Insert tweens into the sequence so multiple tweens can play at the same time.
        my_sequence.Insert(6, transform.DOMove(PointA.position, 2).SetEase(Ease.InOutSine));
        my_sequence.Insert(6, render.material.DOColor(Color.blue, 2).SetEase(Ease.InOutSine));
        my_sequence.Insert(6, transform.DOScale(Vector3.one, 2).SetEase(Ease.InOutSine));

        //Tween the show float function.
        DOVirtual.Float(0, 10, 3, ShowFloat);
    }

    void ShowFloat(float f)
    {
        Debug.Log(f);
    }

}
