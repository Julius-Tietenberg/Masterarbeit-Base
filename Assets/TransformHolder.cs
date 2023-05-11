using UnityEngine;

public class TransformHolder : MonoBehaviour
{
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public Transform Head
    {
        get { return _head; }
        private set { _head = value; }
    }

    public Transform LeftHand
    {
        get { return _leftHand; }
        private set { _leftHand = value; }
    }

    public Transform RightHand
    {
        get { return _rightHand; }
        private set { _rightHand = value; }
    }
}
