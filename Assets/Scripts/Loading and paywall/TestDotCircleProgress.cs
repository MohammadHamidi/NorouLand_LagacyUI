using UnityEngine;

public class TestDotCircleProgress : MonoBehaviour
{
   
    [SerializeField]
    private DotCircleProgress dotCircleProgress;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (dotCircleProgress != null)
            {
                dotCircleProgress.AnimateProgress(1f, 2f); // Animate to 100% in 2 seconds
            }
        }
    }
}